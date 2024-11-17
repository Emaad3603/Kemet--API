using Kemet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Services.Interfaces
{
    public interface IEmailSettings
    {
        public Task SendEmailAsync(Email email);
    }
}
