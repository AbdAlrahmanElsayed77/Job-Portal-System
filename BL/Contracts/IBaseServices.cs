using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IBaseServices<T,DTO>
    {
        List<DTO> GetAll();
        DTO GetById(Guid id);
        bool Add(DTO entity);
        bool Update(DTO entity);
        bool ChangeStatus(Guid id, int status = 1);
    }
}
