namespace App.Application.Models.Requests
{
    public class ProductUpdateRequest
    {
        public int CategoryId { get; set; }
        public int? DiscountId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public byte StockAmount { get; set; }

        /// <summary>
        /// Ürüne ek olarak yüklenecek yeni görseller (mevcut görsellere ekleme yapılır).
        /// </summary>
        public List<FileUploadRequest> NewImages { get; set; } = [];
    }
}
