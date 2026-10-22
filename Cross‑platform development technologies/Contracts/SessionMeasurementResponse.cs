namespace Cross_platform_development_technologies.Contracts
{
    public class SessionMeasurementResponse
    {
        public int Id { get; set; }

        public int SessionId { get; set; }

        public int GaugeId { get; set; }

        public string GaugeName { get; set; } = string.Empty;

        public DateTimeOffset MeasuredAt { get; set; }

        public double Pressure { get; set; }

        public double Confidence { get; set; }
    }
}