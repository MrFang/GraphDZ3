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
        double phi;
        public RoundForm()
        {
            InitializeComponent();
        }

        public RoundForm(double phi) {
            InitializeComponent();
            this.phi = phi;
            SuspendLayout();
            angleBox.Text = phi.ToString("0.##");
            ResumeLayout();
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
