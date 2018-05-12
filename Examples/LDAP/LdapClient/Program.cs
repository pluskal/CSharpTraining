using System;
using System.Linq;

namespace LdapClient
{
    internal class Program
    {
        private static readonly string _sampleUserName = "sampleUser";
        private static readonly string _plaintextPassword = "plaintextPassword";

        private static void Main(string[] args)
        {
            using (var client = CreateClient("cn=admin,dc=example,dc=org", "admin", "localhost:389"))
            {
                AddUser(client);

                SearchForAllUsers(client);

                ValidateCredentialsWithBind(client);

                DeleteUser(client);
            }
        }

        private static Client CreateClient(string distinguishedName, string password, string url)
        {
            var client = new Client(distinguishedName, password, url);
            Console.WriteLine($"Client created.");
            return client;
        }

        private static void DeleteUser(Client client)
        {
            try
            {
                client.Delete($"cn={_sampleUserName},dc=example,dc=org");
                Console.WriteLine($"User: {_sampleUserName} deleted successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"User: {_sampleUserName} deletion failed.");
                Console.WriteLine(e);
            }
        }

        private static void ValidateCredentialsWithBind(Client client)
        {
            //For this to work the server must be configured to map users correctly to its internal database
            if (client.ValidateUserByBind(_sampleUserName, _plaintextPassword))
            {
                Console.WriteLine("Valid credentials (bind)");
            }
            else
            {
                Console.WriteLine("Invalid credentials (bind)");
            }
        }

        private static void SearchForAllUsers(Client client)
        {
            try
            {
                var searchResult = client.Search("dc=example,dc=org", "objectClass=*");
                foreach (var d in searchResult)
                    Console.WriteLine(string.Join("\r\n", d.Select(x => x.Key + ": " + x.Value).ToArray()));
            }
            catch (Exception e)
            {
                Console.WriteLine($"LDAP search for all users failed.");
                Console.WriteLine(e);
            }
        }

        private static void AddUser(Client client)
        {
            try
            {
                client.AddUser(new UserModel($"cn={_sampleUserName},dc=example,dc=org", _sampleUserName, "users",
                    _plaintextPassword));
                Console.WriteLine($"User: {_sampleUserName} added successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"User: {_sampleUserName} addition failed.");
                Console.WriteLine(e);
            }
        }
    }
}