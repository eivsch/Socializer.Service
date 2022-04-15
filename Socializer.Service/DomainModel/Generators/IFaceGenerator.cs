namespace DomainModel.Generators
{
    public interface IFaceGenerator
    {
        Task<Stream> GenerateFace();
    }
}
