namespace Infrastructure.WebGallery
{
    public interface IWebGalleryOptions
    {
        string User { get; }
        string ApiEndpoint { get; }
        string FileServerEndpoint { get; }
    }
}
