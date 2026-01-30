using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskMateApp.Domain.DomainModels;

namespace DeskMateApp.Service.Interface
{
    public interface IOfficeLocationService
    {
        List<OfficeLocation> GetAll();
        OfficeLocation? GetById(Guid id);

        OfficeLocation Insert(OfficeLocation entity);
        OfficeLocation Update(OfficeLocation entity);
        OfficeLocation DeleteById(Guid id);
    }
}
