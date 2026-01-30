using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskMateApp.Domain.DTOs
{
    public class DeskAvilabilityDto
    {
        public Guid DeskId { get; set; }
        public string DeskCode { get; set; }
        public bool IsAvailable { get; set; }
        public List<string> Amenities { get; set; } = new();
    }
}
