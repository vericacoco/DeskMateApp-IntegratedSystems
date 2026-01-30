using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeskMateApp.Service.Interface;
using DeskMateApp.Domain.DTOs; // <-- HolidayVm ти е тука

namespace DeskMateApp.Web.Controllers
{
    [Authorize]
    public class HolidaysController : Controller
    {
        private readonly IHolidayService _holidayService;
        private readonly IConfiguration _config;

        public HolidaysController(IHolidayService holidayService, IConfiguration config)
        {
            _holidayService = holidayService;
            _config = config;
        }

        public async Task<IActionResult> Index(int? year)
        {
            var y = year ?? DateTime.Today.Year;
            var cc = _config["ExternalApis:CountryCode"] ?? "MK";

            var raw = await _holidayService.GetPublicHolidaysAsync(y, cc);

            var today = DateTime.Today;

            var vms = raw
                .Select(h =>
                {
                    var dt = h.Date.Date; 

                    return new PublicHolidayDto
                    {
                        Date = dt,
                        LocalName = h.LocalName,
                        Name = h.Name,
                        Title = $"{h.Name} ({h.LocalName})",
                        MonthLabel = dt.ToString("MMMM yyyy"),
                        IsWeekend = dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday,
                        IsUpcoming = dt >= today
                    };
                })
                .OrderBy(x => x.Date)
                .ToList();

            ViewBag.NextHoliday = vms.FirstOrDefault(x => x.Date >= today);

            ViewBag.Year = y;
            ViewBag.CountryCode = cc;

            return View(vms);
        }
    }
}
