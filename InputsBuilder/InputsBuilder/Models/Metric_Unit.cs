namespace InputsBuilder.Models
{
    public class Metric_Unit : StringEnum
    {
        private Metric_Unit(string Value) : base(Value) { }

        public static Metric_Unit Bytes => new Metric_Unit("bytes");
        public static Metric_Unit Kilobyte => new Metric_Unit("kb");
        public static Metric_Unit Megabyte => new Metric_Unit("mb");
        public static Metric_Unit Millisecond => new Metric_Unit("ms");
        public static Metric_Unit Percent => new Metric_Unit("pct");
        public static Metric_Unit PerSecond => new Metric_Unit("ps");
        public static Metric_Unit Second => new Metric_Unit("sec");

        public static Metric_Unit Read => new Metric_Unit("read");
        public static Metric_Unit Write => new Metric_Unit("write");
        public static Metric_Unit Transfer => new Metric_Unit("transfers");


        public static Metric_Unit? Normalize(string Input)
        {
            switch (Input)
            {
                case "milliseconds":
                case "millisecond":
                    return Millisecond;

                case "sec":
                case "second":
                    return PerSecond;

                case "unit.kbyte":
                case "kilobyte":
                    return Kilobyte;

                case "bytes":
                    return Bytes;

                case "read":
                case "reads":
                    return Read;

                case "write":
                case "writes":
                    return Write;

                case "transfer":
                case "transfers":
                    return Transfer;

                case null:
                case "N/A":
                case "none":
                case "process.unit.none":
                    return null;

                default:
                    return null;
            }
        }
    }
}
