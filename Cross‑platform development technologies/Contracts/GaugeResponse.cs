namespace Cross_platform_development_technologies.Contracts
{
    public class GaugeResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double ScaleMin { get; set; }

        public double ScaleMax { get; set; }

        public double AllowedMin { get; set; }

        public double AllowedMax { get; set; }
    }
}
