namespace Business.Services
{
    public interface IEncryptionService
    {
        string EncryptId(int id);
        int DecryptId(string encryptedId);
    }
}
