using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloFormsClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var client = new HelloService.HelloServiceClient("NetTcpBinding_IHelloService");
            this.label1.Text = client.GetMessage(this.textBox1.Text);
        }
    }
}
