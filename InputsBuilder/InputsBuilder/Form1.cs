using InputsBuilder.Controls;
using InputsBuilder.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InputsBuilder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //Add a dummy index.
            var Indexes = new BindingList<Splunk_Index>();
            Indexes.Add(new Splunk_Index
            {
                Name = "Perfmon",
                RetentionDays = 90
            });


            var Categories = new BindingList<SelectedCategory>();
            foreach (var cat in PerformanceCounterCategory.GetCategories().Where(o => o.GetInstanceNames().Length > 0).Select(o => new SelectedCategory { Category = o, Index = Indexes.First(), Metric_Prefix = MetricNameLookup.Category(o.CategoryName) }))
                Categories.Add(cat);



            var tree = new Controls.PerfCounterTreeView(Indexes)
            {
                Top = menuStrip1.Height,
                Left = 0,
                Width = this.Width,
                Height = this.Height - menuStrip1.Height,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                UseSubItemCheckBoxes = true,
                CheckBoxes = true,
                HierarchicalCheckboxes = false,
                CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick,
                CellEditTabChangesRows = true,
                Roots = Categories,

            };

            this.Controls.Add(tree);

            tree.SetObjects(Categories);
            tree.Sort("Category");

            this.saveToolStripMenuItem.Click += delegate (object sender, EventArgs e)
            {
                //Make sure they have selected a few counters.
                if (!Categories.Any(o => o.Counters.Any(counter => counter.Checked)))
                {
                    MessageBox.Show("Please select counters to generate...");
                    return;
                }

                //Next- select a location to save the config files to.
                FolderBrowserDialog dialog = new FolderBrowserDialog
                {
                    Description = "Select location to store new configuration files. Warning- will overwrite existing files.",
                    ShowNewFolderButton = true,
                };

                //Cancelled.
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                //Lets..... generate some configs...
                Models.ConfigurationData configs = ConfigBuilders.ConfigBuilder.GenerateConfigurationStrings(Categories);

                //and... save them.

                using (var file = File.CreateText(Path.Combine(dialog.SelectedPath, "indexes.conf")))
                    file.Write(configs.Indexes);
                using (var file = File.CreateText(Path.Combine(dialog.SelectedPath, "inputs.conf")))
                    file.Write(configs.Inputs);
                using (var file = File.CreateText(Path.Combine(dialog.SelectedPath, "props.conf")))
                    file.Write(configs.Props);
                using (var file = File.CreateText(Path.Combine(dialog.SelectedPath, "transforms.conf")))
                    file.Write(configs.Transforms);

                //Open up the resulting folder.
                Process.Start("explorer.exe", dialog.SelectedPath);

            };

        }



        private void addCounterMenuItem(object sender, EventArgs e)
        {

        }
    }
}
