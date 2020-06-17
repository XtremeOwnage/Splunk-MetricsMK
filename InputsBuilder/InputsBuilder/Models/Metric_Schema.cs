namespace InputsBuilder.Models
{
    public class Metric_Schema : StringEnum
    {
        private Metric_Schema(string Value) : base(Value) { }

        public static Metric_Schema Application => new Metric_Schema("app");
        public static Metric_Schema Database => new Metric_Schema("db");
        public static Metric_Schema Host => new Metric_Schema("host");
        public static Metric_Schema OperatingSystem => new Metric_Schema("os");
    }
}
