using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace InputsBuilder.Models
{
    public class SelectedCategory
    {
        public PerformanceCounterCategory Category;
        public Splunk_Index Index;
        public int CollectionIntervalSeconds = 300;

        public List<SelectedCounter> SelectedCounters = new List<SelectedCounter>();
        public string Metric_Prefix;
    }
}
