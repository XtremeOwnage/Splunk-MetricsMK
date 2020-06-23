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
            {"Database"                 , string.Join('.', Metric_Schema.Host, Metric_Schema.Database) },
            {"LogicalDisk"              , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Disk, Metric_Descriptor.Logical) },
            {"PhysicalDisk"             , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Disk, Metric_Descriptor.Physical) },
            {"Paging File"              , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Memory) },
            {"Network Adapter"          , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Network, Metric_Child_Object.Adaptor) },
            {"Network Interface"        , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Network, Metric_Child_Object.Interface) },
            {"Process"                  , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Process) },
            {"Processor"                , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Processor) },
            {"Processor Information"    , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Processor) },
            {"Print Queue"              , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Printer, Metric_Child_Object.Queue) },
            {"Thread"                   , string.Join('.', Metric_Schema.Host, Metric_Child_Object.Thread) },
        };

        static Dictionary<(string Category, string Counter), string> CounterToMetric = new Dictionary<(string Category, string Counter), string>()
        {
            {("Processor Information"   , "% of Maximum Frequency")            , string.Join('.', "maximum_frequency.pct") },
           // {("Logical Disk"            , "% Free Space")                      , string.Join('.', Metric_Descriptor.Free, Metric_Unit.Percent) },
        };

        //No idea what to call this.
        // It is literally a hashset of words to look for in a two-word perfmon metric, where the second word, describes what it is doing to the first word.
        static Dictionary<string, string> WordsToDescribe = new Dictionary<string, string>()
        {
            { "usage"                   , Metric_Descriptor.Used },
            { "committed"               , Metric_Descriptor.Committed }
        };

        public static string? Category(string CategoryName) => CategoryToMetric.FirstOrDefault(o => o.Key.Equals(CategoryName, StringComparison.OrdinalIgnoreCase)).Value;

        public static string? Counter(string CategoryName, string CounterName)
        {
            //If there is a defined value, use it, instead of guessing the proper value.
            if (CounterToMetric.ContainsKey((CategoryName, CounterName)))
                return CounterToMetric[(CategoryName, CounterName)];

            //Having disk specified twice is a bit redundant....
            if (CategoryName.Contains("Disk") && CounterName.Contains("Disk"))
                CounterName = CounterName.Replace("Disk", "");

            //Lets break apart the counter name and paste it back togather.
            //Also- convert it to lower invariant.
            string[] Pieces = CounterName
                .ToLowerInvariant()
                .Trim()
                .Split(' ')
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .ToArray();







            int cnt = Pieces.Length;

            //Empty ?
            if (Pieces.Length == 0)
                return null;

            #region The NEW Way...            
            //Lets make a copy of the array, as a list, so that we can remove the pieces we are done using.
            var list = Pieces.ToList();

            //Create a linked list, to allow us to add/remove items at any location pretty easily.
            LinkedList<string> name = new LinkedList<string>();

            //Every metric should contain a "Unit". Bytes/Time/Pct... etc. This lets the user know what the metric is measured in.
            Metric_Unit? Unit = null;
            //SOME, not all, units will contain a descriptor. Avg, Min, Max, etc...
            Metric_Descriptor? Unit_Descriptor = null;


            //Try and determine the unit.
            if (list[0] == "%")
            {
                Unit = Metric_Unit.Percent;
                list.RemoveAt(0);
            }
            else
            {

            }

            //Determine if there is a descriptor attached.
            if (list[0] == "avg.")
            {
                Unit_Descriptor = Metric_Descriptor.Average;
                list.RemoveAt(0);
            }

            //Bytes/sec
            //Sec/read
            //Transisitions/sec
            //etc...... make those values into something "readable"
            if (list.Last().Contains('/'))
            {
                var couldParse = parseTwoPart(list.Last());

                if (couldParse.Success)
                {
                    name.AddLast(couldParse.Name);

                    list.RemoveAt(list.Count - 1);
                }
                else
                {
                    //Do something here?
                }
            }

            //Add everything else togather.
            name.AddLast(string.Join('.', list));

            if (Unit != null)
                name.AddLast(Unit);

            if (Unit_Descriptor != null)
                name.AddLast(Unit_Descriptor);

            return String.Join('.', name);

            #endregion
            //The OLD Way..... before I had a good idea.
            #region the OLD Way...
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
                    //Uh, piece it togather.... and cross fingers.
                    metricName.AddFirst(String.Join('.', Pieces.Skip(1)));
                }
            }
            else if (Pieces[0] == "avg.")
            {
                metricName.AddLast(string.Join('.', Pieces.Skip(1)));
                metricName.AddLast(Metric_Descriptor.Average);
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
                        //Add it as-is....
                        metricName.AddLast(unit);
                        break;
                }

                if (Pieces.Length > 1)
                {
                    //Add in the "object"
                    var obj = string.Join('.', Pieces.SkipLast(1));

                    metricName.AddFirst(obj);
                }

                metricName.AddLast(Metric_Unit.PerSecond);
            }
            //Weird cases like this (which actually works....), but looks like completely horrible code
            //Really makes me hope that somebody who is more adept at determining a good magic formula...
            // can contribute to this code base.
            // Until then- realize this does work.
            else if (cnt == 2 && WordsToDescribe.ContainsKey(Pieces[1]))
            {
                //Reverse the order.
                metricName.AddFirst(WordsToDescribe[Pieces[1]]);
                metricName.AddLast(Pieces[0]);
            }
            else
            {
                //Tack the name togather, and cross your fingers.....
                return String.Join('.', Pieces);
            }
            return String.Join('.', metricName);

            #endregion
        }


        private static (bool Success, string Name) parseTwoPart(string Input)
        {
            string ret = "";

            //Lets attempt to parse this out...
            var p = Input.Split('/');

            var try_parse_unit_1 = Metric_Unit.Normalize(p[0]);
            if (try_parse_unit_1 != null)
            {
                ret += p[1] + "." + try_parse_unit_1.Value;
                return (true, ret);
            }


            return (false, null);
        }
    }
}
