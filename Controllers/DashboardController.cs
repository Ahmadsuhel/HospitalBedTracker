using HospitalBedTracker.Application.DTOs;
using HospitalBedTracker.Domain.Enums;
using HospitalBedTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace HospitalBedTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IDistributedCache _cache;

        public DashboardController(
            AppDbContext db,
            IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }

        // City ka full bed status — public, no auth needed
        [HttpGet("city/{city}")]
        public async Task<IActionResult> GetCityDashboard(string city)
        {
            // Redis cache check — 30 second TTL
            var cacheKey = $"city:{city.ToLower()}:dashboard";
            var cached = await _cache.GetStringAsync(cacheKey);

            if (cached != null)
            {
                var cachedData = JsonSerializer
                    .Deserialize<CityDashboardDto>(cached);
                return Ok(cachedData);
            }

            // DB se fetch
            var hospitals = await _db.Hospitals
                .Where(h => h.City.ToLower() == city.ToLower()
                         && h.IsActive)
                .Include(h => h.BedAvailabilities)
                .OrderBy(h => h.Name)
                .ToListAsync();

            var hospitalDtos = hospitals.Select(h =>
            {
                var beds = h.BedAvailabilities
                    .Select(b => new BedTypeDto
                    {
                        BedType = b.BedType.ToString(),
                        Total = b.TotalBeds,
                        Available = b.AvailableBeds,
                        OccupancyPercent = b.OccupancyPercent
                    }).ToList();

                // Overall status
                var icuBed = h.BedAvailabilities
                    .FirstOrDefault(b => b.BedType == BedType.ICU);

                var status = icuBed switch
                {
                    null => "Unknown",
                    var b when b.AvailableBeds == 0 => "Full",
                    var b when b.OccupancyPercent >= 90 => "Critical",
                    _ => "Available"
                };

                return new HospitalBedDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Address = h.Address,
                    Latitude = h.Latitude,
                    Longitude = h.Longitude,
                    Phone = h.Phone,
                    Beds = beds,
                    OverallStatus = status,
                    LastUpdated = h.BedAvailabilities.Any()
                        ? h.BedAvailabilities.Max(b => b.UpdatedAt)
                        : DateTime.MinValue
                };
            }).ToList();

            var dashboard = new CityDashboardDto
            {
                City = city,
                LastUpdated = DateTime.UtcNow,
                Totals = new BedTotalsDto
                {
                    TotalICU = hospitals.Sum(h => h.BedAvailabilities
                        .Where(b => b.BedType == BedType.ICU)
                        .Sum(b => b.TotalBeds)),
                    AvailableICU = hospitals.Sum(h => h.BedAvailabilities
                        .Where(b => b.BedType == BedType.ICU)
                        .Sum(b => b.AvailableBeds)),
                    TotalGeneral = hospitals.Sum(h => h.BedAvailabilities
                        .Where(b => b.BedType == BedType.General)
                        .Sum(b => b.TotalBeds)),
                    AvailableGeneral = hospitals.Sum(h => h.BedAvailabilities
                        .Where(b => b.BedType == BedType.General)
                        .Sum(b => b.AvailableBeds)),
                    TotalOxygen = hospitals.Sum(h => h.BedAvailabilities
                        .Where(b => b.BedType == BedType.OxygenSupported)
                        .Sum(b => b.TotalBeds)),
                    AvailableOxygen = hospitals.Sum(h => h.BedAvailabilities
                        .Where(b => b.BedType == BedType.OxygenSupported)
                        .Sum(b => b.AvailableBeds)),
                },
                Hospitals = hospitalDtos
            };

            // Redis mein cache karo
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(dashboard),
                options);

            return Ok(dashboard);
        }
    }
}