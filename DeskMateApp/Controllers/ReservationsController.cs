using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DeskMateApp.Domain.DomainModels;
using DeskMateApp.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using DeskMateApp.Service.Interface;
using Microsoft.AspNetCore.Identity;
using DeskMateApp.Service.Implementation;
using DeskMateApp.Domain.DTO;
using System.Security.Claims;
using Humanizer;

namespace DeskMateApp.Web.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IDeskService _deskService;
        private readonly UserManager<DeskMateApp.Domain.Identity.ApplicationUser> _userManager;
        private readonly IHolidayService _holidayService;
        private readonly IConfiguration _config;

        public ReservationsController(
          IReservationService reservationService,
          IDeskService deskService,
          UserManager<DeskMateApp.Domain.Identity.ApplicationUser> userManager,
          IHolidayService holidayService,
          IConfiguration configuration)
        {
            _reservationService = reservationService;
            _deskService = deskService;
            _userManager = userManager;
            _holidayService = holidayService;
            _config = configuration;
        }



        // GET: Reservations
        public IActionResult Index()
        {
            _reservationService.ExpireOldReservations(DateTime.Today);
            var reservations = _reservationService.GetAll();
            return View(reservations);
        }

        // GET: Reservations/Details/5
        public IActionResult Details(Guid id)
        {
            var reservation = _reservationService.GetById(id);
            if (reservation == null) return NotFound();
            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            PopulateDesksDropdown();

            return View(new CreateReservationDto
            {
                DateFrom = DateTime.Today,
                DateTo = DateTime.Today
            });

            
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateReservationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    PopulateDesksDropdown(dto.DeskId);
                    
                    return View(dto);
                }

                var cc = _config["ExternalApis:CountryCode"] ?? "MK";

                // земи празници за годината на DateFrom/DateTo (ако може да премине година, провери двете)
                var years = new HashSet<int> { dto.DateFrom.Year, dto.DateTo.Year };

                foreach (var y in years)
                {
                    var holidayDates = await _holidayService.GetHolidayDatesAsync(y, cc);

                    // блокирај ако било кој ден во интервал е празник (DateFrom..DateTo)
                    for (var d = dto.DateFrom.Date; d <= dto.DateTo.Date; d = d.AddDays(1))
                    {
                        if (holidayDates.Contains(d))
                        {
                            PopulateDesksDropdown(dto.DeskId);
                            ModelState.AddModelError(string.Empty, $"Cannot reserve on public holiday: {d:dd MMM yyyy}");
                            return View(dto);
                        }
                    }
                }

                var userId = _userManager.GetUserId(User);
                _reservationService.Create(dto, userId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                PopulateDesksDropdown(dto.DeskId);
               
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }


        [HttpGet]
        public IActionResult DeskDetails(Guid id)
        {
            if (id == Guid.Empty) return BadRequest();


            var desk = _deskService.GetById(id);

            if (desk == null) return NotFound();

            return Json(new
            {
                code = desk.Code,
                type = desk.Type.ToString(),
                isActive = desk.IsActive,
                location = desk.OfficeLocation?.Name,
                amenities = desk.DeskAmenities?
                    .Select(da => da.Amenity?.Name)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .ToList() ?? new List<string>()
            });
        }



        // GET: Reservations/Edit/5
        public IActionResult Edit(Guid id)
        {
            var reservation = _reservationService.GetById(id);
            if (reservation == null) return NotFound();

            PopulateDesksDropdown(reservation.DeskId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Reservation reservation)
        {
            if (id != reservation.Id) return BadRequest();


            ModelState.Remove(nameof(Reservation.Desk));
            ModelState.Remove(nameof(Reservation.User));



            try
            {
                if (!ModelState.IsValid)
                {
                    PopulateDesksDropdown(reservation.DeskId);
                    return View(reservation);
                }

                // Не дозволувај да менуваат DeskId/UserId/Status преку форма:
                // ќе ги “заклучиме” да се исти како постоечката
                var existing = _reservationService.GetById(id);
                if (existing == null) return NotFound();

                reservation.DeskId = existing.DeskId;
                reservation.UserId = existing.UserId;
                reservation.Status = existing.Status;
                reservation.CreatedAt = existing.CreatedAt;

                _reservationService.Update(reservation);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                PopulateDesksDropdown(reservation.DeskId);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(reservation);
            }
        }

        // GET: Reservations/Delete/5
        public IActionResult Delete(Guid id)
        {
            var reservation = _reservationService.GetById(id);
            if (reservation == null) return NotFound();
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                _reservationService.DeleteById(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var reservation = _reservationService.GetById(id);
                if (reservation == null) return NotFound();

                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete", reservation);
            }
        }

        private bool ReservationExists(Guid id)
        {
            return _reservationService.GetById(id) != null;
        }

        // Cancel only for current userId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(Guid id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                _reservationService.Cancel(id, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private void PopulateDesksDropdown(Guid? selectedDeskId = null)
        {
            // desk service to get the desks for reservation
            var desks = _deskService.GetAll();

            ViewBag.DeskId = new SelectList(desks, "Id", "Code", selectedDeskId);
        }
    


    
    
    }
    }
