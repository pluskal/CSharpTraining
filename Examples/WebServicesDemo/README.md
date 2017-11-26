# Web service 

**Suggested Videos** \
 [Part 1 - Introduction to WCF](http://csharp-video-tutorials.blogspot.com/2013/11/part-1-introduction-to-wcf.html)

In this video we will discuss creating a simple **remoting and a web
service.**This is continuation to [Part
1](http://csharp-video-tutorials.blogspot.com/2013/11/part-1-introduction-to-wcf.html).
Please watch [Part
1](http://csharp-video-tutorials.blogspot.com/2013/11/part-1-introduction-to-wcf.html)
from [WCF video
tutorial](http://www.youtube.com/playlist?list=PL6n9fhu94yhVxEyaRMaMN_-qnDdNVGsL1)
before proceeding. 

## We have 2 clients and we need to implement a service a for them.
**1.** The first client is using a **Java application**to interact with
our service, so for interoperability this client wants messages to be in
**XML format**and the protocol to be **HTTP.**

**2.** The second client uses **.NET**, so for better performance this
client wants messages formatted in **binary **over **TCP**protocol.
 
To satisfy the requirement of the first client let's create a web
service. Web services use HTTP protocol and XML message format. So
interoperability requirement of the first client will be met. Web
services can be consumed by any client built on any platform.

### To create the web service
**1.** Create an **empty asp.net web application**project and name it
**WebServicesDemo.**

**2.** Right click on the project name in solution explorer and add a web
service. Name it **HelloWebService**.

**3.** Copy and paste the following code ```HelloWebService.asmx.cs```.
```C#
 using System.Web.Services;
 namespace WebServicesDemo
 {
     [WebService(Namespace = "http://pragimtech.com/WebServices")]
     [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
     [System.ComponentModel.ToolboxItem(false)]
     [System.Web.Script.Services.ScriptService]
     public class HelloWebService : System.Web.Services.WebService
     {
         [WebMethod]
         public string GetMessage(string name)
         {
             return "Hello " + name;
         }
     }
 }
```
 **Build the solution.**
 
### Creating a client for the Web Service

**1.** Right click on **WebServicesDemo**solution in solution explorer
and add a new asp.net empty web application project and name it
**HelloWebApplication**.

**2.** Right click on **References**folder in
**HelloWebApplication**project and select **Add Service
Reference**option. In **Add Service Reference**dialog box click the
**Discover**button. In the **namespace**textbox type
**HelloWebService**and click **OK**. This should generate a proxy class
to invoke the HelloWebService.

**3.** Right click on **HelloWebApplication**project name in solution
explorer and a new web form. This should add **WebForm1.aspx**

**4.** Copy and paste the following HTML on **WebForm1.aspx**

```html 
<table style="font-family:Arial">
     <tr>
         <td>
             <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
             <asp:Button ID="Button1" runat="server" Text="Get Message" onclick="Button1\_Click" />
         </td>
     </tr>
     <tr>
         <td>
             <asp:Label ID="Label1" runat="server" Font-Bold="true">
             </asp:Label>
         </td>        
     </tr>
 </table>
```
 **5.** Copy and paste the following code in **WebForm1.aspx.cs**
```c#
 protected void Button1_Click(object sender, EventArgs e)
 {
     HelloWebService.HelloWebServiceSoapClient client = 
		new HelloWebService.HelloWebServiceSoapClient();
 
     Label1.Text = client.GetMessage(TextBox1.Text);
 }
 ```
 The ASP.NET web application is now able to communicate with the web
service. Not just asp.net, a JAVA application can also consume the web
service.
 
![WebService + .NET Remoting + WCF tutorial](https://www.youtube.com/watch?v=3Qt7TTS1u4A&list=PL6n9fhu94yhVxEyaRMaMN_-qnDdNVGsL1&index=2)

Based on PRAGIM TECHNOLOGIES' [blog](http://csharp-video-tutorials.blogspot.cz/).
