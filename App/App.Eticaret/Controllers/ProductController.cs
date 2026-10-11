using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationFileUploadRequest = App.Application.Models.Requests.FileUploadRequest;

namespace App.Eticaret.Controllers
{
    [Route("/product")]
    public class ProductController(IProductService productService, IProductCommentService productCommentService) : BaseController
    {
        [HttpGet("")]
        [Authorize(Roles = "seller")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Create([FromForm] SaveProductViewModel newProductModel)
        {
            if (newProductModel.Images.Count == 0)
            {
                ModelState.AddModelError(nameof(SaveProductViewModel.Images), "En az bir ürün görseli yüklemelisiniz.");
            }

            if (!ModelState.IsValid)
            {
                return View(newProductModel);
            }

            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            // Controller yalnızca IFormFile -> Stream dönüşümü yapar; asıl dosya işlemleri
            // Application katmanındaki ProductService üzerinden File API'ye devredilir.
            var images = ToFileUploadRequests(newProductModel.Images);

            try
            {
                var result = await productService.CreateAsync(userId.Value, new ProductCreateRequest
                {
                    CategoryId = newProductModel.CategoryId,
                    DiscountId = newProductModel.DiscountId,
                    Name = newProductModel.Name,
                    Price = newProductModel.Price,
                    Description = newProductModel.Description,
                    StockAmount = newProductModel.StockAmount,
                    Images = images
                });

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message ?? "Ürün eklenemedi.");
                    return View(newProductModel);
                }

                ViewBag.SuccessMessage = result.Message;
                ModelState.Clear();

                return View();
            }
            finally
            {
                DisposeStreams(images);
            }
        }

        private static List<ApplicationFileUploadRequest> ToFileUploadRequests(IList<IFormFile> files) =>
            files.Select(file => new ApplicationFileUploadRequest
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Length = file.Length,
                Content = file.OpenReadStream()
            }).ToList();

        private static void DisposeStreams(IEnumerable<ApplicationFileUploadRequest> images)
        {
            foreach (var image in images)
            {
                image.Content?.Dispose();
            }
        }

        [HttpGet("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit([FromRoute] int productId)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var result = await productService.GetForEditAsync(userId.Value, productId);

            if (!result.Success || result.Data is null)
            {
                return result.Type == ServiceResultType.Forbidden ? Unauthorized() : NotFound();
            }

            var product = result.Data;

            var viewModel = new SaveProductViewModel
            {
                CategoryId = product.CategoryId,
                DiscountId = product.DiscountId,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                StockAmount = product.StockAmount,
                CurrentImages = product.Images
            };

            return View(viewModel);
        }

        [HttpPost("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit([FromRoute] int productId, [FromForm] SaveProductViewModel editProductModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editProductModel);
            }

            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var newImages = ToFileUploadRequests(editProductModel.Images);

            try
            {
                var result = await productService.UpdateAsync(userId.Value, productId, new ProductUpdateRequest
                {
                    CategoryId = editProductModel.CategoryId,
                    DiscountId = editProductModel.DiscountId,
                    Name = editProductModel.Name,
                    Price = editProductModel.Price,
                    Description = editProductModel.Description,
                    StockAmount = editProductModel.StockAmount,
                    NewImages = newImages
                });

                if (!result.Success)
                {
                    if (result.Type == ServiceResultType.NotFound)
                    {
                        return NotFound();
                    }

                    if (result.Type == ServiceResultType.Forbidden)
                    {
                        return Unauthorized();
                    }

                    ModelState.AddModelError(string.Empty, result.Message ?? "Ürün güncellenemedi.");
                    return View(editProductModel);
                }

                ViewBag.SuccessMessage = result.Message;

                var refreshed = await productService.GetForEditAsync(userId.Value, productId);
                if (refreshed.Success && refreshed.Data is not null)
                {
                    editProductModel.CurrentImages = refreshed.Data.Images;
                }

                return View(editProductModel);
            }
            finally
            {
                DisposeStreams(newImages);
            }
        }

        [HttpPost("{productId:int}/images/{imageId:int}/delete")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> DeleteImage([FromRoute] int productId, [FromRoute] int imageId)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await productService.DeleteProductImageAsync(userId.Value, productId, imageId);

            if (!result.Success)
            {
                return result.Type switch
                {
                    ServiceResultType.NotFound => NotFound(),
                    ServiceResultType.Forbidden => Unauthorized(),
                    _ => BadRequest(result.Message)
                };
            }

            return RedirectToAction(nameof(Edit), new { productId });
        }

        [HttpGet("{productId:int}/delete")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Delete([FromRoute] int productId)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await productService.DeactivateAsync(userId.Value, productId);

            if (!result.Success)
            {
                if (result.Type == ServiceResultType.NotFound)
                {
                    return NotFound();
                }

                if (result.Type == ServiceResultType.Forbidden)
                {
                    return Unauthorized();
                }
            }

            ViewBag.SuccessMessage = result.Message;

            return View();
        }

        [HttpPost("{productId:int}/comment")]
        [Authorize(Roles = "buyer, seller")]
        public async Task<IActionResult> Comment([FromRoute] int productId, [FromForm] SaveProductCommentViewModel newProductCommentModel)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await productCommentService.AddCommentAsync(userId.Value, productId, new ProductCommentCreateRequest
            {
                Text = newProductCommentModel.Text,
                StarCount = newProductCommentModel.StarCount,
            });

            if (!result.Success)
            {
                return result.Type switch
                {
                    ServiceResultType.NotFound => NotFound(),
                    ServiceResultType.Forbidden => Unauthorized(),
                    _ => BadRequest(result.Message)
                };
            }

            return Ok();
        }
    }
}
