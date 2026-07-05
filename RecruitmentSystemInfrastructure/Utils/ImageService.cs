using Amazon.S3;
using Amazon.S3.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using RecruitmentSystemApplication.Services.Profile;
namespace RecruitmentSystemInfrastructure.Utils
{
    public class ImageService(IAmazonS3 s3Client) : IImageService
    {
        private const string PublicBaseUrl = "https://pub-8d61ae007ced485d8b04d21436dec6b9.r2.dev";
        public async Task<string> SaveImage(Stream fileStream)
        {
            using (var image = Image.Load(fileStream))
            {
                image.Mutate(x => x.Resize(
                    new ResizeOptions
                    {
                        Size = new Size(400, 400),
                        Mode = ResizeMode.Max
                    }));

                using (var outputStream = new MemoryStream())
                {
                    image.SaveAsWebp(outputStream, new WebpEncoder { Quality = 80 });

                    outputStream.Position = 0;

                    string uniqueFileName = $"{Guid.NewGuid()}.webp";

                    var putRequest = new PutObjectRequest()
                    {
                        BucketName = "recruitmentsystem",
                        Key = $"avatars/{uniqueFileName}",
                        InputStream = outputStream,
                        ContentType = "image/webp",
                        DisablePayloadSigning = true,
                        DisableDefaultChecksumValidation = true
                    };

                    await s3Client.PutObjectAsync(putRequest);

                    string imageUrl = $"{PublicBaseUrl}/{putRequest.Key}";

                    return imageUrl;
                }
            }
        }
    }
}
