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

        public static Models.ConfigurationData GenerateConfigurationStrings(IEnumerable<Models.SelectedCategory> Data)
        {
            return new Models.ConfigurationData
            {
                Indexes = indexes(Data),
                Inputs = inputs(Data),
                Props = props(Data),
                Transforms = transforms(Data),
            };
        }
        private static string indexes(IEnumerable<Models.SelectedCategory> Data)
        {
            var sb = new StringBuilder();

            //Select every category with any selected counter. Order by name.
            foreach (var cat in Data.Select(o => o.Index).Distinct())
            {
                //Convert retention days, into seconds.

                sb.AppendLine($@"[{cat.Name}]
coldPath = $SPLUNK_DB\$_index_name\colddb
enableDataIntegrityControl = 0
enableTsidxReduction = 0
datatype = metric
homePath = $SPLUNK_DB\$_index_name\db
thawedPath = $SPLUNK_DB\$_index_name\thaweddb");
                if (cat.RetentionDays.HasValue)
                {
                    var retentionSecs = cat.RetentionDays.Value * 24 * 60 * 60;
                    sb.AppendLine($"retention = { retentionSecs }");
                }
                if (cat.MaxDataSizeMB.HasValue)
                    sb.AppendLine($"maxTotalDataSizeMB = {cat.MaxDataSizeMB}");

            }

            return sb.ToString();
        }

        private static string inputs(IEnumerable<Models.SelectedCategory> Data)
        {
            var sb = new StringBuilder();

            //Select every category with any selected counter. Order by name.
            foreach (var cat in Data.Where(o => o.Counters.Any(counter => counter.Checked)).OrderBy(o => o.Category.CategoryName))
            {
                sb.AppendLine($@"
[perfmon://{inputPrefix}{cat.Category.CategoryName}]
counters = {string.Join("; ", cat.Counters.Where(o => o.Checked).OrderBy(o => o.Name).Select(o => o.Name))}
object = {cat.Category.CategoryName}
instances = *
disabled = 0
interval = {cat.CollectionIntervalSeconds}
mode=multikv
useEnglishOnly = true
index = {cat.Index.Name}
showZeroValue = 1
sourcetype = {sourceTypePrefix}{cat.Category.CategoryName}

");
            }

            return sb.ToString();
        }

        private static string props(IEnumerable<Models.SelectedCategory> Data)
        {
            var sb = new StringBuilder();

            //Select every category with any selected counter. Order by name.
            foreach (var cat in Data.Where(o => o.Counters.Any(counter => counter.Checked)).OrderBy(o => o.Category.CategoryName))
            {
                sb.AppendLine($@"
[{sourceTypePrefix}{cat.Category.CategoryName}]
INDEXED_EXTRACTIONS = tsv
LINE_BREAKER = ([\r\n]+)
NO_BINARY_CHECK = 1
category = Log To Metrics
pulldown_type = 1
METRIC-SCHEMA-TRANSFORMS = metric-schema:{defaultTransformName}
TRANSFORMS-perfmonmk = {transformPrefix}{cat.Category.CategoryName}
");
            }

            return sb.ToString();
        }

        private static string transforms(IEnumerable<Models.SelectedCategory> Data)
        {
            //This one is used by all of the props.
            var sb = new StringBuilder()
                .AppendLine($@"
[metric-schema:{defaultTransformName}]
#The OLD way
#METRIC-SCHEMA-MEASURES = _ALLNUMS_

# From Splunk-Tony
METRIC-SCHEMA-MEASURES = _NUMS_EXCEPT_ instance
METRIC-SCHEMA-WHITELIST-DIMS = instance
");

            //Select every category with any selected counter. Order by name.
            var categories = Data.Where(o => o.Counters.Any(counter => counter.Checked))
                .OrderBy(o => o.Category.CategoryName);
            foreach (var cat in categories)
            {
                int cnt = cat.Counters.Where(o => o.Checked).Count();
                sb.AppendLine($@"[{transformPrefix}{cat.Category.CategoryName}]");
                #region Build REGEX
                //Append the static part of the REGEX
                sb.Append(@"REGEX = collection=\""?(?<collection>[^\""\n]+)\""?\ncategory=\""?(?<category>[^\""\n]+)\""?\nobject=\""?(?<object>[^\""\n]+)\""?\n");


                //Before anybody fusses about declaring this string on each interation- 
                //go research what the const keyword does.
                const string rex_AnythingExceptTab = @"([^\t]+)\t";
                //Build the dynamic part of the regex.
                //Description- Repeats {rex_AnythingExceptTab} N types, where N = number of counters.
                // + 1 is present, to account for the instance field.
                var rex = string.Concat(Enumerable.Repeat(rex_AnythingExceptTab, cnt + 1));

                //This matches the field names in the headers.
                sb.Append(rex);

                //Drops to the next line, which contains the actual values.
                sb.Append(@"\n");

                //This matches the values.
                sb.Append(rex);

                //Remove the tailing tab.
                sb.Remove(sb.Length - 2, 2);

                ////This matches the end
                //sb.AppendLine(@"\n");
                sb.AppendLine();
                #endregion
                #region Build FORMAT
                sb.Append(@$"FORMAT = collection::""$1"" category::""$2"" object::""$3"" instance::""${ cnt + 5}"" ");

                for (int i = 0; i < cnt; i++)
                {
                    sb.Append(@$"""${5 + i}""::""${cnt + i + 6}"" ");
                }
                sb
                    .AppendLine()
                    .AppendLine("WRITE_META = true");


                //Append two extra lines.... to clean up the file.
                sb.AppendLine().AppendLine();
                #endregion
            }


            return sb.ToString();
        }
    }
}
