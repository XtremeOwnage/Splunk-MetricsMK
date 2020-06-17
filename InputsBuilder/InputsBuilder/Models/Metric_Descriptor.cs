namespace InputsBuilder.Models
{
    public class Metric_Descriptor : StringEnum
    {
        private Metric_Descriptor(string Value) : base(Value) { }

        public static Metric_Descriptor Active => new Metric_Descriptor("active");
        public static Metric_Descriptor Available => new Metric_Descriptor("available");
        public static Metric_Descriptor Average => new Metric_Descriptor("avg");
        public static Metric_Descriptor Closed => new Metric_Descriptor("closed");
        public static Metric_Descriptor Completed => new Metric_Descriptor("completed");
        public static Metric_Descriptor Count => new Metric_Descriptor("count");
        public static Metric_Descriptor Created => new Metric_Descriptor("created");
        public static Metric_Descriptor Free => new Metric_Descriptor("free");
        public static Metric_Descriptor Max => new Metric_Descriptor("max");
        public static Metric_Descriptor Min => new Metric_Descriptor("min");
        public static Metric_Descriptor Open => new Metric_Descriptor("open");
        public static Metric_Descriptor Pending => new Metric_Descriptor("pending");
        public static Metric_Descriptor Size => new Metric_Descriptor("size");
        public static Metric_Descriptor Time => new Metric_Descriptor("time");
        public static Metric_Descriptor Total => new Metric_Descriptor("total");
        public static Metric_Descriptor Used => new Metric_Descriptor("used");

        public static Metric_Descriptor Logical => new Metric_Descriptor("logical");
        public static Metric_Descriptor Physical => new Metric_Descriptor("physical");

    }
}
