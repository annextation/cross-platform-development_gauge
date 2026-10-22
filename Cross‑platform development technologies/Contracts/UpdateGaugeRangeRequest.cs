using System.ComponentModel.DataAnnotations;

namespace Cross_platform_development_technologies.Contracts
{
    public class UpdateGaugeRangeRequest
    {
        [Required]
        public double? AllowedMin { get; set; }

        [Required]
        public double? AllowedMax { get; set; }
    }
}