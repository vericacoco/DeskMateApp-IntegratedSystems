using DeskMateApp.Domain.DomainModels;
using DeskMateApp.Domain.DTO;
using DeskMateApp.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace DeskMateApp.Service.Implementation;

public class AmenityService : IAmenityService
{
    private readonly IRepository<Amenity> _amenityRepository;
    private readonly IRepository<Desk> _deskRepository;
    private readonly IRepository<DeskAmenity> _deskAmenityRepository;

    public AmenityService(
        IRepository<Amenity> amenityRepository,
        IRepository<Desk> deskRepository,
        IRepository<DeskAmenity> deskAmenityRepository)
    {
        _amenityRepository = amenityRepository;
        _deskRepository = deskRepository;
        _deskAmenityRepository = deskAmenityRepository;
    }
    public void Assign(AmenityAssignDto dto)
    {
        if (dto.DeskId == Guid.Empty) throw new Exception("DeskId is required.");
        if (dto.AmenityId == Guid.Empty) throw new Exception("AmenityId is required.");

        var deskExists = _deskRepository.Get(d => d.Id, predicate: d => d.Id == dto.DeskId) != Guid.Empty;
        if (!deskExists) throw new Exception("Desk not found.");

        var amenityExists = _amenityRepository.Get(a => a.Id, predicate: a => a.Id == dto.AmenityId) != Guid.Empty;
        if (!amenityExists) throw new Exception("Amenity not found.");


        var existingLinkId = _deskAmenityRepository.Get(x => x.Id,
            predicate: x => x.DeskId == dto.DeskId && x.AmenityId == dto.AmenityId);

        if (existingLinkId != Guid.Empty) return;

        _deskAmenityRepository.Insert(new DeskAmenity
        {
            Id = Guid.NewGuid(),
            DeskId = dto.DeskId,
            AmenityId = dto.AmenityId
        });
    }

    public Amenity DeleteById(Guid id)
    {
        var existing = GetById(id);
        if (existing == null) throw new Exception("Amenity not found.");


        var links = _deskAmenityRepository.GetAll(x => x, predicate: x => x.AmenityId == id).ToList();
        foreach (var link in links)
            _deskAmenityRepository.Delete(link);

        return _amenityRepository.Delete(existing);
    }

    public List<Amenity> GetAll()
    {
        return _amenityRepository.GetAll(a => a).ToList();
    }

    public Amenity? GetById(Guid id)
    {
        return _amenityRepository.Get(a => a, predicate: a => a.Id == id);
    }

    public Amenity Insert(Amenity entity)
    {
        if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
        if (string.IsNullOrWhiteSpace(entity.Name)) throw new Exception("Amenity name is required.");

        //we are not allowing to add duplicate amenities by name
        var exists = _amenityRepository.Get(a => a.Id, predicate: a => a.Name == entity.Name.Trim());
        if (exists != Guid.Empty) throw new Exception("Amenity with this name already exists.");

        entity.Name = entity.Name.Trim();
        return _amenityRepository.Insert(entity);
    }

    public void Unassign(AmenityAssignDto dto)
    {
        if (dto.DeskId == Guid.Empty) throw new Exception("DeskId is required.");
        if (dto.AmenityId == Guid.Empty) throw new Exception("AmenityId is required.");

        var link = _deskAmenityRepository.Get(x => x,
            predicate: x => x.DeskId == dto.DeskId && x.AmenityId == dto.AmenityId);

        if (link == null) return;

        _deskAmenityRepository.Delete(link);
    }

    public Amenity Update(Amenity entity)
    {
        if (entity.Id == Guid.Empty) throw new Exception("Id is required.");
        if (string.IsNullOrWhiteSpace(entity.Name)) throw new Exception("Amenity name is required.");

        var existing = GetById(entity.Id);
        if (existing == null) throw new Exception("Amenity not found.");

        existing.Name = entity.Name.Trim();
        return _amenityRepository.Update(existing);
    }
}