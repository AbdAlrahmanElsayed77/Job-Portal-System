using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.Contracts;
using Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public List<DTO> GetAll(params Expression<Func<T, object>>[]? includes)
        {
            var list = _repo.GetAll(includes);
            return _mapper.Map<List<T>, List<DTO>>(list);
        }

        public DTO GetById(Guid id, params Expression<Func<T, object>>[]? includes)
        {
            var obj = _repo.GetById(id, includes);
            return _mapper.Map<T, DTO>(obj);
        }

        public bool Add(DTO entity)
        {
            var dbObject = _mapper.Map<DTO, T>(entity);
            dbObject.Id = Guid.NewGuid();
            return _repo.Add(dbObject);
        }

        public bool Update(DTO entity, Guid userId)
        {
            var dbObject = _mapper.Map<DTO, T>(entity);
            dbObject.UpdatedBy = userId;
            return _repo.Update(dbObject);
        }

        public bool ChangeStatus(Guid id, int status = 1)
        {
            return _repo.ChangeStatus(id, status);
        }

    }
}
