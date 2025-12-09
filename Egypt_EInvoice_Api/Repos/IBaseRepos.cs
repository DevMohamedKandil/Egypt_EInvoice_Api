using Egypt_EInvoice_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egypt_EInvoice_Api.Repos
{
    public interface IBaseRepos<T>
    {
        T Add(T item);
        T FindByGuid(Guid guid);
        T FindById(int id);
        List<T> GetAll();
        bool DeleteByGuid(Guid guid);
        bool DeleteById(int id);
        T Update(T item);
        bool UpdateList(List<T> items);
        List<T> SearchByGuid(Guid guid);
    }
}
