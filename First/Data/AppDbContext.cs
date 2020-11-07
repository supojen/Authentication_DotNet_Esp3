using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace First.Data
{
    public class AppDbContext : IdentityDbContext<PlantsistEmployee>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {}
    }
}
