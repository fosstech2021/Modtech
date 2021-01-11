namespace BasePackageModule2.Helpers
{
    public interface IInstaMojoConfiguration
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
    }

    public class InstaMojoConfiguration : IInstaMojoConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
