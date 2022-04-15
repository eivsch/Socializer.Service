using DomainModel;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Infrastructure
{
    public class UserFace
    {
        public double? Age { get; set; }
        public string EmotionType { get; set; }
        public bool HasFacialHair { get; set; }
        public string? GlassesType { get; set; }
        public string? HairColor { get; set; }
        public double? Smile { get; set; }
        public GenderType? Gender { get; set; }
        public string? RecognitionQuality { get; set; }
    }

    public class FaceReqClient : IFaceClassifier
    {
        const string SUBSCRIPTION_KEY = "e6e74b463e6342579b26f5b555e88ecb";
        const string ENDPOINT = "https://web-gallery-facereq.cognitiveservices.azure.com/";
        // Sample images:
        const string IMAGE_BASE_URL = "https://csdx.blob.core.windows.net/resources/Face/Images/";

        public async Task<UserFace> ClassifyUserFace(Stream faceImage)
        {
            const string RECOGNITION_MODEL4 = RecognitionModel.Recognition04;
            IFaceClient client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);
            var face = await DetectFace(client, faceImage, RECOGNITION_MODEL4);

            return face;
        }

        IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        async Task<UserFace?> DetectFace(IFaceClient client, Stream imageStream, string recognitionModel)
        {
            IList<DetectedFace> detectedFaces;
            detectedFaces = await client.Face.DetectWithStreamAsync(imageStream,
                returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Accessories, FaceAttributeType.Age,
                            FaceAttributeType.Emotion, FaceAttributeType.FacialHair,
                            FaceAttributeType.Glasses, FaceAttributeType.Hair,
                            FaceAttributeType.Makeup, FaceAttributeType.Noise,
                            FaceAttributeType.Smile, FaceAttributeType.Gender, FaceAttributeType.QualityForRecognition },
                // We specify detection model 1 because we are retrieving attributes.
                detectionModel: DetectionModel.Detection01,
                recognitionModel: recognitionModel);

            if (detectedFaces.Count > 1)
                throw new Exception("Too many faces in image.");
            if (detectedFaces.Count == 0)
                return null;

            var userFace = new UserFace();
            var face = detectedFaces.Single();

            userFace.Age = face.FaceAttributes.Age;
            userFace.HasFacialHair = face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0;
            userFace.GlassesType = face.FaceAttributes.Glasses.ToString();
            userFace.Smile = face.FaceAttributes.Smile;
            userFace.Gender = face.FaceAttributes.Gender?.ToString().ToLower() == "male" ? GenderType.Male : GenderType.Female;
            userFace.RecognitionQuality = face.FaceAttributes.QualityForRecognition.ToString();

            // Get emotion on the face
            string emotionType = string.Empty;
            double emotionValue = 0.0;
            Emotion emotion = face.FaceAttributes.Emotion;
            if (emotion.Anger > emotionValue) { emotionValue = emotion.Anger; emotionType = "Anger"; }
            if (emotion.Contempt > emotionValue) { emotionValue = emotion.Contempt; emotionType = "Contempt"; }
            if (emotion.Disgust > emotionValue) { emotionValue = emotion.Disgust; emotionType = "Disgust"; }
            if (emotion.Fear > emotionValue) { emotionValue = emotion.Fear; emotionType = "Fear"; }
            if (emotion.Happiness > emotionValue) { emotionValue = emotion.Happiness; emotionType = "Happiness"; }
            if (emotion.Neutral > emotionValue) { emotionValue = emotion.Neutral; emotionType = "Neutral"; }
            if (emotion.Sadness > emotionValue) { emotionValue = emotion.Sadness; emotionType = "Sadness"; }
            if (emotion.Surprise > emotionValue) { emotionType = "Surprise"; }
            userFace.EmotionType = emotionType;

            // Get hair color
            Hair hair = face.FaceAttributes.Hair;
            string color = null;
            if (hair.HairColor.Count == 0) { if (hair.Invisible) { color = "Invisible"; } else { color = "Bald"; } }
            HairColorType returnColor = HairColorType.Unknown;
            double maxConfidence = 0.0f;
            foreach (HairColor hairColor in hair.HairColor)
            {
                if (hairColor.Confidence <= maxConfidence) { continue; }
                maxConfidence = hairColor.Confidence; returnColor = hairColor.Color; color = returnColor.ToString();
            }
            userFace.HairColor = color;

            return userFace;
        }



        public void Detect()
        {
            const string RECOGNITION_MODEL4 = RecognitionModel.Recognition04;

            // Authenticate.
            IFaceClient client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);

            // Detect - get features from faces.
            DetectFaceExtract(client, IMAGE_BASE_URL, RECOGNITION_MODEL4).Wait();
        }

        /* 
         * DETECT FACES
         * Detects features from faces and IDs them.
         */
        public static async Task DetectFaceExtract(IFaceClient client, string url, string recognitionModel)
        {
            Console.WriteLine("========DETECT FACES========");
            Console.WriteLine();

            // Create a list of images
            List<string> imageFileNames = new List<string>
                    {
                        "detection1.jpg",    // single female with glasses
                        // "detection2.jpg", // (optional: single man)
                        // "detection3.jpg", // (optional: single male construction worker)
                        // "detection4.jpg", // (optional: 3 people at cafe, 1 is blurred)
                        // "detection5.jpg",    // family, woman child man
                        // "detection6.jpg"     // elderly couple, male female
                    };

            foreach (var imageFileName in imageFileNames)
            {
                IList<DetectedFace> detectedFaces;

                // Detect faces with all attributes from image url.
                //detectedFaces = await client.Face.DetectWithUrlAsync($"{url}{imageFileName}",
                //        returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Accessories, FaceAttributeType.Age,
                //FaceAttributeType.Emotion, FaceAttributeType.FacialHair,
                //FaceAttributeType.Glasses, FaceAttributeType.Hair,
                //FaceAttributeType.Makeup, FaceAttributeType.Noise,
                //FaceAttributeType.Smile, FaceAttributeType.Gender, FaceAttributeType.QualityForRecognition },
                //        // We specify detection model 1 because we are retrieving attributes.
                //        detectionModel: DetectionModel.Detection01,
                //        recognitionModel: recognitionModel);

                
                Stream imageStream = File.OpenRead(@"C:\temp\Pics\testuser\socializer_profilepics\abdurrahman.charbonneau.jpg");
                detectedFaces = await client.Face.DetectWithStreamAsync(imageStream,
                    returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Accessories, FaceAttributeType.Age,
                            FaceAttributeType.Emotion, FaceAttributeType.FacialHair,
                            FaceAttributeType.Glasses, FaceAttributeType.Hair,
                            FaceAttributeType.Makeup, FaceAttributeType.Noise,
                            FaceAttributeType.Smile, FaceAttributeType.Gender, FaceAttributeType.QualityForRecognition },
                    // We specify detection model 1 because we are retrieving attributes.
                    detectionModel: DetectionModel.Detection01,
                    recognitionModel: recognitionModel);

                Console.WriteLine($"{detectedFaces.Count} face(s) detected from image `{imageFileName}`.");

                // Parse and print all attributes of each detected face.
                foreach (var face in detectedFaces)
                {
                    Console.WriteLine($"Face attributes for {imageFileName}:");

                    // Get bounding box of the faces
                    Console.WriteLine($"Rectangle(Left/Top/Width/Height) : {face.FaceRectangle.Left} {face.FaceRectangle.Top} {face.FaceRectangle.Width} {face.FaceRectangle.Height}");

                    // Get accessories of the faces
                    List<Accessory> accessoriesList = (List<Accessory>)face.FaceAttributes.Accessories;
                    int count = face.FaceAttributes.Accessories.Count;
                    string accessory; string[] accessoryArray = new string[count];
                    if (count == 0) { accessory = "NoAccessories"; }
                    else
                    {
                        for (int i = 0; i < count; ++i) { accessoryArray[i] = accessoriesList[i].Type.ToString(); }
                        accessory = string.Join(",", accessoryArray);
                    }
                    Console.WriteLine($"Accessories : {accessory}");

                    // Get face other attributes
                    Console.WriteLine($"Age : {face.FaceAttributes.Age}");

                    // Get emotion on the face
                    string emotionType = string.Empty;
                    double emotionValue = 0.0;
                    Emotion emotion = face.FaceAttributes.Emotion;
                    if (emotion.Anger > emotionValue) { emotionValue = emotion.Anger; emotionType = "Anger"; }
                    if (emotion.Contempt > emotionValue) { emotionValue = emotion.Contempt; emotionType = "Contempt"; }
                    if (emotion.Disgust > emotionValue) { emotionValue = emotion.Disgust; emotionType = "Disgust"; }
                    if (emotion.Fear > emotionValue) { emotionValue = emotion.Fear; emotionType = "Fear"; }
                    if (emotion.Happiness > emotionValue) { emotionValue = emotion.Happiness; emotionType = "Happiness"; }
                    if (emotion.Neutral > emotionValue) { emotionValue = emotion.Neutral; emotionType = "Neutral"; }
                    if (emotion.Sadness > emotionValue) { emotionValue = emotion.Sadness; emotionType = "Sadness"; }
                    if (emotion.Surprise > emotionValue) { emotionType = "Surprise"; }
                    Console.WriteLine($"Emotion : {emotionType}");

                    // Get more face attributes
                    Console.WriteLine($"FacialHair : {string.Format("{0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No")}");
                    Console.WriteLine($"Glasses : {face.FaceAttributes.Glasses}");

                    // Get hair color
                    Hair hair = face.FaceAttributes.Hair;
                    string color = null;
                    if (hair.HairColor.Count == 0) { if (hair.Invisible) { color = "Invisible"; } else { color = "Bald"; } }
                    HairColorType returnColor = HairColorType.Unknown;
                    double maxConfidence = 0.0f;
                    foreach (HairColor hairColor in hair.HairColor)
                    {
                        if (hairColor.Confidence <= maxConfidence) { continue; }
                        maxConfidence = hairColor.Confidence; returnColor = hairColor.Color; color = returnColor.ToString();
                    }
                    Console.WriteLine($"Hair : {color}");

                    // Get more attributes
                    Console.WriteLine($"Makeup : {string.Format("{0}", (face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No")}");
                    Console.WriteLine($"Noise : {face.FaceAttributes.Noise.NoiseLevel}");
                    Console.WriteLine($"Smile : {face.FaceAttributes.Smile}");
                    Console.WriteLine($"Gender : {face.FaceAttributes.Gender}");

                    // Get quality for recognition attribute
                    Console.WriteLine($"QualityForRecognition : {face.FaceAttributes.QualityForRecognition}");
                    Console.WriteLine();
                }
            }
        }
    }
}
