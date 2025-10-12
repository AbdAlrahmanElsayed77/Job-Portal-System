using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.Contracts;
using Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class CVFileService : BaseService<CVFile, CVFileDto>, ICVFileRepository
    {
        public CVFileService(ITableRepository<CVFile> repo, IMapper mapper) : base(repo, mapper)
        {
        }
    }
}
