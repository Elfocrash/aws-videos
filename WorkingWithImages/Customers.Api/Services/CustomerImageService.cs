using Amazon.S3;
using Amazon.S3.Model;

namespace Customers.Api.Services;

public class CustomerImageService : ICustomerImageService
{
    private readonly IAmazonS3 _s3;
    private const string BucketName = "nickchapsasvideo";

    public CustomerImageService(IAmazonS3 s3)
    {
        _s3 = s3;
    }

    public async Task<PutObjectResponse> UploadImageAsync(Guid id, IFormFile file)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = $"profile_images/{id}",
            ContentType = file.ContentType,
            InputStream = file.OpenReadStream(),
            Metadata =
            {
                ["x-amz-meta-originalname"] = file.FileName,
                ["x-amz-meta-extension"] = Path.GetExtension(file.FileName),
            }
        };

        return await _s3.PutObjectAsync(putObjectRequest);
    }

    public async Task<GetObjectResponse?> GetImageAsync(Guid id)
    {
        try
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = $"profile_images/{id}"
            };
        
            return await _s3.GetObjectAsync(getObjectRequest);
        }
        catch (AmazonS3Exception ex) when (ex.Message is "The specified key does not exist.")
        {
            return null;
        }
    }

    public async Task<DeleteObjectResponse> DeleteImageAsync(Guid id)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = BucketName,
            Key = $"profile_images/{id}"
        };

        return await _s3.DeleteObjectAsync(deleteObjectRequest);
    }
}

public interface ICustomerImageService
{
    Task<PutObjectResponse> UploadImageAsync(Guid id, IFormFile file);

    Task<GetObjectResponse?> GetImageAsync(Guid id);

    Task<DeleteObjectResponse> DeleteImageAsync(Guid id);
}


