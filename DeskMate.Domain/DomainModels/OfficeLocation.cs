using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskMateApp.Domain.DomainModels
{
    public class OfficeLocation : BaseEntity
    {
        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string City { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Address { get; set; }

        public ICollection<Desk> Desks { get; set; } = new List<Desk>();
    }
}
