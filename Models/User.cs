using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.Models
{
    public class User : IdentityUser
    {
        public string Avatar { get; set; }
        public DateTime BirthDate { get; set; }


    }
}
