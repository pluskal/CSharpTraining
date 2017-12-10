using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;
using RemotingServiceInterfaces;

namespace HelloRemotingServiceClient
{
    public partial class MainView : Form
    {
        public MainView()
        {
            this.InitializeComponent();
            var channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            this.Client = (IHelloRemotingService)
                Activator.GetObject(
                    typeof(IHelloRemotingService),
                    "tcp://localhost:8081/GetMessage"
                );
        }

        public IHelloRemotingService Client { get; }

        private void button1_Click(object sender, EventArgs e)
        {
            this.label1.Text = this.Client.GetMessage(this.textBox1.Text);
        }

        private void MainView_Load(object sender, EventArgs e)
        {
        }
    }
}