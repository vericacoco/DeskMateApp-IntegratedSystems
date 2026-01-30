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
using DeskMateApp.Domain.DTO;

namespace DeskMateApp.Web.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AmenitiesController : Controller
    {
        private readonly IAmenityService _amenityService;
        private readonly IDeskService _deskService;

        public AmenitiesController(IAmenityService amenityService, IDeskService deskService)
        {
            _amenityService = amenityService;
            _deskService = deskService;
        }

        // GET: Amenities
        public IActionResult Index()
        {
            var amenities = _amenityService.GetAll();
            return View(amenities);
        }

        // GET: Amenities/Details/5
        public IActionResult Details(Guid id)
        {
            var amenity = _amenityService.GetById(id);
            if (amenity == null) return NotFound();

            return View(amenity);
        }

        // GET: Amenities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Amenities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Amenity amenity)
        {
            try
            {
                if (!ModelState.IsValid) return View(amenity);

                _amenityService.Insert(amenity);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(amenity);
            }
        }

        // GET: Amenities/Edit/5
        public IActionResult Edit(Guid id)
        {
            var amenity = _amenityService.GetById(id);
            if (amenity == null) return NotFound();

            return View(amenity);
        }

        // POST: Amenities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Amenity amenity)
        {
            if (id != amenity.Id) return BadRequest();

            try
            {
                if (!ModelState.IsValid) return View(amenity);

                _amenityService.Update(amenity);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(amenity);
            }
        }

        // GET: Amenities/Delete/5
        public IActionResult Delete(Guid id)
        {
            var amenity = _amenityService.GetById(id);
            if (amenity == null) return NotFound();

            return View(amenity);
        }

        // POST: Amenities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                _amenityService.DeleteById(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // прикажи ја истата Delete страна со error
                var amenity = _amenityService.GetById(id);
                if (amenity == null) return NotFound();

                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete", amenity);
            }
        }


        // GET: Amenities/Assign
        public IActionResult Assign()
        {
            PopulateDropdowns();
            return View(new AmenityAssignDto());
        }


        // POST: Amenities/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(AmenityAssignDto dto)
        {
            try
            {
                _amenityService.Assign(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                PopulateDropdowns(dto.DeskId, dto.AmenityId);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }


        // POST: Amenities/Unassign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Unassign(AmenityAssignDto dto)
        {
            try
            {
                _amenityService.Unassign(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


        private void PopulateDropdowns(Guid? selectedDeskId = null, Guid? selectedAmenityId = null)
        {
            var desks = _deskService.GetAll();          // List<Desk> result
            var amenities = _amenityService.GetAll();   // List<Amenity>

            ViewBag.Desks = new SelectList(desks, "Id", "Code", selectedDeskId);
            ViewBag.Amenities = new SelectList(amenities, "Id", "Name", selectedAmenityId);
        }

    }
}
