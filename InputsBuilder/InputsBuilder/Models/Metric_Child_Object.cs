namespace InputsBuilder.Models
{
    public class Metric_Child_Object : StringEnum
    {
        private Metric_Child_Object(string Value) : base(Value) { }
        public static Metric_Child_Object Adaptor => new Metric_Child_Object("adaptor");
        public static Metric_Child_Object Interface => new Metric_Child_Object("interface");
        public static Metric_Child_Object Authentication => new Metric_Child_Object("authentication");
        public static Metric_Child_Object Authorization => new Metric_Child_Object("authorization");
        public static Metric_Child_Object Connections => new Metric_Child_Object("connections");
        public static Metric_Child_Object Disk => new Metric_Child_Object("disk");
        public static Metric_Child_Object Errors => new Metric_Child_Object("errors");
        public static Metric_Child_Object Faults => new Metric_Child_Object("faults");
        public static Metric_Child_Object Heap => new Metric_Child_Object("heap");
        public static Metric_Child_Object Host => new Metric_Child_Object("host");
        public static Metric_Child_Object GarbageCollection => new Metric_Child_Object("gc");
        public static Metric_Child_Object JCA => new Metric_Child_Object("JCA");
        public static Metric_Child_Object GPU => new Metric_Child_Object("gpu");
        public static Metric_Child_Object JDBC => new Metric_Child_Object("JDBC");
        public static Metric_Child_Object JVM => new Metric_Child_Object("JVM");
        public static Metric_Child_Object Memory => new Metric_Child_Object("mem");
        public static Metric_Child_Object Network => new Metric_Child_Object("net");
        public static Metric_Child_Object Pool => new Metric_Child_Object("pool"); 
        public static Metric_Child_Object Interrupts => new Metric_Child_Object("interrupts");
        public static Metric_Child_Object Operations => new Metric_Child_Object("ops");
        public static Metric_Child_Object Printer => new Metric_Child_Object("print");
        public static Metric_Child_Object Process => new Metric_Child_Object("process");
        public static Metric_Child_Object Processor => new Metric_Child_Object("cpu");
        public static Metric_Child_Object Requests => new Metric_Child_Object("requests");
        public static Metric_Child_Object Queue => new Metric_Child_Object("queue");
        public static Metric_Child_Object Router => new Metric_Child_Object("router");
        public static Metric_Child_Object Servlet => new Metric_Child_Object("servlet");
        public static Metric_Child_Object Session => new Metric_Child_Object("session");
        public static Metric_Child_Object System => new Metric_Child_Object("system");
        public static Metric_Child_Object Transitions => new Metric_Child_Object("transition");
        public static Metric_Child_Object Thread => new Metric_Child_Object("thread");
        public static Metric_Child_Object ThreadPool => new Metric_Child_Object("threadpool");
        public static Metric_Child_Object Uptime => new Metric_Child_Object("uptime");
        public static Metric_Child_Object Users => new Metric_Child_Object("users");
    }
}
