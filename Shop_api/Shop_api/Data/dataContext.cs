using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop_api.Models;

namespace Shop_api.Data
{
    public class dataContext : IdentityDbContext<User>
    {
        public dataContext(DbContextOptions<dataContext> options) : base(options) { }
    }


}
