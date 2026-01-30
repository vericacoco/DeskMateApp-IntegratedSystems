using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskMateApp.Domain.Enums;

namespace DeskMateApp.Domain.DomainModels
{
    public class Desk : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Code { get; set; } = string.Empty;

        [Range(1, 20)]
        public int Floor { get; set; }
        public DeskType Type { get; set; }
        public bool IsActive { get; set; } = true;

        [Required]
        public Guid OfficeLocationId { get; set; }
        public OfficeLocation? OfficeLocation { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<DeskAmenity> DeskAmenities { get; set; } = new List<DeskAmenity>();
    }
}