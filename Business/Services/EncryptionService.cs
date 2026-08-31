using HashidsNet;

namespace Business.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly Hashids _hashids;

        public EncryptionService()
        {
            // Salt is hardcoded here for simplicity, but ideally should be in .env
            _hashids = new Hashids("PertaminaBackend_SuperSecretSalt_2026", minHashLength: 8);
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
