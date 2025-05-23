﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.Identity
{
    public class Admin :AppUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }

        public string SSN { get; set; }

        public string Gender { get; set; }

        public string Nationality { get; set; }
    }
}
