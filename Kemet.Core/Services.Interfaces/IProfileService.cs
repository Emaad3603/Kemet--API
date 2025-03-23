using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.ModelView;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Kemet.Core.Services.Interfaces
{
    public interface IProfileService
    {

        Task<(bool IsSuccess, string Message, string ImageUrl)> UploadProfileImageAsync(string userEmail, IFormFile profileImage , IFormFile backgroundImage);

        Task<AdventureDTO> GetAdventureModeSuggest(AppUser? user);

    }
}
