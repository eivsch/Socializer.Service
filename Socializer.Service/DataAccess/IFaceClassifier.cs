namespace Infrastructure
{
    public interface IFaceClassifier
    {
        Task<UserFace> ClassifyUserFace(Stream faceImage);
    }
}
