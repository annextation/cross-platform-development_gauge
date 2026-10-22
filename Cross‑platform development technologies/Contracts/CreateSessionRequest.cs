using System.ComponentModel.DataAnnotations;

namespace Cross_platform_development_technologies.Contracts
{
    public class CreateSessionRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int? GaugeId { get; set; }

        [Required]
        public double? MeasurementIntervalSeconds { get; set; }
    }
}