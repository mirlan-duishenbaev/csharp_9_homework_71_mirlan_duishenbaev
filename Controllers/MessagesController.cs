using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyChat.Models;
using MyChat.Services;
using MyChat.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyChat.Controllers
{
    public class MessagesController : Controller
    {
        private ChatContext _db;
        private IMemoryCache cache;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        IWebHostEnvironment _appEnvironment;

        public MessagesController(ChatContext db,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IWebHostEnvironment appEnvironment,
            IMemoryCache memoryCache)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _appEnvironment = appEnvironment;
            cache = memoryCache;
        }

        public IActionResult ChatRoom()
        {
            List<Message> messages = _db.Messages.Include(x => x.Author).ToList();


            var currentUser = GetCurrentUser().Result;

            ChatRoomViewModel crvm = new ChatRoomViewModel
            {
                Messages = messages,
                User = currentUser
            };

            return View(crvm);
        }


        [HttpPost]
        public async Task<JsonResult> Add(string msgText)
        {
            User currentUser = GetCurrentUser().Result;

            Message message = new Message
            {
                Text = msgText,
                MessageDate = DateTime.Now,
                Author = currentUser,
            };

            var result = _db.Messages.AddAsync(message);

            if (result.IsCompleted)
            {
                
                int n = await _db.SaveChangesAsync();

                if(n > 0)
                {
                    cache.Set(message.Id, message, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }

            return Json(new { message });
        }


        private async Task<User> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }


        public async Task<IActionResult> ShowCachedMessages()
        {
            var userId = GetCurrentUser().Result.Id;

            List<Message> messages = _db.Messages.Where(x => x.Author.Id == userId).ToList();

            MessageService msgService = new MessageService(_db, cache);

            int count = messages.Count;

            Message lastMsg = await msgService.GetMessage(messages[count - 1].Id);


            if (lastMsg != null)

                return Content($"User: {lastMsg.Author.UserName} ; Message: {lastMsg.Text}");
            return Content("User not found");

        }
    }
}
