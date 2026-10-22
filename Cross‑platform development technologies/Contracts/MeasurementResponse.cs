namespace Cross_platform_development_technologies.Contracts
{
    public class MeasurementResponse
    {
        public int Id { get; set; }

        public int SessionId { get; set; }

        public DateTimeOffset MeasuredAt { get; set; }

        public double Pressure { get; set; }

        public double Confidence { get; set; }
    }
}
