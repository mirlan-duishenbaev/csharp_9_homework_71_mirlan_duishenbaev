using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.Models
{
    public class ChatContext : IdentityDbContext<User>
    {
        public DbSet<User> ChatUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FileModel> Files { get; set; }

        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
        }
    }
}
