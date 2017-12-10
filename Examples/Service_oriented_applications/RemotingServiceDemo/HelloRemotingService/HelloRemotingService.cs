using System;
using RemotingServiceInterfaces;

namespace HelloRemotingService
{
    public class HelloRemotingService : MarshalByRefObject, IHelloRemotingService
    {
        public string GetMessage(string name)
        {
            return $"Hello {name}";
        }
    }
}