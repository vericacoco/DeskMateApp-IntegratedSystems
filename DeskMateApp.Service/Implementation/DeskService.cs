using DeskMateApp.Domain.DomainModels;
using DeskMateApp.Domain.DTOs;
using DeskMateApp.Domain.Enums;
using DeskMateApp.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace DeskMateApp.Service.Implementation;

public class DeskService : IDeskService
{
    private readonly IRepository<Desk> _deskRepository;
    private readonly IRepository<OfficeLocation> _locationRepository;

    public DeskService(IRepository<Desk> deskRepository, IRepository<OfficeLocation> locationRepository)
    {
        _deskRepository = deskRepository;
        _locationRepository = locationRepository;
    }
    public Desk DeleteById(Guid id)
    {
        var existing = _deskRepository.Get(d => d,
            predicate: d => d.Id == id,
            include: q => q.Include(x => x.Reservations)
        );

        if (existing == null) throw new Exception("Desk not found.");


        var hasActive = existing.Reservations.Any(r => r.Status == ReservationStatus.Active);
        if (hasActive) throw new Exception("Cannot delete desk with active reservations.");

        return _deskRepository.Delete(existing);
    }

    public List<Desk> GetAll()
    {
        return _deskRepository.GetAll(d => d,
           include: q => q
               .Include(x => x.OfficeLocation)
               .Include(x => x.DeskAmenities).ThenInclude(da => da.Amenity)
       ).ToList();
    }

    public List<DeskAvilabilityDto> GetAvailability(Guid locationId, DateTime date)
    {
        if (locationId == Guid.Empty) throw new Exception("LocationId is required.");

        var locExists = _locationRepository.Get(l => l.Id, predicate: l => l.Id == locationId);
        if (locExists == Guid.Empty) throw new Exception("OfficeLocation not found.");

        var day = date.Date;


        var desks = _deskRepository.GetAll(d => d,
            predicate: d => d.OfficeLocationId == locationId && d.IsActive,
            include: q => q
                .Include(x => x.Reservations)
                .Include(x => x.DeskAmenities).ThenInclude(da => da.Amenity)
        ).ToList();

        return desks.Select(d =>
        {
            var busy = d.Reservations.Any(r =>
                r.Status == ReservationStatus.Active &&
                r.DateFrom.Date <= day &&
                r.DateTo.Date >= day
            );

            return new DeskAvilabilityDto
            {
                DeskId = d.Id,
                DeskCode = d.Code,
                IsAvailable = !busy,
                Amenities = d.DeskAmenities.Select(x => x.Amenity.Name).Distinct().ToList()
            };
        }).ToList();
    }

    public Desk? GetById(Guid id)
    {
        return _deskRepository.Get(d => d,
           predicate: d => d.Id == id,
           include: q => q
               .Include(x => x.OfficeLocation)
               .Include(x => x.DeskAmenities).ThenInclude(da => da.Amenity)
       );
    }

    public Desk Insert(Desk entity)
    {
        if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
        if (string.IsNullOrWhiteSpace(entity.Code)) throw new Exception("Desk Code is required.");
        if (entity.Floor <= 0) throw new Exception("Floor must be > 0.");
        if (entity.OfficeLocationId == Guid.Empty) throw new Exception("OfficeLocationId is required.");


        var locExists = _locationRepository.Get(l => l.Id, predicate: l => l.Id == entity.OfficeLocationId);
        if (locExists == Guid.Empty) throw new Exception("OfficeLocation not found.");

        entity.Code = entity.Code.Trim();

        return _deskRepository.Insert(entity);
    }

    public ICollection<Desk> InsertMany(ICollection<Desk> entities)
    {
        foreach (var d in entities)
        {
            if (d.Id == Guid.Empty) d.Id = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(d.Code)) throw new Exception("Desk Code is required.");
            if (d.Floor <= 0) throw new Exception("Floor must be > 0.");
            if (d.OfficeLocationId == Guid.Empty) throw new Exception("OfficeLocationId is required.");
            d.Code = d.Code.Trim();
        }

        return _deskRepository.InsertMany(entities);
    }

    public Desk Update(Desk entity)
    {
        if (entity.Id == Guid.Empty) throw new Exception("Id is required.");

        var existing = _deskRepository.Get(d => d, predicate: d => d.Id == entity.Id);
        if (existing == null) throw new Exception("Desk not found.");

        if (string.IsNullOrWhiteSpace(entity.Code)) throw new Exception("Desk Code is required.");
        if (entity.Floor <= 0) throw new Exception("Floor must be > 0.");
        if (entity.OfficeLocationId == Guid.Empty) throw new Exception("OfficeLocationId is required.");


        var locExists = _locationRepository.Get(l => l.Id, predicate: l => l.Id == entity.OfficeLocationId);
        if (locExists == Guid.Empty) throw new Exception("OfficeLocation not found.");

        existing.Code = entity.Code.Trim();
        existing.Floor = entity.Floor;
        existing.Type = entity.Type;
        existing.IsActive = entity.IsActive;
        existing.OfficeLocationId = entity.OfficeLocationId;

        return _deskRepository.Update(existing);
    }

}

