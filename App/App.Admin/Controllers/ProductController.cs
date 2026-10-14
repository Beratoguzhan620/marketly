using App.Admin.Models.ViewModels;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationFileUploadRequest = App.Application.Models.Requests.FileUploadRequest;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductController(IProductService productService, IAdminService adminService, ICategoryService categoryService) : Controller
    {
        [Route("/products")]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] AdminProductQuery query)
        {
            return View(new ProductListPageViewModel
            {
                Query = query,
                Result = await adminService.GetProductsAsync(query),
                Categories = await categoryService.GetAllAsync(),
            });
        }

        [Route("/products/bulk")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bulk([FromForm] int[] ids, [FromForm] string operation, [FromForm] string? returnUrl)
        {
            var result = operation switch
            {
                "enable" => await adminService.SetProductsEnabledAsync(ids, true),
                "disable" => await adminService.SetProductsEnabledAsync(ids, false),
                _ => ServiceResult.ValidationError("Bilinmeyen işlem."),
            };

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(List));
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
        [ValidateAntiForgeryToken]
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

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Images), new { productId });
            }
            finally
            {
                DisposeStreams(images);
            }
        }

        [Route("/products/{productId:int}/images/{imageId:int}/delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

            TempData["SuccessMessage"] = "Görsel silindi.";
            return RedirectToAction(nameof(Images), new { productId });
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
