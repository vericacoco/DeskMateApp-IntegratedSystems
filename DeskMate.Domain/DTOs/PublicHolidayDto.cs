using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskMateApp.Domain.DTOs
{
    public class PublicHolidayDto
    {
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;     // "New Year (Нова Година)"
        public string LocalName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string MonthLabel { get; set; } = string.Empty; // "January 2026"
        public bool IsWeekend { get; set; }
        public bool IsUpcoming { get; set; }
    }
}
