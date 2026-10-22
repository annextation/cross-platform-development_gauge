namespace Cross_platform_development_technologies.Contracts
{
    public class SessionResponse
    {
        public int Id { get; set; }

        public int GaugeId { get; set; }

        public double MeasurementIntervalSeconds { get; set; }

        public DateTimeOffset StartedAt { get; set; }

        public DateTimeOffset? EndedAt { get; set; }
    }
}
