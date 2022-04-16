namespace DomainModel.Generators
{
    public interface IRandomTextGenerator
    {
        Task<string?> GenerateRandomText();
    }
}
