using MyChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.ViewModels
{
    public class ChatRoomViewModel
    {
        public IEnumerable<Message> Messages { get; set; }

        public User User { get; set; }
    }
}
