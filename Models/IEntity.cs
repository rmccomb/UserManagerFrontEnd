using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManager.Models
{
    public interface IEntity
    {
        string id { get; set; }
        static string type { get; }
        bool isDeleted { get; set; }
    }
}
