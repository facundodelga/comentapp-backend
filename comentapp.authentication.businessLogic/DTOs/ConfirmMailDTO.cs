using comentapp.persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.authentication.businessLogic.DTOs
{
    public class ConfirmMailDTO
    {
        public User User { get; set; } = new User();
        public string Token { get; set; } = string.Empty;
    }
}
