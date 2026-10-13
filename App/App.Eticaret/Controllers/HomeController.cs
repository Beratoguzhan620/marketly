using App.Application.Models.Dtos;
using App.Application.Models.Requests;
using App.Application.Services.Interfaces;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class HomeController(
        IContactService contactService,
        IProductService productService,
        ICategoryService categoryService) : BaseController
    {
        public async Task<IActionResult> Index()
        {
            var newArrivals = await productService.GetListingAsync(new ProductListingQuery { Sort = ProductSort.Newest, PageSize = 8 });
            var discounted = await productService.GetListingAsync(new ProductListingQuery { OnlyDiscounted = true, Sort = ProductSort.PriceAsc, PageSize = 4 });
            var topRated = await productService.GetListingAsync(new ProductListingQuery { Sort = ProductSort.TopRated, PageSize = 4 });

            var viewModel = new HomeIndexViewModel
            {
                Categories = (await categoryService.GetAllAsync()).Where(c => c.ProductCount > 0).ToList(),
                NewArrivals = newArrivals.Items,
                Discounted = discounted.Items,
                TopRated = topRated.Items.Where(p => p.ReviewCount > 0).ToList(),
                ProductCount = newArrivals.TotalCount,
                DiscountedCount = discounted.TotalCount,
            };

            return View(viewModel);
        }

        [HttpGet("/not-found")]
        public IActionResult PageNotFound()
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View("Status", new StatusPageViewModel(404, "Sayfa bulunamadı", "Aradığınız sayfa kaldırılmış ya da adresi değişmiş olabilir."));
        }

        [HttpGet("/access-denied")]
        public IActionResult AccessDenied()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return View("Status", new StatusPageViewModel(403, "Bu sayfaya erişiminiz yok", "Bu işlem için alıcı veya satıcı hesabıyla giriş yapmanız gerekiyor."));
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
        public async Task<IActionResult> Listing(
            [FromQuery] string? search = null,
            [FromQuery] int? category = null,
            [FromQuery] decimal? min = null,
            [FromQuery] decimal? max = null,
            [FromQuery] bool discounted = false,
            [FromQuery] ProductSort sort = ProductSort.Newest,
            [FromQuery] int page = 1)
        {
            if (min is not null && max is not null && min > max)
            {
                (min, max) = (max, min);
            }

            var query = new ProductListingQuery
            {
                Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
                CategoryId = category,
                MinPrice = min is >= 0 ? min : null,
                MaxPrice = max is >= 0 ? max : null,
                OnlyDiscounted = discounted,
                Sort = Enum.IsDefined(sort) ? sort : ProductSort.Newest,
                Page = page,
            };

            var viewModel = new ProductListingPageViewModel
            {
                Query = query,
                Result = await productService.GetListingAsync(query),
                Categories = await categoryService.GetAllAsync(),
            };

            // The service clamps out-of-range pages; reflect that in the links.
            query.Page = viewModel.Result.Page;

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

            var related = await productService.GetListingAsync(new ProductListingQuery
            {
                CategoryId = product.CategoryId,
                ExcludeProductId = product.Id,
                PageSize = 4,
            });

            var viewModel = new HomeProductDetailViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                FinalPrice = product.FinalPrice,
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
                    UserName = c.UserName,
                    CreatedAt = c.CreatedAt,
                }).ToArray(),
                RelatedProducts = related.Items,
            };

            return View(viewModel);
        }
    }
}
