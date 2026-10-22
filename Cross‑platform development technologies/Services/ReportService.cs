using Cross_platform_development_technologies.Contracts;
using Cross_platform_development_technologies.Data;
using Microsoft.EntityFrameworkCore;

namespace Cross_platform_development_technologies.Services
{
    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GaugeSummaryResponse>> GetGaugeSummariesAsync()
        {
            return await _context.Gauges
                .OrderBy(gauge => gauge.Id)
                .Select(gauge => new GaugeSummaryResponse
                {
                    GaugeId = gauge.Id,
                    GaugeName = gauge.Name,

                    SessionCount = _context.Sessions
                        .Count(session => session.GaugeId == gauge.Id),

                    OpenSessionCount = _context.Sessions
                        .Count(session =>
                            session.GaugeId == gauge.Id
                            && session.EndedAt == null),

                    MeasurementCount = _context.Measurements
                        .Count(measurement =>
                            _context.Sessions.Any(session =>
                                session.Id == measurement.SessionId
                                && session.GaugeId == gauge.Id))
                })
                .ToListAsync();
        }
    }
}