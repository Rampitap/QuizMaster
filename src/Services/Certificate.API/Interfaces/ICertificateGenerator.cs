namespace Certificate.API.Interfaces;

public interface ICertificateGenerator
{
    public byte[] Generate(string userName, string quizTitle, int score);
}
