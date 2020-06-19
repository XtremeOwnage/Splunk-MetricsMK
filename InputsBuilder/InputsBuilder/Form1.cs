using InputsBuilder.Controls;
using InputsBuilder.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
                HierarchicalCheckboxes = true,
                CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick,
                CellEditTabChangesRows = true,
                Roots = Categories,

            };

            this.Controls.Add(tree);

            tree.SetObjects(Categories);

        }



        private void addCounterMenuItem(object sender, EventArgs e)
        {

        }
    }
}
