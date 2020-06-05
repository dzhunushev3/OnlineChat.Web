using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OnlineChat.Models.Entities;

namespace OnlineChat.DAL.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task CreateAsync(T entity);
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        void Update(T entity);
        void RemoveAsync(T entity);
    }
}
