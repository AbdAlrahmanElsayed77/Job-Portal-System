using DAL.Contracts;
using DAL.DbContext;
using DAL.Exceptions;
using Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class TableRepository<T> : ITableRepository<T> where T : BaseTable
    {
        private readonly PortalContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<TableRepository<T>> _logger;

        public TableRepository(PortalContext context, ILogger<TableRepository<T>> log)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _logger = log;
        }

        public List<T> GetAll(params Expression<Func<T, object>>[]? includes)
        {
            try
            {
                var query = _dbSet.Where(d => d.CurrentState == 0 || d.CurrentState == 1);

                if (includes != null && includes.Length > 0)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                return query.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll for {Type}", typeof(T).Name);
                throw new DataAccessException(ex, $"Error retrieving {typeof(T).Name} records", _logger);
            }
        }

        public T GetById(Guid id, params Expression<Func<T, object>>[]? includes)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (includes != null && includes.Length > 0)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                // ✅ Filter by CurrentState to get only active/valid records
                return query.FirstOrDefault(n => n.Id == id && (n.CurrentState == 0 || n.CurrentState == 1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetById for {Type} with ID {Id}", typeof(T).Name, id);
                throw new DataAccessException(ex, $"Error retrieving {typeof(T).Name} with ID {id}", _logger);
            }
        }

        public bool Add(T entity)
        {
            try
            {
                if (entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();

                entity.CreatedDate = DateTime.UtcNow;
                entity.CurrentState = entity.CurrentState == 0 ? 0 : 1; // Default to 1 if not set

                _dbSet.Add(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Add for {Type}", typeof(T).Name);
                throw new DataAccessException(ex, $"Error adding {typeof(T).Name}", _logger);
            }
        }

        public bool Update(T entity)
        {
            try
            {
                var dbData = GetById(entity.Id);
                if (dbData == null)
                    throw new Exception($"{typeof(T).Name} with ID {entity.Id} not found");

                // ✅ Detach the existing tracked entity
                _context.Entry(dbData).State = EntityState.Detached;

                // ✅ Preserve original creation data
                entity.CreatedDate = dbData.CreatedDate;
                entity.CreatedBy = dbData.CreatedBy;
                entity.UpdatedDate = DateTime.UtcNow;

                // ✅ Attach and mark as modified
                _context.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Update for {Type} with ID {Id}", typeof(T).Name, entity.Id);
                throw new DataAccessException(ex, $"Error updating {typeof(T).Name}", _logger);
            }
        }

        public bool Delete(Guid id)
        {
            try
            {
                var entity = GetById(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Delete for {Type} with ID {Id}", typeof(T).Name, id);
                throw new DataAccessException(ex, $"Error deleting {typeof(T).Name}", _logger);
            }
        }

        public bool ChangeStatus(Guid id, int status = 1)
        {
            try
            {
                var entity = GetById(id);
                if (entity != null)
                {
                    entity.CurrentState = status;
                    entity.UpdatedDate = DateTime.UtcNow;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChangeStatus for {Type} with ID {Id}", typeof(T).Name, id);
                throw new DataAccessException(ex, $"Error changing status for {typeof(T).Name}", _logger);
            }
        }
    }
}