using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.ViewModels
{
    public class EditViewModel
    {
        [Display(Name = "Логин")]
        public string Login { get; set; }


        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Дата рождения")]
        [Remote(action: "CheckAge", controller: "Validation", ErrorMessage = "Требование по возрасту 18+")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Аватар")]
        public string Avatar { get; set; }
    }
}
