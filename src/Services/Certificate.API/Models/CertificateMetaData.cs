namespace Certificate.API.Models;

public class CertificateMetaData
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public string S3Key { get; set; } = default!; //route to file in MinIO
    public DateTimeOffset IssuedAt { get; set; }
}
