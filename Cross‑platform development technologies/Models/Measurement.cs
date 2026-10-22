namespace Cross_platform_development_technologies.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        public int SessionId { get; private set; }
        public DateTimeOffset MeasuredAt { get; private set; }
        public double Pressure { get; private set; }
        public double Confidence { get; private set; }

        private Measurement()
        {
        }

        public Measurement(
                int sessionId,
                DateTimeOffset measuredAt,
                double pressure,
                double confidence)
        {
            if (sessionId <= 0)
            {
                throw new ArgumentException(
                    "Идентификатор сеанса должен быть положительным.");
            }

            UpdateResult(pressure, confidence);

            SessionId = sessionId;
            MeasuredAt = measuredAt;
        }

        public bool MeetsConfidenceThreshold(double threshold)
        {
            if (!double.IsFinite(threshold)
                || threshold < 0
                || threshold > 1)
            {
                throw new ArgumentException(
                    "Порог уверенности должен быть от 0 до 1.");
            }

            return Confidence >= threshold;
        }

        public void UpdateResult(double pressure, double confidence)
        {
            if (!double.IsFinite(pressure))
            {
                throw new ArgumentException(
                    "Давление должно быть конечным числом.");
            }

            if (!double.IsFinite(confidence)
                || confidence < 0
                || confidence > 1)
            {
                throw new ArgumentException(
                    "Уверенность распознавания должна быть от 0 до 1.");
            }

            Pressure = pressure;
            Confidence = confidence;
        }
    }
}