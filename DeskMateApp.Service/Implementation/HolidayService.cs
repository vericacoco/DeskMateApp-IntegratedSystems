using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DeskMateApp.Domain.DTOs;
using DeskMateApp.Service.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace DeskMateApp.Service.Implementation
{
    public class HolidayService : IHolidayService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public HolidayService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<List<PublicHolidayDto>> GetPublicHolidaysAsync(int year, string countryCode)
        {
            var key = $"holidays:{year}:{countryCode}".ToLowerInvariant();

            return await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);

                var url = $"PublicHolidays/{year}/{countryCode.Trim().ToUpper()}";
                var result = await _httpClient.GetFromJsonAsync<List<PublicHolidayDto>>(url);
                return result ?? new List<PublicHolidayDto>();
            }) ?? new List<PublicHolidayDto>();
        }

        public async Task<HashSet<DateTime>> GetHolidayDatesAsync(int year, string countryCode)
        {
            var list = await GetPublicHolidaysAsync(year, countryCode);

            //formatting date
            var set = new HashSet<DateTime>();
            foreach (var h in list)
            {
                 set.Add(h.Date.Date);
            }
            return set;
        }
    }
}
