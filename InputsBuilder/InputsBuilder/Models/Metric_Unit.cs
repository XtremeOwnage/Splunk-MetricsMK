namespace InputsBuilder.Models
{
    public class Metric_Unit : StringEnum
    {
        private Metric_Unit(string Value) : base(Value) { }

        public static Metric_Unit Percent => new Metric_Unit("pct");
        public static Metric_Unit Second => new Metric_Unit("sec");
        public static Metric_Unit Millisecond => new Metric_Unit("ms");
        public static Metric_Unit Kilobyte => new Metric_Unit("kb");
        public static Metric_Unit Megabyte => new Metric_Unit("mb");
        public static Metric_Unit Bytes => new Metric_Unit("bytes");
        public static Metric_Unit PerSecond => new Metric_Unit("ps");


        public static Metric_Unit Normalize(string Input)
        {
            switch (Input)
            {
                case "Milliseconds":
                case "MILLISECOND":
                    return Millisecond;

                case "SECOND":
                    return Second;

                case "unit.kbyte":
                case "KILOBYTE":
                    return Kilobyte;

                case null:
                case "N/A":
                case "None":
                case "Process.unit.none":
                    return null;

                default:
                    return null;
            }
        }
    }
}
