using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskMateApp.Domain.DomainModels;
using DeskMateApp.Domain.DTO;

namespace DeskMateApp.Service.Interface
{
    public interface IReservationService
    {

        Reservation? GetById(Guid id);
        List<Reservation> GetByUserId(string userId);
        List<Reservation> GetByDeskId(Guid deskId);
        List<Reservation> GetAll();


        Guid Create(CreateReservationDto dto, string userId);


        Reservation Insert(Reservation entity);
        Reservation Update(Reservation entity);
        Reservation DeleteById(Guid id);


        void Cancel(Guid reservationId, string userId);
        int ExpireOldReservations(DateTime today);
    }
}
