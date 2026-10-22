namespace Cross_platform_development_technologies.Models
{
    public class Gauge
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public double ScaleMin { get; private set; }
        public double ScaleMax { get; private set; }
        public double AllowedMin { get; private set; }
        public double AllowedMax { get; private set; }

        private Gauge()
        {
            Name = string.Empty;
        }

        public Gauge(
            string name,
            double scaleMin,
            double scaleMax,
            double allowedMin,
            double allowedMax)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Название манометра не должно быть пустым.");
            }

            if (!double.IsFinite(scaleMin) || !double.IsFinite(scaleMax))
            {
                throw new ArgumentException("Границы шкалы должны быть конечными числами.");
            }

            if (scaleMin >= scaleMax)
            {
                throw new ArgumentException("Нижняя граница шкалы должна быть меньше верхней.");
            }

            Name = name.Trim();
            ScaleMin = scaleMin;
            ScaleMax = scaleMax;

            SetAllowedRange(allowedMin, allowedMax);
        }


        public void SetAllowedRange(double min, double max)
        {
            if (!double.IsFinite(min) || !double.IsFinite(max))
            {
                throw new ArgumentException("Границы диапазона должны быть конечными числами.");
            }

            if (min > max)
            {
                throw new ArgumentException("Нижняя граница не может быть больше верхней.");
            }

            if (min < ScaleMin || max > ScaleMax)
            {
                throw new ArgumentException("Допустимый диапазон должен находиться внутри шкалы.");
            }

            AllowedMin = min;
            AllowedMax = max;
        }

        public void ValidatePressureWithinScale(double pressure)
        {
            if (!double.IsFinite(pressure))
            {
                throw new ArgumentException("Давление должно быть конечным числом.");
            }

            if (pressure < ScaleMin || pressure > ScaleMax)
            {
                throw new ArgumentException(
                    $"Давление должно находиться в пределах шкалы манометра от {ScaleMin} до {ScaleMax} МПа.");
            }
        }
        public bool IsPressureAllowed(double pressure)
        {
            return double.IsFinite(pressure)
                && pressure >= AllowedMin
                && pressure <= AllowedMax;
        }
    }
}