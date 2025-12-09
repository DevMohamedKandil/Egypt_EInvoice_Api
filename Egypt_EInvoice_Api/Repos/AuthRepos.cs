using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Egypt_EInvoice_Api.Models;

namespace Egypt_EInvoice_Api.Repos
{
    public class AuthRepos:IAuthRepos<User>
    {
        private readonly EInvoiceDBContext context;
        public AuthRepos(EInvoiceDBContext context)
        {
            this.context = context;
        }

        public User Login(string userName, string password)
        {
            string encryptedPassword = GetEncryptedPassord(password);
            return this.context.Users.SingleOrDefault(x => x.EntryName == userName && x.Pass == encryptedPassword);            
        }

        private string GetEncryptedPassord(string password)
        {
            password = password == null ? "" : password;
            if (password.Length == 0)
            {
                password = "Azeez-Sliman-mahmmod";
            }
            Encoding encoding = Encoding.GetEncoding(1252); //To Support Extended Asscii Char
            byte[] passwordBytes = encoding.GetBytes(password);
            for (int i = 0; i < passwordBytes.Length; i++)
            {
                passwordBytes[i] = (byte)(passwordBytes[i] + 11);
            }

            //string paswordEnc = encoding.GetString(passwordBytes);
            return encoding.GetString(passwordBytes);
        }
    
    }


}
