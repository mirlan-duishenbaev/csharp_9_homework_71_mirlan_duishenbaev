using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.Services
{
    public class UserService
    {
        private ChatContext db;
        private IMemoryCache cache;
        public UserService(ChatContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await db.ChatUsers.ToListAsync();
        }

        public async Task AddUser(User user)
        {
            db.ChatUsers.Add(user);
            int n = await db.SaveChangesAsync();
            if (n > 0)
            {
                cache.Set(user.Id, user, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
        }

        public async Task<User> GetUser(string id)
        {
            User user = null;
            if (!cache.TryGetValue(id, out user))
            {
                user = await db.ChatUsers.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                {
                    cache.Set(user.Id, user,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return user;
        }


    }
}
