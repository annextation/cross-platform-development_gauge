using Cross_platform_development_technologies.Exceptions;

namespace Cross_platform_development_technologies.Models
{
    public class ObservationSession
    {
        public int Id { get; set; }
        public int GaugeId { get; private set; }
        public DateTimeOffset StartedAt { get; private set; }
        public DateTimeOffset? EndedAt { get; private set; }

        public double MeasurementIntervalSeconds { get; private set; }

        private ObservationSession()
        {
        }

        public ObservationSession(
            int gaugeId,
            double measurementIntervalSeconds)
        {
            if (gaugeId <= 0)
            {
                throw new ArgumentException(
                    "Идентификатор манометра должен быть положительным.");
            }

            if (!double.IsFinite(measurementIntervalSeconds)
                || measurementIntervalSeconds <= 0)
            {
                throw new ArgumentException(
                    "Интервал измерений должен быть положительным конечным числом.");
            }

            GaugeId = gaugeId;
            MeasurementIntervalSeconds = measurementIntervalSeconds;
            StartedAt = DateTimeOffset.UtcNow;
        }
        

        public void Complete()
        {
            if (EndedAt.HasValue)
            {
                throw new BusinessConflictException(
                        "Сеанс наблюдения уже завершён.");
            }

            EndedAt = DateTimeOffset.UtcNow;
        }

        public bool ContainsMeasurementTime(DateTimeOffset measuredAt)
        {
            DateTimeOffset upperBound = EndedAt ?? DateTimeOffset.UtcNow;

            return measuredAt >= StartedAt
                && measuredAt <= upperBound;
        }
    }
}