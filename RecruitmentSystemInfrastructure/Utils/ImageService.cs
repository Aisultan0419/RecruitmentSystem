using Amazon.S3;
using Amazon.S3.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using RecruitmentSystemApplication.Services.Profile;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
namespace RecruitmentSystemInfrastructure.Utils
{
    public class ImageService(IAmazonS3 s3Client) : IImageService
    {
        private const string PublicBaseUrl = "https://pub-8d61ae007ced485d8b04d21436dec6b9.r2.dev";
        public async Task<string> SaveImage(Stream fileStream, bool isAvatar)
        {
            using (var image = Image.Load(fileStream))
            {

                image.Mutate(x => x.Resize(
                    new ResizeOptions
                    {
                        Size = isAvatar ? new Size(400, 400) : new Size(1200, 1200),
                        Mode = ResizeMode.Max,
                        Sampler = isAvatar ? KnownResamplers.Bicubic : KnownResamplers.Lanczos3
                    }));

                using (var outputStream = new MemoryStream())
                {
                    image.SaveAsWebp(outputStream, new WebpEncoder { Quality = isAvatar ? 80 : 95 });

                    outputStream.Position = 0;

                    string uniqueFileName = $"{Guid.NewGuid()}.webp";

                    var putRequest = new PutObjectRequest()
                    {
                        BucketName = "recruitmentsystem",
                        Key = isAvatar ? $"avatars/{uniqueFileName}" : $"images/{uniqueFileName}",
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
