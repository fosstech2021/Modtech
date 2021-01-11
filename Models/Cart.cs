namespace BasePackageModule2.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }

        public int Qty { get; set; }
    }
}