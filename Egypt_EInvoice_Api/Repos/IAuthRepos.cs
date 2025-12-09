using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.Repos
{
    public interface IAuthRepos<T>
    {
         T Login(string userName, string password);
    }
}
