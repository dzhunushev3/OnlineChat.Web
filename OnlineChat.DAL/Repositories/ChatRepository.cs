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
    public class ChatRepository : BaseRepository<Chat>
    {
        public ChatRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<Chat> GetChatByIdAsync(string id)
        {
            // получения чата по "id"
            return await DbSet.FirstOrDefaultAsync(g => g.Id.ToString() == id);
        }
        public async Task<List<Chat>> GetAllChatByUserIdAsync(string userId)
        {
            // получения всех чатов по "id" пользователя
            return await DbSet.Where(c => (c.UserId1 == userId) || (c.UserId2 == userId)).ToListAsync();
        }

        public async Task<Chat> GetChatByUser1IdAndUser2IdAsync(string idUser1, string idUser2)
        {
            // получения чата с определенными пользователями
            return await DbSet.FirstOrDefaultAsync(c => (c.UserId1 == idUser1 && c.UserId2 == idUser2) ||
                                                        (c.UserId1 == idUser2 && c.UserId2 == idUser1));
        }
    }
}
