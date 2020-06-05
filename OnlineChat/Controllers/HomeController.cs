using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineChat.BLL.Services;
using OnlineChat.Models.Entities;

namespace OnlineChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MessageService _messageService;

        public HomeController(UserManager<ApplicationUser> userManager, MessageService messageService)
        {
            _userManager = userManager;
            _messageService = messageService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            //Возвращает начальную страницу 
            return View();
        }

        public async Task<JsonResult> GetUser()
        {
            //Метод для получения текущего клиента в JSON формате
            ApplicationUser user = await _userManager.GetUserAsync(User);
            return Json(user);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(Exception exception)
        {
            if (exception != null)
            {
                return View(new ErrorViewModel { Exception = exception });
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<JsonResult> SendMessage(string message, string idChat)
        {
            //Метод отправки сообщения
            // message - Текстовое сообщения
            // idChat - id чата которому отправляется сообщение
            // user - текущий авторизованный  клиент
            try
            {
                var user = await _userManager.GetUserAsync(User);
                await _messageService.SendMessage(user, message, idChat);
                return Json(Ok());
            }
            catch (Exception ex)
            {
                return Json(new { Exception = ex.Message });
            }
        }

        public async Task<JsonResult> GetOnlineUsers()
        {
            // метод для получения онлайн пользователей в виде JSON
            try
            {
                var onlineUsers = await _messageService.GetOnlineUsersAsync();
                return Json( onlineUsers);
            }
            catch (Exception e)
            {
                return Json(new {Exception = e});
            }
        }
        [Authorize]
        public async Task<JsonResult> SearchMessages(string searchParam, string idChat)
        {
            // Метод поиска сообщений в определенном чате
            // searchParam - параметры поиска
            // idChat  - "id" выбранного чата
            // messages - список найденных сообщений
            try
            {
                List<Message> messages = new List<Message>();
                if (searchParam != null)
                {
                    messages = await _messageService.SearchMessages(searchParam, idChat);
                }
                return Json(messages);
            }
            catch (Exception e)
            {
                return Json(new { Exception = e });
            }

        }

        public async Task<JsonResult> SelectedChat(string idChat)
        {
            // метод для получения списка сообщений из выбранного чата
            // idChat - "id" выбранного чата
            // messages - список сообщений 
            try
            {
                var messages = await _messageService.GetChatMessage(idChat);
                return Json(messages);
            }
            catch (Exception e)
            {
                return Json(new {Exception = e});
            }
        }
        public async Task<JsonResult> SelectedUserChat(string userId)
        {
            // метод для получения списка сообщений с выбранным пользователям
            // userId - "id" выбранного пользователя
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var messages = await _messageService.GetUserChatMessages(userId, user);
                return Json( messages);
            }
            catch (Exception e)
            {
                return Json(new {e});
            }
        }
        public async Task<JsonResult> GetListChats()
        {
            //метод для получения списка чата текущего авторизованного пользователя
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var chats = await _messageService.GetListChats(user);
                return Json(chats);
            }
            catch (Exception e)
            {
                return Json(new {e});
            }
        }
    }
}
