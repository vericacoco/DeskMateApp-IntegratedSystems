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
using DeskMateApp.Service.Implementation;

namespace DeskMateApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OfficeLocationsController : Controller
    {
        private readonly IOfficeLocationService _officeLocationService;

        public OfficeLocationsController(IOfficeLocationService officeLocationService)
        {
            _officeLocationService = officeLocationService;
        }


        // GET: OfficeLocations
        public IActionResult Index()
        {
            return View(_officeLocationService.GetAll());
        }

        // GET: OfficeLocations/Details/5
        public IActionResult Details(Guid? id)
        {

            if (id == null) return NotFound();

            var officeLocation = _officeLocationService.GetById(id.Value);
            if (officeLocation == null) return NotFound();

            return View(officeLocation);
        }

        // GET: OfficeLocations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OfficeLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OfficeLocation officeLocation)
        {
            try
            {
                if (!ModelState.IsValid) return View(officeLocation);

                _officeLocationService.Insert(officeLocation);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(officeLocation);
            }
        }

        // GET: OfficeLocations/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var officeLocation = _officeLocationService.GetById(id.Value);
            if (officeLocation == null) return NotFound();

            return View(officeLocation);
        }

        // POST: OfficeLocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, OfficeLocation officeLocation)
        {
            if (id != officeLocation.Id) return NotFound();

            try
            {
                if (!ModelState.IsValid) return View(officeLocation);

                _officeLocationService.Update(officeLocation);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(officeLocation);
            }
        }

        // GET: OfficeLocations/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var officeLocation = _officeLocationService.GetById(id.Value);
            if (officeLocation == null) return NotFound();

            return View(officeLocation);
        }

        // POST: OfficeLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                _officeLocationService.DeleteById(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var officeLocation = _officeLocationService.GetById(id);
                if (officeLocation == null) return NotFound();

                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete", officeLocation);
            }
        }

        private bool OfficeLocationExists(Guid id)
        {
            return _officeLocationService.GetById(id) != null;
        }
    }
}
