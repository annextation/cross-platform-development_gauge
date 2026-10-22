using Cross_platform_development_technologies.Data;
using Cross_platform_development_technologies.Models;
using Microsoft.EntityFrameworkCore;

namespace Cross_platform_development_technologies.Services
{
    public class MeasurementService
    {
        private readonly AppDbContext _context;

        public MeasurementService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Measurement?> CreateAsync(
            int sessionId,
            DateTimeOffset measuredAt,
            double pressure,
            double confidence)
        {
            var session = await _context.Sessions
                .AsNoTracking()
                .FirstOrDefaultAsync(session => session.Id == sessionId);

            if (session is null)
            {
                return null;
            }

            if (!session.ContainsMeasurementTime(measuredAt))
            {
                throw new ArgumentException(
                    "Время измерения не входит в период сеанса "
                    + "или находится в будущем.");
            }

            await ValidatePressureAsync(sessionId, pressure);

            var measurement = new Measurement(
                sessionId,
                measuredAt,
                pressure,
                confidence);

            _context.Measurements.Add(measurement);

            await _context.SaveChangesAsync();

            return measurement;
        }

        public async Task<Measurement?> GetByIdAsync(int id)
        {
            return await _context.Measurements
                .AsNoTracking()
                .FirstOrDefaultAsync(measurement => measurement.Id == id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var measurement = await _context.Measurements
                .FirstOrDefaultAsync(measurement => measurement.Id == id);

            if (measurement is null)
            {
                return false;
            }

            _context.Measurements.Remove(measurement);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Measurement?> UpdateResultAsync(
                                            int id,
                                            double pressure,
                                            double confidence)
        {
            var measurement = await _context.Measurements
                .FirstOrDefaultAsync(measurement => measurement.Id == id);

            if (measurement is null)
            {
                return null;
            }

            await ValidatePressureAsync(measurement.SessionId, pressure);

            measurement.UpdateResult(pressure, confidence);

            await _context.SaveChangesAsync();

            return measurement;
        }
        private async Task ValidatePressureAsync(int sessionId, double pressure)
        {
            var gauge = await _context.Sessions
                .Where(session => session.Id == sessionId)
                .Join(
                    _context.Gauges,
                    session => session.GaugeId,
                    gauge => gauge.Id,
                    (session, gauge) => gauge)
                .AsNoTracking()
                .SingleAsync();

            gauge.ValidatePressureWithinScale(pressure);
        }
    }
}