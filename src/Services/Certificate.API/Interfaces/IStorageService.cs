namespace Certificate.API.Interfaces;

public interface IStorageService
{
    public Task<string> UploadFileAsync(byte[] content, string fileName);
}
