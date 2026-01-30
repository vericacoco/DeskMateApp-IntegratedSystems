using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskMateApp.Domain.Enums;
using DeskMateApp.Domain.Identity;

namespace DeskMateApp.Domain.DomainModels
{
    public class Reservation : BaseEntity
    {
        public Guid DeskId { get; set; }
        public Desk Desk { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;


        [DataType(DataType.Date)]
        public DateTime DateFrom { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateTo { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
