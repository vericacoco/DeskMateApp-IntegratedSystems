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

namespace DeskMateApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DesksController : Controller
    {
        private readonly IDeskService _deskService;
        private readonly IOfficeLocationService _officeLocationService;

        public DesksController(IDeskService deskService, IOfficeLocationService officeLocationService)
        {
            _deskService = deskService;
            _officeLocationService = officeLocationService;
        }

        // GET: Desks
        public IActionResult Index()
        {
            var desks = _deskService.GetAll(); 
            return View(desks);
        }

        // GET: Desks/Details/5
        public IActionResult Details(Guid id)
        {
            var desk = _deskService.GetById(id);
            if (desk == null) return NotFound();

            return View(desk);
        }

        // GET: Desks/Create
        public IActionResult Create()
        {
            PopulateLocationsDropdown();
            return View(new Desk { IsActive = true });
        }

        // POST: Desks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Desk desk)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    PopulateLocationsDropdown(desk.OfficeLocationId);
                    return View(desk);
                }

                _deskService.Insert(desk);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                PopulateLocationsDropdown(desk.OfficeLocationId);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(desk);
            }
        }

        // GET: Desks/Edit/5
        public IActionResult Edit(Guid id)
        {
            var desk = _deskService.GetById(id);
            if (desk == null) return NotFound();

            PopulateLocationsDropdown(desk.OfficeLocationId);
            return View(desk);
        }

        // POST: Desks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Desk desk)
        {
            if (id != desk.Id) return BadRequest();

            try
            {
                if (!ModelState.IsValid)
                {
                    PopulateLocationsDropdown(desk.OfficeLocationId);
                    return View(desk);
                }

                _deskService.Update(desk);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                PopulateLocationsDropdown(desk.OfficeLocationId);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(desk);
            }
        }

        // GET: Desks/Delete/5
        public IActionResult Delete(Guid id)
        {
            var desk = _deskService.GetById(id);
            if (desk == null) return NotFound();

            return View(desk);
        }

        // POST: Desks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                _deskService.DeleteById(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var desk = _deskService.GetById(id);
                if (desk == null) return NotFound();

                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete", desk);
            }
        }

        private bool DeskExists(Guid id)
        {
            return _deskService.GetById(id) != null;
        }


        // GET: Desks/Availability
        public IActionResult Availability()
        {
            PopulateLocationsDropdown();
            ViewBag.Date = DateTime.Today.ToString("yyyy-MM-dd");
            return View();
        }


        // POST: Desks/Availability
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Availability(Guid officeLocationId, DateTime date)
        {
            try
            {
                PopulateLocationsDropdown(officeLocationId);
                var result = _deskService.GetAvailability(officeLocationId, date);
                ViewBag.Date = date.ToString("yyyy-MM-dd");
                return View(result);
            }
            catch (Exception ex)
            {
                PopulateLocationsDropdown(officeLocationId);
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Date = date.ToString("yyyy-MM-dd");
                return View();
            }
        }


        private void PopulateLocationsDropdown(Guid? selectedLocationId = null)
        {
            var locations = _officeLocationService.GetAll(); 
            ViewBag.OfficeLocationId = new SelectList(locations, "Id", "City", selectedLocationId);
            
        }
    }
}
