using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace weather.Models
{
   
    public class AppDbContext:DbContext
    {
        public DbSet<User> User {get;set;}
       public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
       {
        
       } 
    }
}