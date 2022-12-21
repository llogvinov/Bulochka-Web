using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulochka.Models
{
    public class CompanyBranch
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string PostalCode { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
