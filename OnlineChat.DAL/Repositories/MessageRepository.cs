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
    public class MessageRepository : BaseRepository<Message>
    {
        public MessageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Message>> GetChatMessagesByIdAsync(string id)
        {
            // получения всех сообщений по "id" чата
            return await DbSet.Include(m => m.User)
                .Include(m => m.Chat)
                .Where(m => m.ChatId == id).ToListAsync();
        }
    }
}
