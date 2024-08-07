﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulochka.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public string PaymentOption { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CompanyBranchList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> PaymentOptionList { get; set; }
    }
}
