using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManager.Models
{
    public class User //: IEntity
    {
        //public string id { get; set; } // partition key
        //public string type => "user";
        //public bool isDeleted { get; set; }

        [DisplayName("First name")] 
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("Last name")]
        public string LastName { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("Email address")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Created Date")]
        public DateTime? CreatedDate { get; set; }

        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; }
    }
}
