using BL.Dtos;
using Domains;
using Domains.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IApplicationRepository : IBaseServices<Application, ApplicationDto>
    {
    }
}
