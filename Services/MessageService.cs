using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.Services
{
    public class MessageService
    {
        private ChatContext db;
        private IMemoryCache cache;
        public MessageService(ChatContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public async Task<IEnumerable<Message>> GetMessage()
        {
            return await db.Messages.ToListAsync();
        }

        public async Task AddMessage(Message message)
        {
            db.Messages.Add(message);
            int n = await db.SaveChangesAsync();
            if (n > 0)
            {
                cache.Set(message.Id, message, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
        }

        public async Task<Message> GetMessage(int id)
        {
            Message message = null;
            if (!cache.TryGetValue(id, out message))
            {
                message = await db.Messages.FirstOrDefaultAsync(p => p.Id == id);
                if (message != null)
                {
                    cache.Set(message.Id, message,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return message;
        }

    }
}
