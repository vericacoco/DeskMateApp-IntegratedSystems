using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskMateApp.Domain.DomainModels;
using DeskMateApp.Service.Interface;
using Microsoft.EntityFrameworkCore;

using DeskMateApp.Domain.DTO;
using DeskMateApp.Domain.Enums;

public class ReservationService : IReservationService
{

    private readonly IRepository<Reservation> _reservationRepository;
    private readonly IRepository<Desk> _deskRepository;

    public ReservationService(IRepository<Reservation> reservationRepository, IRepository<Desk> deskRepository)
    {
        _reservationRepository = reservationRepository;
        _deskRepository = deskRepository;
    }

    public void Cancel(Guid reservationId, string userId)
    {
        var r = _reservationRepository.Get(x => x, predicate: x => x.Id == reservationId);
        if (r == null) throw new Exception("Reservation not found.");

        //only the user that created the reservation can cancel it
        if (r.UserId != userId) throw new Exception("Not allowed.");

        if (r.Status != ReservationStatus.Active) return;

        r.Status = ReservationStatus.Canceled;
        _reservationRepository.Update(r);
    }

    public Guid Create(CreateReservationDto dto, string userId)
    {
        if (dto.DeskId == Guid.Empty) throw new Exception("DeskId is required.");
        if (string.IsNullOrWhiteSpace(userId)) throw new Exception("UserId is required.");

        var from = dto.DateFrom.Date;
        var to = dto.DateTo.Date;

        if (from > to) throw new Exception("DateFrom cannot be after DateTo.");


        //check if the desk actually exists and then check if it is active
        var desk = _deskRepository.Get(d => d, predicate: d => d.Id == dto.DeskId);
        if (desk == null) throw new Exception("Desk not found.");
        if (!desk.IsActive) throw new Exception("Desk is not active.");


        //check if the reservation for the desk overlaps with other desk (from -> to interval)
        var overlapId = _reservationRepository.Get(r => r.Id,
            predicate: r =>
                r.DeskId == dto.DeskId &&
                r.Status == ReservationStatus.Active &&
                r.DateFrom.Date <= to &&
                r.DateTo.Date >= from
        );

        if (overlapId != Guid.Empty)
            throw new Exception("Desk is already reserved for the selected dates.");

        var entity = new Reservation
        {
            Id = Guid.NewGuid(),
            DeskId = dto.DeskId,
            UserId = userId,
            DateFrom = from,
            DateTo = to,
            Status = ReservationStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _reservationRepository.Insert(entity);
        return entity.Id;
    }

    public Reservation DeleteById(Guid id)
    {
        var existing = _reservationRepository.Get(r => r, predicate: r => r.Id == id);
        if (existing == null) throw new Exception("Reservation not found.");
        return _reservationRepository.Delete(existing);
    }

    public int ExpireOldReservations(DateTime today)
    {
        var day = today.Date;

        var toExpire = _reservationRepository.GetAll(r => r,
            predicate: r => r.Status == ReservationStatus.Active && r.DateTo.Date < day
        ).ToList();

        foreach (var r in toExpire)
        {
            r.Status = ReservationStatus.Expired;
            _reservationRepository.Update(r);
        }

        return toExpire.Count;
    }

    public List<Reservation> GetAll()
    {
        return _reservationRepository.GetAll(r => r,
       orderBy: q => q.OrderByDescending(x => x.CreatedAt),
       include: q => q
           .Include(x => x.Desk).ThenInclude(d => d.OfficeLocation)
           .Include(x => x.User)
     ).ToList();
    }

    public List<Reservation> GetByDeskId(Guid deskId)
    {
        return _reservationRepository.GetAll(r => r,
         predicate: r => r.DeskId == deskId,
         orderBy: q => q.OrderByDescending(x => x.CreatedAt),
         include: q => q
             .Include(x => x.Desk).ThenInclude(d => d.OfficeLocation)
             .Include(x => x.User)
     ).ToList();
    }

    public Reservation? GetById(Guid id)
    {
        return _reservationRepository.Get(r => r,
        predicate: r => r.Id == id,
        include: q => q
            .Include(x => x.Desk).ThenInclude(d => d.OfficeLocation)
            .Include(x => x.User)
    );
    }

    public List<Reservation> GetByUserId(string userId)
    {
        return _reservationRepository.GetAll(r => r,
      predicate: r => r.UserId == userId,
      orderBy: q => q.OrderByDescending(x => x.CreatedAt),
      include: q => q
          .Include(x => x.Desk).ThenInclude(d => d.OfficeLocation)
          .Include(x => x.User)
  ).ToList();
    }

    public Reservation Insert(Reservation entity)
    {
        if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
        if (entity.DeskId == Guid.Empty) throw new Exception("DeskId is required.");
        if (string.IsNullOrWhiteSpace(entity.UserId)) throw new Exception("UserId is required.");


        entity.DateFrom = entity.DateFrom.Date;
        entity.DateTo = entity.DateTo.Date;

        if (entity.DateFrom > entity.DateTo) throw new Exception("Invalid date range.");


        var overlapId = _reservationRepository.Get(r => r.Id,
            predicate: r =>
                r.DeskId == entity.DeskId &&
                r.Status == ReservationStatus.Active &&
                r.DateFrom.Date <= entity.DateTo.Date &&
                r.DateTo.Date >= entity.DateFrom.Date
        );

        if (overlapId != Guid.Empty)
            throw new Exception("Desk is already reserved for the selected dates.");

        entity.Status = ReservationStatus.Active;
        entity.CreatedAt = DateTime.UtcNow;

        return _reservationRepository.Insert(entity);
    }

    public Reservation Update(Reservation entity)
    {
        if (entity.Id == Guid.Empty) throw new Exception("Id is required.");

        var existing = _reservationRepository.Get(r => r, predicate: r => r.Id == entity.Id);
        if (existing == null) throw new Exception("Reservation not found.");

        //if reservation is not active then we cannot update it
        if (existing.Status != ReservationStatus.Active)
            throw new Exception("Only Active reservations can be updated.");


        var from = entity.DateFrom.Date;
        var to = entity.DateTo.Date;
        if (from > to) throw new Exception("Invalid date range.");

        //check if the reservation overlaps with other reservation excluding the current reservation that we are updating
        var overlapId = _reservationRepository.Get(r => r.Id,
            predicate: r =>
                r.Id != existing.Id &&
                r.DeskId == existing.DeskId &&
                r.Status == ReservationStatus.Active &&
                r.DateFrom.Date <= to &&
                r.DateTo.Date >= from
        );

        if (overlapId != Guid.Empty)
            throw new Exception("Desk is already reserved for the selected dates.");

        existing.DateFrom = from;
        existing.DateTo = to;

        return _reservationRepository.Update(existing);
    }
}