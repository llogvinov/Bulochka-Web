using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bulochka.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Название")]
        public string Title { get; set; }
        [DisplayName("Описание")]
        public string Description { get; set; }

        [Required]
        [Range(1, 10000)]
        [DisplayName("Цена")]
        public double Price { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }
    }
}
