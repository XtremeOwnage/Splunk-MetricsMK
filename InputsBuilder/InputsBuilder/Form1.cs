using InputsBuilder.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

            //test code.
            var form = new AddCounter();
            form.ShowDialog();
            this.Close();


            var tree = new Controls.TreeView();
            this.Controls.Add(tree);

            

            tree.Dock = DockStyle.Fill;
        }

        private void addCounterMenuItem(object sender, EventArgs e)
        {
            var form = new AddCounter();

            form.ShowDialog();
        }
    }
}
