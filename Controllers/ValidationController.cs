using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.Controllers
{
    public class ValidationController : Controller
    {
        private ChatContext _db;

        public ValidationController(ChatContext db)
        {
            _db = db;
        }

        [AcceptVerbs("GET", "POST")]
        public bool CheckAge(DateTime DateOfBirth)
        {
            DateTime minAge = DateTime.Now.AddYears(-18);

            if (DateOfBirth > minAge)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
