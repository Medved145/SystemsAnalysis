using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemKyrs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();//153
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReversePolishNotation Rev = new ReversePolishNotation();
            Rev.Expression = textBox1.Text;
            textBox2.Text = Rev.Result().ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
