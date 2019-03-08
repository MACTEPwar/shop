using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop_api 
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "http://localhost:51884/"; // потребитель токена
        const string KEY = "Accessdeniedrroma2005!@#!@#!@#";   // ключ для шифрации
        public const int LIFETIME = 60 * 24; // время жизни токена - 24 часа
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
