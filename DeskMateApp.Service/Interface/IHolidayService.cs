using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskMateApp.Domain.DTOs;

namespace DeskMateApp.Service.Interface
{
    public interface IHolidayService
    {
        Task<List<PublicHolidayDto>> GetPublicHolidaysAsync(int year, string countryCode);
        Task<HashSet<DateTime>> GetHolidayDatesAsync(int year, string countryCode);
    }
}
