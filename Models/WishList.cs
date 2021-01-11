namespace BasePackageModule2.Models
{
    public class WishList
    {
        public int Id { get; set; }

        public ApplicationUser User { get; set; }
        public int UserId { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }  

    }
}