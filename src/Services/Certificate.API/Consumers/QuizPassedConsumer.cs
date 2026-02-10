using Amazon.Runtime.Internal.Util;
using Certificate.API.Data;
using Certificate.API.Models;
using Certificate.API.Services;
using EventBus.Messages.Events;
using MassTransit;

namespace Certificate.API.Consumers;

public class QuizPassedConsumer : IConsumer<QuizPassedEvent>
{
    private readonly CertificateGenerator _certificateGenerator;
    private readonly S3StorageService _storage;
    private readonly CertificateDbContext _dbContext;
    private readonly ILogger<QuizPassedConsumer> _logger;

    public QuizPassedConsumer(CertificateGenerator certificateGenerator, S3StorageService storage, CertificateDbContext dbContext, ILogger<QuizPassedConsumer> logger)
    {
        _certificateGenerator = certificateGenerator;
        _storage = storage;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<QuizPassedEvent> context)
    {
        var data = context.Message;
        _logger.LogInformation("Received QuizPassedEvent for User: {UserId}, Quiz: {QuizTitle}", data.UserId, data.QuizTitle);

        // Generate certificate
        _logger.LogInformation("Generating PDF certificate...");
        var pdfBytes = _certificateGenerator.Generate(data.UserId, data.QuizTitle, (int)data.Score);

        // add to MinIO
        var fileName = $"{data.UserId}_{data.QuizTitle}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var s3key = await _storage.UploadFileAsync(pdfBytes, fileName);
        _logger.LogInformation("Uploaded to MinIO. S3Key: {S3Key}", s3key);

        // Save certificate record to database
        _dbContext.Certificates.Add(new CertificateMetaData
        {
            Id = Guid.NewGuid(),
            UserId = data.UserId,
            S3Key = s3key,
            IssuedAt = DateTimeOffset.UtcNow
        });
    }
}
