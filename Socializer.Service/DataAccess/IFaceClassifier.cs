namespace Infrastructure
{
    public interface IFaceClassifier
    {
        Task<UserFaceFeatures> ClassifyUserFace(Stream faceImage);
    }
}
