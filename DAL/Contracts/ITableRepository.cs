using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contracts
{
    public interface ITableRepository<T> where T : class
    {
        List<T> GetAll(params Expression<Func<T, object>>[]? includes);
        T GetById(Guid id, params Expression<Func<T, object>>[]? includes);
        bool Add(T entity);
        bool Update(T entity);
        bool Delete(Guid id);
        bool ChangeStatus(Guid id, int status = 1);
    }
}
