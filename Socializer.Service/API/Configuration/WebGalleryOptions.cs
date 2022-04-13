using Infrastructure.WebGallery;

namespace API.Configuration
{
    public class WebGalleryOptions : IWebGalleryOptions
    {
        private IConfigurationSection _webGallerySection;

        public WebGalleryOptions(IConfiguration configuration)
        {
            _webGallerySection = configuration.GetSection("WebGallery");
        }

        public string User => _webGallerySection.GetValue<string>("User");

        public string ApiEndpoint => _webGallerySection.GetValue<string>("ApiEndpoint");

        public string FileServerEndpoint => _webGallerySection.GetValue<string>("FileServerEndpoint");
    }
}
