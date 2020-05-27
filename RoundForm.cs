using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphDZ3
{
    public partial class RoundForm : Form
    {
        public double phi = 0;
        public RoundForm()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            bool isOk = Double.TryParse(angleBox.Text, out phi);

            if (!isOk || phi < 0 || phi > 2*Math.PI)
            {
                MessageBox.Show("Incorrect angle");
                DialogResult = DialogResult.Abort;
            }
        }
    }
}
