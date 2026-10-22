using System.ComponentModel.DataAnnotations;

namespace Cross_platform_development_technologies.Contracts
{
    public class CreateGaugeRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public double? ScaleMin { get; set; }

        [Required]
        public double? ScaleMax { get; set; }

        [Required]
        public double? AllowedMin { get; set; }

        [Required]
        public double? AllowedMax { get; set; }
    }
}