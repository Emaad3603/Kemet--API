using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities
{
    public class Email
    {
        public string Subject { get; set; }

        public string Recipients { get; set; }

        public string Body { get; set; }
    }
}
