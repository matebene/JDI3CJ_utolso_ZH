using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JDI3CJ_utolso_ZH
{
    public partial class UjFogas : Form
    {
        public UjFogas()
        {
            InitializeComponent();
            button1.Enabled = false;
        }

        private void UjFogas_Load(object sender, EventArgs e)
        {

        }
        bool ÚjFogásValidate(string data)
        {
            int outputValue = 0;
            if (string.IsNullOrEmpty(data))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (ÚjFogásValidate((string)textBox1.Text))
            {
                e.Cancel = true;
                button1.Enabled = false;
                errorProvider1.SetError(textBox1, "A mező nem lehet üres");
            }
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            button1.Enabled = true;
            errorProvider1.SetError(textBox1, "");
        }

        private void richTextBox1_Validating(object sender, CancelEventArgs e)
        {
            if (ÚjFogásValidate((string)richTextBox1.Text))
            {
                e.Cancel = true;
                button1.Enabled = false;
                errorProvider1.SetError(richTextBox1, "A mező nem lehet üres");
            }
        }

        private void richTextBox1_Validated(object sender, EventArgs e)
        {
            button1.Enabled = true;
            errorProvider1.SetError(richTextBox1, "");
        }
    }
}
