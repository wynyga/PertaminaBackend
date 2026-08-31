using HashidsNet;

namespace Business.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly Hashids _hashids;

        public EncryptionService()
        {
            var salt = Environment.GetEnvironmentVariable("HASHIDS_SALT") ?? "DefaultFallbackSalt_2026";
            _hashids = new Hashids(salt, minHashLength: 8);
        }

        public string EncryptId(int id)
        {
            return _hashids.Encode(id);
        }

        public int DecryptId(string encryptedId)
        {
            var decoded = _hashids.Decode(encryptedId);
            if (decoded.Length > 0)
                return decoded[0];
            
            throw new ArgumentException("Invalid encrypted ID");
        }
    }
}
