namespace App.Application.Models.Dtos
{
    /// <summary>
    /// Ürün düzenleme ekranını doldurmak için kullanılan; mevcut alanlar ile birlikte
    /// hâlihazırda yüklenmiş görselleri de içeren DTO.
    /// </summary>
    public class ProductEditDto
    {
        public int CategoryId { get; set; }
        public int? DiscountId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public byte StockAmount { get; set; }
        public List<ProductImageDto> Images { get; set; } = [];
    }
}
