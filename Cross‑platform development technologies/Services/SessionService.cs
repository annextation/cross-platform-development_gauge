using Cross_platform_development_technologies.Data;
using Cross_platform_development_technologies.Models;
using Cross_platform_development_technologies.Contracts;
using Cross_platform_development_technologies.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Cross_platform_development_technologies.Services
{
    public class SessionService
    {
        private readonly AppDbContext _context;

        public SessionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ObservationSession?> CreateAsync(
            int gaugeId,
            double measurementIntervalSeconds)
        {
            bool gaugeExists = await _context.Gauges
                .AnyAsync(gauge => gauge.Id == gaugeId);

            if (!gaugeExists)
            {
                return null;
            }

            var session = new ObservationSession(gaugeId, measurementIntervalSeconds);

            _context.Sessions.Add(session);

            await _context.SaveChangesAsync();

            return session;
        }

        public async Task<ObservationSession?> GetByIdAsync(int id)
        {
            return await _context.Sessions
                .AsNoTracking()
                .FirstOrDefaultAsync(session => session.Id == id);
        }

        public async Task<ObservationSession?> CompleteAsync(int id)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(session => session.Id == id);

            if (session is null)
            {
                return null;
            }

            session.Complete();

            await _context.SaveChangesAsync();

            return session;
        }

        public async Task<List<SessionMeasurementResponse>?>
        GetMeasurementsAsync(int sessionId)
        {
            bool sessionExists = await _context.Sessions
                .AnyAsync(session => session.Id == sessionId);

            if (!sessionExists)
            {
                return null;
            }

            var query = _context.Measurements
                .Where(measurement => measurement.SessionId == sessionId)
                .Join(
                    _context.Sessions,
                    measurement => measurement.SessionId,
                    session => session.Id,
                    (measurement, session) => new
                    {
                        Measurement = measurement,
                        Session = session
                    })
                .Join(
                    _context.Gauges,
                    item => item.Session.GaugeId,
                    gauge => gauge.Id,
                    (item, gauge) => new
                    {
                        Measurement = item.Measurement,
                        Gauge = gauge
                    })
                .OrderBy(item => item.Measurement.Id)
                .Select(item => new SessionMeasurementResponse
                {
                    Id = item.Measurement.Id,
                    SessionId = item.Measurement.SessionId,
                    GaugeId = item.Gauge.Id,
                    GaugeName = item.Gauge.Name,
                    MeasuredAt = item.Measurement.MeasuredAt,
                    Pressure = item.Measurement.Pressure,
                    Confidence = item.Measurement.Confidence
                });

            return await query.ToListAsync();
        }

        public async Task<List<ObservationSession>> GetAllAsync()
        {
            return await _context.Sessions
                .AsNoTracking()
                .OrderBy(session => session.Id)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(session => session.Id == id);

            if (session is null)
            {
                return false;
            }

            bool hasMeasurements = await _context.Measurements
                .AnyAsync(measurement => measurement.SessionId == id);

            if (hasMeasurements)
            {
                throw new BusinessConflictException(
                        "Нельзя удалить сеанс, в котором есть измерения.");
            }

            _context.Sessions.Remove(session);

            await _context.SaveChangesAsync();

            return true;
        }
    }   
}