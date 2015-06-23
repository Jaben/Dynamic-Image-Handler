// DynamicImageHandler - Copyright (c) 2015 CaptiveAire

using System.Security.Cryptography;
using System.Text;

namespace DynamicImageHandler.Utils
{
    public class SymCryptKey : ISymCryptKey
    {
        private readonly byte[] _key;

        private readonly byte[] _salt;

        public SymCryptKey(string password)
        {
            var salt = Encoding.ASCII.GetBytes(password.Substring(0, 10));

            var secretKey = new Rfc2898DeriveBytes(password, salt);

            this._key = secretKey.GetBytes(32);
            this._salt = secretKey.GetBytes(16);
        }

        public byte[] Key
        {
            get
            {
                return this._key;
            }
        }

        public byte[] Salt
        {
            get
            {
                return this._salt;
            }
        }
    }
}