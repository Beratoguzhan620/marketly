namespace App.Application.Models.Dtos
{
    /// <summary>
    /// Admin panelindeki ürün görsel yönetimi ekranını doldurmak için kullanılan DTO.
    /// </summary>
    public class AdminProductImagesDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public List<ProductImageDto> Images { get; set; } = [];
    }
}
