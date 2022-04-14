namespace DomainModel.Generators
{
    public interface IProfilePicGenerator
    {
        Task<Stream> GeneratePicture();
    }
}
