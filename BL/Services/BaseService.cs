using AutoMapper;
using BL.Contracts;
using DAL.Contracts;
using Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class BaseService<T,DTO> : IBaseServices<T, DTO> where T : BaseTable
    {
        readonly ITableRepository<T> _repo;
        readonly IMapper _mapper;
        public BaseService(ITableRepository<T> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public List<DTO> GetAll()
        {
            var list = _repo.GetAll();
            return _mapper.Map<List<T>, List<DTO>>(list);
        }

        public DTO GetById(Guid id)
        {
            var obj = _repo.GetById(id);
            return _mapper.Map<T, DTO>(obj);
        }

        public bool Add(DTO entity)
        {
            var dbObject = _mapper.Map<DTO, T>(entity);
            dbObject.Id = Guid.NewGuid();
            return _repo.Add(dbObject);
        }

        public bool Update(DTO entity)
        {
            var dbObject = _mapper.Map<DTO, T>(entity);
            //dbObject.UpdatedBy = userId;
            return _repo.Add(dbObject);
        }

        public bool ChangeStatus(Guid id, int status = 1)
        {
            return _repo.ChangeStatus(id, status);
        }

    }
}
