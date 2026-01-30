using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskMateApp.Domain.DomainModels
{
    public class DeskAmenity : BaseEntity
    {
        public Guid DeskId { get; set; }
        public Desk Desk { get; set; }

        public Guid AmenityId { get; set; }
        public Amenity Amenity { get; set; }
    }
}
