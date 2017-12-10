# Remoting service 

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

## To satisfy the requirement of the second client let's create a .NET remoting service.
**Creating a remoting service**

**1.** Create a new **Class Library project**and name it
**IHelloRemotingService.**

**2.** Rename **Class1.cs**file to **IHelloRemotingService.cs.** Copy
and paste the following code in **IHelloRemotingService.cs**file.

```C#
 namespace IHelloRemotingService
 {
     public interface IHelloRemtingService
     {
         string GetMessage(string name);
     }
 }
```
**3.** Right click on **IHelloRemotingService**solution in solution
explorer and add new class library project, and name it
**HelloRemotingService**.

**4.** We want to use interface **IHelloRemotingService**in
**HelloRemotingService**project. So add a reference
to **IHelloRemotingService** project.

**5.** Rename **Class1.cs** file to **HelloRemotingService.cs.**Copy and
paste the following code in **HelloRemotingService.cs** file.

```C#
 using System;
 namespace HelloRemotingService
 {
     public class HelloRemotingService : MarshalByRefObject,
         IHelloRemotingService.IHelloRemtingService
     {
         public string GetMessage(string name)
         {
             return "Hello " + name;
         }
     }
 }
```
**6.** Now we need to **host the remoting service**. To host it let's use
a **console application**. A windows application or IIS can also be used
to host the remoting service. Right click on
**IHelloRemotingService**solution in solution explorer and add **new
Console Application  project**, and name it **RemotingServiceHost.**

**7.** Add a reference to **IHelloRemotingService** and
**HelloRemotingService** projects and
**System.Runtime.Remoting** assembly.

**8.** Copy and paste the following code in **Program.cs** file
```C#
 using System;
 using System.Runtime.Remoting;
 using System.Runtime.Remoting.Channels;
 using System.Runtime.Remoting.Channels.Tcp;
 
 namespace RemotingServiceHost
 {
     class Program
     {
         static void Main()
         {
             HelloRemotingService.HelloRemotingService remotingService =
                 new HelloRemotingService.HelloRemotingService();
             TcpChannel channel = new TcpChannel(8080);
             ChannelServices.RegisterChannel(channel, false);
             RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(HelloRemotingService.HelloRemotingService),
                "GetMessage",
                WellKnownObjectMode.Singleton);
             Console.WriteLine("Host started @ " + DateTime.Now.ToString());
             Console.WriteLine("Press ANY key to continue...");
             Console.ReadKey();
         }
     }
 }
```
**9.** Now we need to create the **client for our remoting
service**. Let's use **windows application as the client**. Right click on
**IHelloRemotingService** solution in solution explorer and **add new
windows application**.

**10.** Add a reference to **IHelloRemotingService**project and
**System.Runtime.Remoting**assembly.

**11.** Drag and drop a **textbox, button**and a **label**control on
**Form1**in the windows application.

**12.** Double click the button to generate the **click event
handler**. Copy and paste the following code in **Form1.cs.**

```C#
 using System;
 using System.Runtime.Remoting.Channels;
 using System.Runtime.Remoting.Channels.Tcp;
 using System.Windows.Forms;
 
 namespace HelloRemotingServiceClient
 {
     public partial class MainView : Form
     {
        public IHelloRemotingService Client { get; }
        public MainView()
        {
            this.InitializeComponent();
            var channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            this.Client = (RemotingServiceInterfaces.IHelloRemotingService)
                Activator.GetObject(
                    typeof(RemotingServiceInterfaces.IHelloRemotingService),
                    "tcp://localhost:8080/GetMessage"
                );
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
     }
 }
 
```
 By this point you may have already realized how different web service
and remoting programming models are. In Part 3, we will discuss
implementing a **single WCF service**that can satisfy the requirements
of both the clients.
 
 Can we use .NET Remoting to build a Web service? 
Yes.
 
 If the client is not built using .NET platform, will they be able to
consume a .NET Remoting web service?
 They can, but we need to be very careful in choosing the data types
that we use in the service, and client-activated objects and events
should be avoided.  But keep in mind .NET Remoting is not meant for
implementing interoperable services. 
 
 If your goal is to build interoperable services use ASP.NET Web
Services. But with introduction of WCF, both .NET Remoting and ASP.NET
Web Services are legacy technologies.  
 
![WebService + .NET Remoting + WCF tutorial](https://www.youtube.com/watch?v=3Qt7TTS1u4A&list=PL6n9fhu94yhVxEyaRMaMN_-qnDdNVGsL1&index=2)

Based on PRAGIM TECHNOLOGIES' [blog](http://csharp-video-tutorials.blogspot.cz/).