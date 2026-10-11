using App.Application.Models.Requests;
using App.Application.Services.Interfaces;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class HomeController(IContactService contactService, IProductService productService) : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/about-us")]
        public IActionResult AboutUs()
        {
            return View();
        }

        [HttpGet("/contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost("/contact")]
        public async Task<IActionResult> Contact([FromForm] NewContactFormMessageViewModel newContactMessage)
        {
            if (!ModelState.IsValid)
            {
                return View(newContactMessage);
            }

            var result = await contactService.SubmitAsync(new ContactCreateRequest
            {
                Name = newContactMessage.Name,
                Email = newContactMessage.Email,
                Message = newContactMessage.Message,
            });

            ViewBag.SuccessMessage = result.Message;

            return View();
        }

        [HttpGet("/product/list")]
        public async Task<IActionResult> Listing([FromQuery] string? search = null)
        {
            // TODO: add paging support
            var products = await productService.GetListingAsync(search);

            var viewModel = products.Select(p => new ProductListingViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.CategoryName,
                DiscountPercentage = p.DiscountPercentage,
                ImageUrl = p.ImageUrl
            }).ToList();

            return View(viewModel);
        }

        [HttpGet("/product/{productId:int}/details")]
        public async Task<IActionResult> ProductDetail([FromRoute] int productId)
        {
            var result = await productService.GetDetailAsync(productId);

            if (!result.Success || result.Data is null)
            {
                return NotFound();
            }

            var product = result.Data;

            var viewModel = new HomeProductDetailViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                DiscountRate = product.DiscountRate,
                Description = product.Description,
                StockAmount = product.StockAmount,
                SellerName = product.SellerName,
                CategoryName = product.CategoryName,
                CategoryId = product.CategoryId,
                ImageUrls = product.ImageUrls,
                Reviews = product.Comments.Select(c => new ProductReviewViewModel
                {
                    Id = c.Id,
                    Text = c.Text,
                    StarCount = c.StarCount,
                    UserName = c.UserName
                }).ToArray()
            };

            return View(viewModel);
        }
    }
}
