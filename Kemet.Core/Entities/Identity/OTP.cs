using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.Identity
{
    public class OTP : BaseEntity
    {
        public string OTPValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime ExpirationTime => CreatedAt.AddMinutes(30);
        public string UserId { get; set; }
    }
}
