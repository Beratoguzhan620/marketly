using App.Admin.Models.ViewModels;
using App.Application.Models.Requests;
using App.Application.Results;
using App.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Route("/categories")]
    [Authorize(Roles = "admin")]
    public class CategoryController(ICategoryService categoryService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var categories = await categoryService.GetAllAsync();

            var viewModel = categories.Select(c => new CategoryListViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                IconCssClass = c.IconCssClass,
                ProductCount = c.ProductCount,
            }).ToList();

            return View(viewModel);
        }

        [Route("create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] SaveCategoryViewModel newCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return View(newCategoryModel);
            }

            var result = await categoryService.CreateAsync(new CategoryCreateRequest
            {
                Name = newCategoryModel.Name,
                Color = newCategoryModel.Color,
                IconCssClass = newCategoryModel.IconCssClass,
            });

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Kategori oluşturulamadı.");
                return View(newCategoryModel);
            }

            TempData["SuccessMessage"] = result.Message ?? "Kategori eklendi.";
            return RedirectToAction(nameof(List));
        }

        [Route("{categoryId:int}/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int categoryId)
        {
            var result = await categoryService.GetByIdAsync(categoryId);

            if (!result.Success || result.Data is null)
            {
                return NotFound();
            }

            var editCategoryModel = new SaveCategoryViewModel
            {
                Name = result.Data.Name,
                Color = result.Data.Color,
                IconCssClass = result.Data.IconCssClass
            };

            return View(editCategoryModel);
        }

        [Route("{categoryId:int}/edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int categoryId, [FromForm] SaveCategoryViewModel editCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editCategoryModel);
            }

            var result = await categoryService.UpdateAsync(categoryId, new CategoryUpdateRequest
            {
                Name = editCategoryModel.Name,
                Color = editCategoryModel.Color,
                IconCssClass = editCategoryModel.IconCssClass,
            });

            if (!result.Success)
            {
                if (result.Type == ServiceResultType.NotFound)
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "Kategori güncellenemedi.");
                return View(editCategoryModel);
            }

            TempData["SuccessMessage"] = result.Message ?? "Kategori güncellendi.";
            return RedirectToAction(nameof(List));
        }

        [Route("{categoryId:int}/delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int categoryId)
        {
            var result = await categoryService.DeleteAsync(categoryId);

            if (!result.Success)
            {
                if (result.Type == ServiceResultType.NotFound)
                {
                    return NotFound();
                }

                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = result.Message ?? "Kategori silindi.";
            }

            return RedirectToAction(nameof(List));
        }
    }
}
