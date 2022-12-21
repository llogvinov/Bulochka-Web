using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulochka.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [DisplayName("Полное имя")]
        public string Name { get; set; }
        [DisplayName("Город")]
        public string? City { get; set; }
        [DisplayName("Адрес")]
        public string? StreetAddress { get; set; }
        [DisplayName("Почтовый индекс")]
        public string? PostalCode { get; set; }

        public int? CompanyBranchId { get; set; }
        [ForeignKey("CompanyBranchId")]
        [ValidateNever]
        public CompanyBranch CompanyBranch { get; set; }
    }
}
