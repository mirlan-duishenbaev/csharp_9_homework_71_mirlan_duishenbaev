using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime MessageDate { get; set; }

        public string UserId { get; set; }
        public User Author { get; set; }

    }
}
