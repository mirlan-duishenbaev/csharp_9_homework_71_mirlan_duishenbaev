using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан электронный адрес")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан возраст")]
        [Display(Name = "Дата рождения")]
        [Remote(action: "CheckAge", controller: "Validation", ErrorMessage = "Требование по возрасту 18+")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Аватар")]
        public string Avatar { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Не указан пароль")]
        [MinLength(6, ErrorMessage ="Длина пароля должна быть не менее 6 символов")]    
        [RegularExpression(@"^(?=.{6,})(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "Пароль должен содержать в себе 1 букву верхнего регистра, 1 букву нижнего регистра, а также минимум 1 цифру")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        [Required(ErrorMessage = "Не указан пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
    }
}
