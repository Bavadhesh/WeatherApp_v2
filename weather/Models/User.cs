using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace weather.Models
{
    public class User
    {
        [Key]
        public string UserId { get; set; } 
        public string First_name {get;set;}

        public string Last_name{get;set;}
        
        public string Mobile_num{get;set;}
       
        public string Email_id {get;set;}

        public string Password {get;set;}
    }
}