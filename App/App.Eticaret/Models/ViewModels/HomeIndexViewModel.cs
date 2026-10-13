using App.Application.Models.Dtos;

namespace App.Eticaret.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<CategoryDto> Categories { get; set; } = [];
        public List<ProductListDto> NewArrivals { get; set; } = [];
        public List<ProductListDto> Discounted { get; set; } = [];
        public List<ProductListDto> TopRated { get; set; } = [];
        public int ProductCount { get; set; }
        public int DiscountedCount { get; set; }
    }
}
