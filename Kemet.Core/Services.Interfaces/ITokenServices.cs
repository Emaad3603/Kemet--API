using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kemet.Core.Entities.Identity;

namespace Kemet.Core.Services.InterFaces
{
    public interface ITokenServices
    {

        Task<string> CreateTokenAsync(AppUser user , UserManager<AppUser> _userManager);

        

    }
}
