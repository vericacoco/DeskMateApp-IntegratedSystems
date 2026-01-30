using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskMateApp.Domain.DomainModels;
using DeskMateApp.Domain.DTO;

namespace DeskMateApp.Service.Interface
{
    public interface IAmenityService
    {

        List<Amenity> GetAll();
        Amenity? GetById(Guid id);
        Amenity Insert(Amenity entity);
        Amenity Update(Amenity entity);
        Amenity DeleteById(Guid id);


        void Assign(AmenityAssignDto dto);
        void Unassign(AmenityAssignDto dto);
    }
}
