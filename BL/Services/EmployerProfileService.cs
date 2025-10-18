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
    public class EmployerProfileService : BaseService<EmployerProfile,EmployerProfileDto>, IEmployerProfileService
    {
        private readonly ITableRepository<EmployerProfile> _repo;
        private readonly IMapper _mapper;
        public EmployerProfileService(ITableRepository<EmployerProfile> repo, IMapper mapper) : base(repo, mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public EmployerProfileDto GetByUserId(Guid userId)
        {
            var profile = _repo.GetAll(x => x.UserId == userId).FirstOrDefault();
            return _mapper.Map<EmployerProfileDto>(profile);
        }
    }
}
