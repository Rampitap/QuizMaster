using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace Certificate.API.Services;

public class S3StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _config;
    private readonly string _bucketName = "certificates";
    private readonly ILogger<S3StorageService> _logger;

    public S3StorageService(IAmazonS3 s3Client, IConfiguration config, ILogger<S3StorageService> logger)
    {
        _s3Client = s3Client;
        _config = config;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(byte[] content, string fileName)
    {
        //check wether the buvket was created
        var exists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _bucketName);

        if (!exists) 
        {
            await _s3Client.PutBucketAsync(_bucketName);
            _logger.LogInformation("Bucket '{BucketName}' created successfully.", _bucketName);
        }

        var request = new PutObjectRequest 
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = new MemoryStream(content),
            ContentType = "application/pdf"
        };

        await _s3Client.PutObjectAsync(request);
        return fileName;
    }
}
