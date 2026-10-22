using Cross_platform_development_technologies.Data;
using Cross_platform_development_technologies.Models;
using Cross_platform_development_technologies.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Cross_platform_development_technologies.Services
{
    public class GaugeService
    {
        private readonly AppDbContext _context;

        public GaugeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Gauge>> GetAllAsync()
        {
            return await _context.Gauges
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Gauge?> GetByIdAsync(int id)
        {
            return await _context.Gauges
                .AsNoTracking()
                .FirstOrDefaultAsync(gauge => gauge.Id == id);
        }

        public async Task<Gauge> CreateAsync(
                                    string name,
                                    double scaleMin,
                                    double scaleMax,
                                    double allowedMin,
                                    double allowedMax)
        {
            var gauge = new Gauge(
                name,
                scaleMin,
                scaleMax,
                allowedMin,
                allowedMax);

            _context.Gauges.Add(gauge);

            await _context.SaveChangesAsync();

            return gauge;
        }

        public async Task<Gauge?> UpdateAllowedRangeAsync(
                                    int id,
                                    double allowedMin,
                                    double allowedMax)
        {
            var gauge = await _context.Gauges
                .FirstOrDefaultAsync(gauge => gauge.Id == id);

            if (gauge is null)
            {
                return null;
            }

            gauge.SetAllowedRange(allowedMin, allowedMax);

            await _context.SaveChangesAsync();

            return gauge;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var gauge = await _context.Gauges
                .FirstOrDefaultAsync(gauge => gauge.Id == id);

            if (gauge is null)
            {
                return false;
            }

            bool hasSessions = await _context.Sessions
                .AnyAsync(session => session.GaugeId == id);

            if (hasSessions)
            {
                throw new BusinessConflictException(
                        "Нельзя удалить манометр, у которого есть сеансы наблюдения.");
            }

            _context.Gauges.Remove(gauge);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}