using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineChat.DAL.UoW;
using OnlineChat.Models.Entities;

namespace OnlineChat.BLL.Services
{
    public class MessageService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<Hub.Hub> _hub;
        public MessageService(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHubContext<Hub.Hub> hub)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _hub = hub;

        }
        public async Task SendMessage(ApplicationUser user, string textMessage, string idChat)
        {
            // метот отправки сообщения
            // user - текущий авторизованный пользователь
            // textMessage - текст сообщения 
            // idChat - "id" чата которому отправляется сообщения 
            try
            {
                Message message = new Message()
                {
                    Messag = textMessage,
                    UserId = user.Id,
                    ChatId = idChat,
                    DateTime = DateTime.Now
                };
                await _unitOfWork.MessageRepository.CreateAsync(message);
                await _unitOfWork.CompleteAsync();
                await _hub.Clients.All.SendAsync("Knockout", idChat, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public async Task<List<Message>> SearchMessages(string searchParam, string idChat)
        {
            // метод поиска сообщения 
            // searchParam - параметры поиска
            // idChat- "id" чата в котором будет поиск 
            try
            {
                var messages = await _unitOfWork.MessageRepository.GetChatMessagesByIdAsync(idChat);
                messages = messages.Where(x => (x.Messag?.ToUpper() ?? String.Empty).Contains(searchParam.ToUpper()))
                    .ToList();
                return messages;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task<List<ApplicationUser>> GetOnlineUsersAsync()
        {
            // Метод получения онлайн пользователей
            try
            {
                var onlineUsers = Hub.Hub.OnlineUsers;
                List<ApplicationUser> users = new List<ApplicationUser>();
                foreach (var user in onlineUsers)
                {
                    users.Add(await _userManager.GetUserAsync(user));
                }

                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<List<Message>> GetChatMessage(string idChat)
        {
            // метод поучения списка сообщений из выбранного чата 
            // idChat - "id" выбранного чата 
            try
            {
                var messages = await _unitOfWork.MessageRepository.GetChatMessagesByIdAsync(idChat);
                return messages;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<List<Message>> GetUserChatMessages(string idUser, ApplicationUser user)
        {
            // метод для получения списка сообщений с выбранным пользователям
            // userId - "id" выбранного пользователя
            try
            {
                var chat = await GetUserChatAsync(idUser, user);
                var messages = await _unitOfWork.MessageRepository.GetChatMessagesByIdAsync(chat.Id.ToString());
                return messages;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<Chat> GetUserChatAsync(string idUser1, ApplicationUser user2)
        {
            // метод для получения чата с выбранным пользователям 
            // idUser1 - "id" выбранного пользователя
            // user2 - текущий авторизованный пользователь
            var chat = _unitOfWork.ChatRepository.GetChatByUser1IdAndUser2IdAsync(idUser1, user2.Id);
            if (chat.Result == null)
            {
                return await CreateUserChat(idUser1, user2.Id);
            }

            return chat.Result;
        }

        private async Task<string> GetChatName(string idChat, ApplicationUser user)
        {
            // метод для получения имени чата
            // idChat - "id" выбранного чата
            // user - текущий авторизованный пользователь
            try
            {
                var name = "";
                ApplicationUser user2 = new ApplicationUser();
                var chat = await _unitOfWork.ChatRepository.GetChatByIdAsync(idChat);
                if (chat.UserId1 == user.Id)
                {
                    user2 = await _userManager.FindByIdAsync(chat.UserId2);
                }
                else
                {
                    user2 = await _userManager.FindByIdAsync(chat.UserId1);
                }
                
                if (chat.IsGroup)
                {
                    name = chat.Name;
                }
                else
                {
                    
                    if (chat.UserId1 == user.Id && chat.UserId2 == user.Id)
                    {
                        name = "Save";
                    }
                    else if (chat.UserId1 == user.Id)
                    {
                        name = user2.UserName;
                    }
                    else
                    {
                        name = user2.UserName;
                    }
                }
                return name;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<List<Chat>> GetListChats(ApplicationUser user)
        {
            // метод для получения списка чатов у текущего пользователя 
            try
            {
                var chats = await _unitOfWork.ChatRepository.GetAllChatByUserIdAsync(user.Id);
                foreach (var chat in chats)
                {
                    chat.Name = await GetChatName(chat.Id.ToString(), user);
                }
                return chats;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private async Task<Chat> CreateUserChat(string idUser1, string idUser2)
        {
            // метод для созданий чата между двумя пользователями 
            Chat newChat = new Chat()
            {
                DateCreated = DateTime.Now,
                IsGroup = false,
                IsPublic = false,
                UserId1 = idUser1,
                UserId2 = idUser2,
                IsJoin = false
            };
            await _unitOfWork.ChatRepository.CreateAsync(newChat);
            await _unitOfWork.CompleteAsync();
            return newChat;
        }
    }
}