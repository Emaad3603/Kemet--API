using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
       
        public string? ImageURL { get; set; }

    }
}
