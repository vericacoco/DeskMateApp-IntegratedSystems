using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskMateApp.Domain.DomainModels
{
    public class Amenity : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        // "Monitor", "Docking Station", "Near Window"
        public ICollection<DeskAmenity> DeskAmenities { get; set; } = new List<DeskAmenity>();
    }
}
