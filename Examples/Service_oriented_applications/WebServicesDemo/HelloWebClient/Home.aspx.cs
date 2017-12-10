using System;
using System.Web.UI;
using HelloWebClient.HelloWebService;

namespace HelloWebClient
{
    public partial class Home1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //Do not forget to add default defaultDocument settings to Web.config
            var client = new HelloWebServiceSoapClient();
            this.Label1.Text = client.GetMessage(this.TextBox1.Text);
        }
    }
}