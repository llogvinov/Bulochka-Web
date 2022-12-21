using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulochka.Models
{
    public class CompanyBranch
    {
        public int Id { get; set; }
        [DisplayName("Город")]
        public string City { get; set; }
        [DisplayName("Адрес")]
        public string StreetAddress { get; set; }
        [DisplayName("Почтовый индекс")]
        public string PostalCode { get; set; }
        [DisplayName("Номер телефона")]
        public string? PhoneNumber { get; set; }
    }
}
