﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CookieAuthentication.Models
{
    public class AppUser : BaseEntity
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }    
        public string? Role { get; set; }
    }
}
