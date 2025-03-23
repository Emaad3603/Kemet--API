
using Kemet.Core.Entities.Identity;
using Kemet.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Services.Interfaces
{
    public interface ISearchInterface
    {
        Task<RagionalSearchDTO> SearchAll ( string textA , UserManager<AppUser> userManager);
    }
}
