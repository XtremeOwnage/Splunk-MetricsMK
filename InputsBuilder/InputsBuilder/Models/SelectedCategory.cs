using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace InputsBuilder.Models
{
    public class SelectedCategory
    {
        public string Name => Category.CategoryName;
        public bool Checked
        {
            get => this.Counters.Count > 0 && this.Counters.All(o => o.Checked);
            set
            {
                if (this.Counters.Count == 0)
                    this.Expand();

                foreach (var c in this.Counters)
                    c.Checked = value;
            }
        }
        public PerformanceCounterCategory Category;
        public Splunk_Index Index;
        public int CollectionIntervalSeconds = 300;

        public BindingList<SelectedCounter> Counters = new BindingList<SelectedCounter>();
        public string Metric_Prefix;

        /// <summary>
        /// A flag which keeps track if we have loaded the counmters for the category yet.
        /// </summary>
        private bool loaded = false;
        public void Reload()
        {
            this.Counters.Clear();
            this.loaded = false;
            Expand();

        }
        public void Expand()
        {
            if (loaded)
                return;

            if (Category.CategoryType == PerformanceCounterCategoryType.SingleInstance)
            {
                var counters = Category
                    .GetCounters()
                    .Select(o => new SelectedCounter
                    {
                        Category = this,
                        Name = o.CounterName,
                        //InstanceCount = 1,
                        Checked = false,
                        MetricName = MetricNameLookup.Counter(this.Name, o.CounterName)
                    });
                foreach (var c in counters)
                    this.Counters.Add(c);

            }
            else if (Category.CategoryType == PerformanceCounterCategoryType.MultiInstance)
            {
                var instanceNames = Category.GetInstanceNames();
                var counters = instanceNames
                    //I assume the counters will remain the same between instances.
                    //This was put in to prevent... issues when accessing categories with many instances... such as "thread".
                    //.Take(instanceNames.Length < 15 ? instanceNames.Length : 1)
                    .Take(1)
                    .SelectMany(o => this.Category.GetCounters(o))
                    .GroupBy(o => o.CounterName)
                    .Select(o => new SelectedCounter
                    {
                        Category = this,
                        Name = o.Key,
                        //InstanceCount = instanceNames.Length < 15 ? o.Count() : (int?)null,
                        Checked = false,
                        MetricName = MetricNameLookup.Counter(this.Name, o.Key)
                    }); ;

                foreach (var c in counters)
                    this.Counters.Add(c);

            }

            loaded = true;
        }
    }
}
