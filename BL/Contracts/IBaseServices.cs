using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IBaseServices<T,DTO>
    {
        List<DTO> GetAll(params Expression<Func<T, object>>[]? includes);
        DTO GetById(Guid id, params Expression<Func<T, object>>[]? includes);
        bool Add(DTO entity);
        bool Update(DTO entity, Guid userId);
        bool ChangeStatus(Guid id, int status = 1);
    }
}
