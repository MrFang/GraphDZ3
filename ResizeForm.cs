using System;
using System.Windows.Forms;

namespace GraphDZ3
{
    public partial class ResizeForm : Form
    {
        public double len;
        public ResizeForm()
        {
            InitializeComponent();
        }

        public ResizeForm(double size) {
            InitializeComponent();
            this.len = size;
            SuspendLayout();
            sizeBox.Text = size.ToString("0.##");
            ResumeLayout();
        }
        private void OkButton_Click(object sender, EventArgs e)
        {
            bool isOk = Double.TryParse(sizeBox.Text, out len);

            if (!isOk || len <= 0) {
                MessageBox.Show("Incorect size");
                DialogResult = DialogResult.Abort;
            }
        }
    }
}
