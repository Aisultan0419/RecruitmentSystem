using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.S3;
namespace RecruitmentSystemInfrastructure.Utils
{
    public static class AmazonClientInjection
    {
        public static IServiceCollection AddAmazonClient(this IServiceCollection service, IConfiguration configuration)
        {
            string ACCESS_KEY_ID = configuration["ACCESS_KEY_ID"] ?? throw new NullReferenceException("Access key id is not set(R2 storage)");
            string SECRET_ACCESS_KEY = configuration["SECRET_ACCESS_KEY"] ?? throw new NullReferenceException("Access secret id is not set(R2 storage)");
            var s3Config = new AmazonS3Config
            {
                ServiceURL = "https://7af450b0bd6097fdf2b94f0260b4ed0a.r2.cloudflarestorage.com",
                ForcePathStyle = true
            };
            service.AddSingleton<IAmazonS3>(new AmazonS3Client(ACCESS_KEY_ID, SECRET_ACCESS_KEY, s3Config));
            return service;
        }
    }
}
