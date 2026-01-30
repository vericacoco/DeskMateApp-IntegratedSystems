using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskMateApp.Domain.DomainModels;
using DeskMateApp.Domain.DTOs;

namespace DeskMateApp.Service.Interface
{

    public interface IDeskService
    {

        List<Desk> GetAll();
        Desk? GetById(Guid id);
        Desk Insert(Desk entity);
        ICollection<Desk> InsertMany(ICollection<Desk> entities);
        Desk Update(Desk entity);
        Desk DeleteById(Guid id);

        


        List<DeskAvilabilityDto> GetAvailability(Guid locationId, DateTime date);
    }

}
