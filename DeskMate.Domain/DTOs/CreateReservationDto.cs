using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DeskMateApp.Domain.DTO
{
    public class CreateReservationDto
    {
        public Guid DeskId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

      
    }
}
