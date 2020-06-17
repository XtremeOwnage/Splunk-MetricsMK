using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace InputsBuilder.Models
{
    public static class MetricNameLookup
    {
        const string defaultHostPrefix = "host";
        static Dictionary<string, string> CategoryToMetric = new Dictionary<string, string>()
        {
            {"GPU Adapter Memory"       , string.Join('.', Metric_Schema.Host, Metric_Child_Object.GPU, Metric_Child_Object.Memory) },
            {"GPU Engine"               , string.Join('.', Metric_Schema.Host, Metric_Child_Object.GPU) },
            {"GPU Local Adapter Memory" , string.Join('.', Metric_Schema.Host, Metric_Child_Object.GPU, Metric_Child_Object.Adaptor, Metric_Child_Object.Memory) },
            {"GPU Process Memory"       , string.Join('.', Metric_Schema.Host, Metric_Child_Object.GPU, Metric_Child_Object.Process, Metric_Child_Object.Memory) },
            {"Database"                 , string.Join('.', Metric_Schema.Host, Metric_Child_Object.GPU) },
            {"LogicalDisk"              , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Disk, Metric_Descriptor.Logical) },
            {"PhysicalDisk"             , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Disk, Metric_Descriptor.Physical) },
            {"Paging File"              , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Memory) },
            {"Network Adapter"          , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Network, Metric_Child_Object.Adaptor) },
            {"Network Interface"        , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Network, Metric_Child_Object.Interface) },
            {"Process"                  , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Process) },
            {"Processor"                , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Processor) },
            {"Processor Information"    , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Processor) },
            {"Print Queue"              , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Printer, Metric_Child_Object.Queue) },
        };


        public static string? Category(string CategoryName) => CategoryToMetric.FirstOrDefault(o => o.Key.Equals(CategoryName, StringComparison.OrdinalIgnoreCase)).Value;

        public static string? Counter(string CounterName)
        {
            //Lets break apart the counter name and paste it back togather.
            //Also- convert it to lower invariant.
            string[] Pieces = CounterName.ToLowerInvariant().Trim().Split(' ');
            int cnt = Pieces.Length;

            //Empty ?
            if (Pieces.Length == 0)
                return null;

            //A linked list to hold the pieces togather.
            LinkedList<string> metricName = new LinkedList<string>();

            //This refers to a percent. The unit needs to go at the end.
            if (Pieces[0] == "%")
            {
                metricName.AddLast(Metric_Unit.Percent);

                //Just take the rest on.......
                if (Pieces[cnt - 1] == "time" && Pieces.Length == 3)
                {
                    metricName.AddFirst(Metric_Descriptor.Time);
                    metricName.AddFirst(Pieces[1]);
                }
                else
                {

                }



            }
            //Measuring a unit, per second. We will assume it is referring to an average.
            //else if is being used- since...... % /sec doesn't make much since in a metric name.
            else if (Pieces.Last().EndsWith("/sec"))
            {
                //Remember- arrays are 0 based. Subtract 1 from the total array length to get the last element.
                var unit = Pieces[cnt - 1].Replace("/sec", "");
                switch (unit)
                {
                    case "byte":
                    case "bytes":
                        metricName.AddLast(Metric_Unit.Bytes);
                        break;
                    case "fault":
                    case "faults":
                        metricName.AddLast(Metric_Child_Object.Faults);
                        break;
                    case "operation":
                    case "operations":
                        metricName.AddLast(Metric_Child_Object.Operations);
                        break;
                    case "interrupt":
                    case "interrupts":
                        metricName.AddLast(Metric_Child_Object.Interrupts);
                        break;
                    case "queue":
                    case "queued":
                        metricName.AddLast(Metric_Child_Object.Queue);
                        break;
                    case "transition":
                    case "transitions":
                        metricName.AddLast(Metric_Child_Object.Transitions);
                        break;
                    default:
                        break;
                }

                //Add in the "object"
                var obj = string.Join('.', Pieces.SkipLast(1));

                metricName.AddFirst(obj);

                metricName.AddLast(Metric_Unit.PerSecond);
            }
            else
            {
                //Tack the name togather, and cross your fingers.....
                return String.Join('.', Pieces);
            }
            return String.Join('.', metricName);
        }
    }
}
