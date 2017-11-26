### Part 3 - Creating a wcf service {.post-title .entry-title itemprop="name"}

**Suggested Videos** \
 [Part 1 - Introduction to WCF](http://csharp-video-tutorials.blogspot.com/2013/11/part-1-introduction-to-wcf.html)

 [Part 2 - Creating a remoting service and a web service](http://csharp-video-tutorials.blogspot.com/2013/11/part-2-creating-remoting-service-and_17.html)
 
This is continuation to [Part2](http://csharp-video-tutorials.blogspot.com/2013/11/part-2-creating-remoting-service-and_17.html).
Please watch [Part2](http://csharp-video-tutorials.blogspot.com/2013/11/part-2-creating-remoting-service-and_17.html)
from [WCF videotutorial](http://www.youtube.com/playlist?list=PL6n9fhu94yhVxEyaRMaMN_-qnDdNVGsL1)
before proceeding. 

## In this video, we will discuss 
1. Creating a WCF service
2. Hosting the WCF service using a console application
3. Exposing 2 service endpoints. 
4. Creating a windows and a web Client applications.
 
## Let's take the scenario that we discussed in [Part 2](http://csharp-video-tutorials.blogspot.com/2013/11/part-2-creating-remoting-service-and_17.html)
We have 2 clients and we need to implement a service a for them. 

1. The first client is using a Java application to interact with our
service, so for interoperability this client wants meesages to be in XML
format and the protocl to be HTTP.
2. The second client uses .NET, so for better performance this
client wants messages formmated in binary over TCP protocol.
 
In **[Part 2](http://csharp-video-tutorials.blogspot.com/2013/11/part-2-creating-remoting-service-and_17.html)**,\
To meet the requirement of the first client, we implemented a **web service**and to meet the requirement of the second client we implemented a **remoting service**.

In this video, we will create a single **WCF service**, and **configure 2 endpoints** to meet the requirements of both the clients.

### Creating the WCF Service:

1. Create a new Class Library Project and name it **HelloService**.
2. Delete **Class1.cs** file that is auto-generated.
3. Add a new **WCF Service**with name = **HelloService**. This
should automatically generate 2 files (**HelloService.cs** &
**IHelloService.cs**). Also a reference to
**System.ServiceModel** assembly is added.
4. Copy and paste the following code in **IHelloService.cs** file

```C#
 using System.ServiceModel;
 namespace HelloService
 {
    
[ServiceContract(Namespace="http://PragimTech.com/ServiceVersion1")]
     public interface IHelloService
     {
         [OperationContract]
         string GetMessage(string name);
     }
 }
```
5. Copy and paste the following code in **HelloService.cs**
```C#
 namespace HelloService
 {
     public class HelloService : IHelloService
     {
         public string GetMessage(string name)
         {
             return "Hello " + name;
         }
     }
 }
 
```
 That's it we are done implementing a WCF Service. The next step is to
**host the service** using a **console application**. A WCF service can
also be hosted in a **Windows application**, or **Windows Service** or
**IIS**. We will discuss these hosting options in a later video session.
 
### Hosting the WCF service using a console application
1. Right click on **HelloService** solution in Solution Explorer and
add a new Console Application project with name = **HelloServiceHost**
2. Add a reference to **System.ServiceModel** assembly and
**HelloService**project
3. Right click on **HelloServiceHost** project and add **Application
Configuration File**. This should add **App.config** file to the project.
Copy and paste the following XML. Notice that we have specified **2
endpoints** in the configuration. One endpoint uses
**basicHttpBinding**, which communicates over **HTTP** protocol using
**XML** messages. This endpoint will satisfy the requirement of the first
client. The other endpoint uses **netTcpBinding**, which communicates
over **TCP** protocol using **binary messages**. This endpoint will
satisfy the requirement of the second client. 

```html
 <?xml version="1.0" encoding="utf-8" ?>
 <configuration>
   <system.serviceModel>
     <behaviors>
       <serviceBehaviors>
         <behavior name="mexBehaviour">
           <serviceMetadata httpGetEnabled="true" />
         </behavior>
       </serviceBehaviors>
     </behaviors>
     <services>
       <service name="HelloService.HelloService" behaviorConfiguration="mexBehaviour">
         <endpoint address="HelloService" binding="basicHttpBinding" contract="HelloService.IHelloService" />
         <endpoint address="HelloService" binding="netTcpBinding" contract="HelloService.IHelloService" />
         <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange" />
         <host>
           <baseAddresses>
             <add baseAddress="http://localhost:8080/" />
             <add baseAddress="net.tcp://localhost:8090" />
           </baseAddresses>
         </host>
       </service>
     </services>
   </system.serviceModel>
 </configuration>
```
 **4.** Copy and paste the following code in **Program.cs** file

```C#
 using System;
 namespace HelloServiceHost
 {
     class Program
     {
         static void Main()
         {
             using(var host = new System.ServiceModel.ServiceHost(typeof(HelloService.HelloService)))
             {
                 host.Open();
                 Console.WriteLine("Host started @ " + DateTime.Now.ToString());
                 Console.ReadLine();
             }
         }
     }
 }
```
Build the solution. Set **HelloServiceHost** as startup project and run
it by pressing **CTRL + F5** keys.

Now let's build a web application that is going to **consume the WCF
service**using the endpoint with **basicHttpBinding**. basicHttpBinding
communicates over HTTP protocol using XML messages.

1. Create a **new asp.net empty web application** and name it **HelloWebClient**
2. Right click on **References** folder and select **Add Service
Reference** option. In the address textbox type
**http://localhost:8080/** and click on **GO** button. In the namespace
textbox type **HelloService**and click **OK**. This should generate a
proxy class to communicate with the service.
3. Add a new webform. Copy and paste the following HTML in
WebForm1.aspx\

```HTML
 <div style="font-family:Arial">
     <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
     <asp:Button ID="Button1" runat="server" Text="Get Message" \
         onclick="Button1\_Click" />
     <br />
     <asp:Label ID="Label1" runat="server" Font-Bold="true"></asp:Label>
 </div>
```
 **4.** Copy and paste the following code in WebForm1.aspx.cs file\
```C#
 protected void Button1_Click(object sender, EventArgs e)
 {
     var client = new HelloService.HelloServiceClient("BasicHttpBinding_IHelloService");
     Label1.Text = client.GetMessage(TextBox1.Text);
 } 
```
Now let's build a **windows application** that is going to **consume the
WCF service** using the endpoint with **netTcpBinding**. netTcpBinding
communicated over TCP protocol using binary messages.
1. Create a new Windows Forms application and name it
**HelloWindowsClient**\
2. Right click on **References**folder and select **Add Service
Reference**option. In the address textbox type
**http://localhost:8080/** and click on **GO**button. In the namespace
textbox type **HelloService**and click **OK.**This should generate a
proxy class to communicate with the service.\
3. On **Form1**, drag and drop a textbox, a button and a label
control. Double click the button to generate the click event handler.\
4. Copy and paste the following code in Form1.cs file

```C#
 private void button1_Click(object sender, EventArgs e)
 {
     var client = new HelloService.HelloServiceClient("NetTcpBinding_IHelloService");
     label1.Text = client.GetMessage(textBox1.Text);
 }
```
 [![wcf tutorial](http://1.bp.blogspot.com/-uCLgi3fnHv4/UofgWLaI2-I/AAAAAAAAHfI/wqIjnIsaW14/s1600/WCF+Tutorial.png)](http://www.youtube.com/playlist?list=PL6n9fhu94yhVxEyaRMaMN_-qnDdNVGsL1)

Based on PRAGIM TECHNOLOGIES' [blog](http://csharp-video-tutorials.blogspot.cz/).