using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OnlineChat.DAL.Data;
using OnlineChat.DAL.Repositories;

namespace OnlineChat.DAL.UoW
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public readonly MessageRepository MessageRepository;
        public readonly ChatRepository ChatRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            MessageRepository = new MessageRepository(context);
            ChatRepository = new ChatRepository(context);
        }


        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

    }
}
