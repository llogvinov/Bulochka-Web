using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulochka.Models.ViewModels
{
    public class OrderVM
    {
        public Dictionary<OrderHeader, string> Orders { get; set; }
    }
}
