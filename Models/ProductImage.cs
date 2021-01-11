namespace BasePackageModule2.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        public Product Product { get; set; }

        public int ProductId { get; set; }

        public string Img { get; set; }
        public int Order { get; set; }
    }
}