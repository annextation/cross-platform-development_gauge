using System.ComponentModel.DataAnnotations;

namespace Cross_platform_development_technologies.Contracts
{
    public class UpdateMeasurementResultRequest
    {
        [Required]
        public double? Pressure { get; set; }

        [Required]
        [Range(0.0, 1.0)]
        public double? Confidence { get; set; }
    }
}