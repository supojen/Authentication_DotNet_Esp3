using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace First.Data
{
    public class PlantsistEmployee : IdentityUser
    {
        public string Department { get; set; }
        public int Level { get; set; }  
    }
}
