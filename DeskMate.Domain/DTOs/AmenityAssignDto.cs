using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskMateApp.Domain.DTO
{
    public class AmenityAssignDto
    {
        public Guid DeskId { get; set; }
        public Guid AmenityId { get; set; }
    }
}
