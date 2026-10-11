using App.Admin.Models.ViewModels;
using App.Application.Results;
using App.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationFileUploadRequest = App.Application.Models.Requests.FileUploadRequest;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductController(IProductService productService) : Controller
    {
        [Route("/products/")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var products = await productService.GetAllForAdminAsync();

            var viewModel = products.Select(p => new AdminProductListItemViewModel
            {
                Id = p.Id,
                Name = p.Name,
                SellerName = p.SellerName,
                Price = p.Price,
                Enabled = p.Enabled,
                ImageUrl = p.ImageUrl
            }).ToList();

            return View(viewModel);
        }

        [Route("/products/filter")]
        [HttpGet]
        public IActionResult Filter([FromQuery] object filterOptions)
        {
            // will return filtered products as json
            return Json(new { });
        }

        [Route("/products/{productId:int}/images")]
        [HttpGet]
        public async Task<IActionResult> Images([FromRoute] int productId)
        {
            var result = await productService.GetProductImagesForAdminAsync(productId);

            if (!result.Success || result.Data is null)
            {
                return NotFound();
            }

            return View(new AdminProductImagesViewModel
            {
                ProductId = result.Data.ProductId,
                ProductName = result.Data.ProductName,
                CurrentImages = result.Data.Images
            });
        }

        [Route("/products/{productId:int}/images")]
        [HttpPost]
        public async Task<IActionResult> Images([FromRoute] int productId, [FromForm] AdminProductImagesViewModel model)
        {
            model.ProductId = productId;

            if (!ModelState.IsValid)
            {
                await RefreshCurrentImagesAsync(productId, model);
                return View(model);
            }

            if (model.NewImages.Count == 0)
            {
                ModelState.AddModelError(nameof(AdminProductImagesViewModel.NewImages), "En az bir görsel seçmelisiniz.");
                await RefreshCurrentImagesAsync(productId, model);
                return View(model);
            }

            // Controller yalnızca IFormFile -> Stream dönüşümü yapar; asıl dosya işlemleri
            // Application katmanındaki ProductService üzerinden File API'ye devredilir.
            var images = ToFileUploadRequests(model.NewImages);

            try
            {
                var result = await productService.AddProductImagesAsAdminAsync(productId, images);

                if (!result.Success)
                {
                    if (result.Type == ServiceResultType.NotFound)
                    {
                        return NotFound();
                    }

                    ModelState.AddModelError(string.Empty, result.Message ?? "Görseller eklenemedi.");
                    await RefreshCurrentImagesAsync(productId, model);
                    return View(model);
                }

                ViewBag.SuccessMessage = result.Message;
                model.NewImages = [];

                await RefreshCurrentImagesAsync(productId, model);
                return View(model);
            }
            finally
            {
                DisposeStreams(images);
            }
        }

        [Route("/products/{productId:int}/images/{imageId:int}/delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteImage([FromRoute] int productId, [FromRoute] int imageId)
        {
            var result = await productService.DeleteProductImageAsAdminAsync(productId, imageId);

            if (!result.Success)
            {
                return result.Type switch
                {
                    ServiceResultType.NotFound => NotFound(),
                    _ => BadRequest(result.Message)
                };
            }

            return RedirectToAction(nameof(Images), new { productId });
        }

        [Route("/products/{productId:int}/delete")]
        [HttpGet]
        public IActionResult Delete([FromRoute] int productId)
        {
            return View();
        }

        private async Task RefreshCurrentImagesAsync(int productId, AdminProductImagesViewModel model)
        {
            var refreshed = await productService.GetProductImagesForAdminAsync(productId);

            if (refreshed.Success && refreshed.Data is not null)
            {
                model.ProductName = refreshed.Data.ProductName;
                model.CurrentImages = refreshed.Data.Images;
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
    }
}
