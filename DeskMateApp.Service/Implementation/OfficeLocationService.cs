using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskMateApp.Domain.DomainModels;
using DeskMateApp.Service.Interface;
using Microsoft.EntityFrameworkCore;


namespace DeskMateApp.Service.Implementation
{
    public class OfficeLocationService : IOfficeLocationService
    {
        private readonly IRepository<OfficeLocation> _officeLocationRepository;
        public OfficeLocationService(IRepository<OfficeLocation> officeLocationRepository)
        {
            _officeLocationRepository = officeLocationRepository;
        }

        public OfficeLocation DeleteById(Guid id)
        {
            var location = GetById(id);
            if (location == null) throw new Exception("Can't find location.");
            return _officeLocationRepository.Delete(location);

        }

        public List<OfficeLocation> GetAll()
        {
            return _officeLocationRepository.GetAll(selector: l => l).ToList();

        }

        public OfficeLocation? GetById(Guid id)
        {
            return _officeLocationRepository.Get(selector: l => l, predicate: l => l.Id == id);

        }

        public OfficeLocation Insert(OfficeLocation entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            if (string.IsNullOrWhiteSpace(entity.Name) || string.IsNullOrWhiteSpace(entity.City))
                throw new Exception("Name and City are required.");

            return _officeLocationRepository.Insert(entity);

        }

        public OfficeLocation Update(OfficeLocation entity)
        {
            if (entity.Id == Guid.Empty) throw new Exception("Id is required.");

            var existing = GetById(entity.Id);
            if (existing == null) throw new Exception("Can't find location.");

            existing.Name = entity.Name;
            existing.City = entity.City;
            existing.Address = entity.Address;

            return _officeLocationRepository.Update(entity);
        }
    }
}
