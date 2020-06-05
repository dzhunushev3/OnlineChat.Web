using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineChat.DAL.Data;
using OnlineChat.Models.Entities;

namespace OnlineChat.DAL.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected DbSet<T> DbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            DbSet = _context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return DbSet.ToList();
        }

        public async Task CreateAsync(T entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual Task<List<T>> GetAllAsync()
        {
            return DbSet.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            return await DbSet.FindAsync(id);
        }

        public void RemoveAsync(T entity)
        {
            DbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
        }
    }
}
