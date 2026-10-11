namespace App.Application.Models.Requests
{
    public class ProductCreateRequest
    {
        public int CategoryId { get; set; }
        public int? DiscountId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public byte StockAmount { get; set; }

        /// <summary>
        /// Ürün ile birlikte yüklenecek görsellerin ham içerikleri. ProductService bunları
        /// File API üzerinden yükleyip dönen Url/FileKey bilgisini ProductImageEntity'de saklar.
        /// </summary>
        public List<FileUploadRequest> Images { get; set; } = [];
    }
}
