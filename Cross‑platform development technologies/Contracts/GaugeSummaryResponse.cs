namespace Cross_platform_development_technologies.Contracts
{
    public class GaugeSummaryResponse
    {
        public int GaugeId { get; set; }

        public string GaugeName { get; set; } = string.Empty;

        public int SessionCount { get; set; }

        public int OpenSessionCount { get; set; }

        public int MeasurementCount { get; set; }
    }
}