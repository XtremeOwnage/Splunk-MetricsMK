using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputsBuilder.ConfigBuilders
{
    public static class ConfigBuilder
    {
        const string inputPrefix = "MetricMKV_";
        const string sourceTypePrefix = "Perfmon_Metrics_MKV_";
        const string transformPrefix = "PerfmonMK_To_MetricMK:";
        const string defaultTransformName = "Schema";

        public static string InputsConf(IEnumerable<Models.SelectedCategory> Data)
        {
            var sb = new StringBuilder();

            //Select every category with any selected counter. Order by name.
            foreach (var cat in Data.Where(o => o.SelectedCounters.Any(counter => counter.Selected)).OrderBy(o => o.Category.CategoryName))
            {
                sb.AppendLine($@"
[perfmon://{inputPrefix}{cat.Category.CategoryName}]
counters = {string.Join("; ", cat.SelectedCounters.Where(o => o.Selected).OrderBy(o => o.Counter).Select(o => o.Counter))}
object = {cat.Category.CategoryName}
instances = *
disabled = 0
interval = {cat.CollectionIntervalSeconds}
mode=multikv
useEnglishOnly = true
index = {cat.Index}
showZeroValue=1
sourcetype = {sourceTypePrefix}{cat.Category.CategoryName}

");
            }

            return sb.ToString();
        }

        public static string PropsConf(IEnumerable<Models.SelectedCategory> Data)
        {
            var sb = new StringBuilder();

            //Select every category with any selected counter. Order by name.
            foreach (var cat in Data.Where(o => o.SelectedCounters.Any(counter => counter.Selected)).OrderBy(o => o.Category.CategoryName))
            {
                sb.AppendLine($@"
[{sourceTypePrefix}{cat.Category.CategoryName}]
INDEXED_EXTRACTIONS = tsv
LINE_BREAKER = ([\r\n]+)
NO_BINARY_CHECK = 1
category = Log To Metrics
pulldown_type = 1
METRIC-SCHEMA-TRANSFORMS = {transformPrefix}{defaultTransformName}
TRANSFORMS-perfmonmk = {transformPrefix}{cat.Category.CategoryName}
");
            }

            return sb.ToString();
        }

        public static string TransformsConf(IEnumerable<Models.SelectedCategory> Data)
        {
            //This one is used by all of the props.
            var sb = new StringBuilder()
                .AppendLine($@"
[{transformPrefix}{defaultTransformName}]
#The OLD way
#METRIC-SCHEMA-MEASURES = _ALLNUMS_

# From Splunk-Tony
METRIC-SCHEMA-MEASURES = _NUMSEXCEPT instance
METRIC-SCHEMA-WHITELIST-DIMS = instance, object
");

            //Select every category with any selected counter. Order by name.
            foreach (var cat in Data.Where(o => o.SelectedCounters.Any(counter => counter.Selected)).OrderBy(o => o.Category.CategoryName))
            {
                sb.AppendLine($@"
[{sourceTypePrefix}{cat.Category.CategoryName}]
INDEXED_EXTRACTIONS = tsv
LINE_BREAKER = ([\r\n]+)
NO_BINARY_CHECK = 1
category = Log To Metrics
pulldown_type = 1
METRIC-SCHEMA-TRANSFORMS = metric-schema:PerfmonMK_To_MetricMK_AUTO
TRANSFORMS-perfmonmk = {transformPrefix}{cat.Category.CategoryName}
");
            }

            return sb.ToString();
        }
    }
}
