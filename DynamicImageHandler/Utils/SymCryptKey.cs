// // --------------------------------------------------------------------------------------------------------------------
// // Copyright (c) 2009-2010 Esben Carlsen
// // Forked Copyright (c) 2011-2017 Jaben Cargman and CaptiveAire Systems
// // 
// // This library is free software; you can redistribute it and/or
// // modify it under the terms of the GNU Lesser General Public
// // License as published by the Free Software Foundation; either
// // version 2.1 of the License, or (at your option) any later version.
// 
// // This library is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// // Lesser General Public License for more details.
// 
// // You should have received a copy of the GNU Lesser General Public
// // License along with this library; if not, write to the Free Software
// // Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA 
// // --------------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace DynamicImageHandler.Utils
{
    public class SymCryptKey : ISymCryptKey
    {
        public SymCryptKey(string password)
        {
            var salt = Encoding.ASCII.GetBytes(password.Substring(0, 10));

            var secretKey = new Rfc2898DeriveBytes(password, salt);

            this.Key = secretKey.GetBytes(32);
            this.Salt = secretKey.GetBytes(16);
        }

        public byte[] Key { get; }

        public byte[] Salt { get; }
    }
}