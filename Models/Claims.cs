using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManager.Models
{
    public class Claims
    {
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public string IsValid { get; set; }

    }
}
