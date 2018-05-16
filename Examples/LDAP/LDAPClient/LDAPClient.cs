using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LDAPClient
{
    public class LDAPClient : IDisposable
    {
        private LdapConnection _connection;

        public void Bind(string distinguishedName, string password, string url, AuthType authType)
        {
            var credentials = new NetworkCredential(distinguishedName, password);
            var serverId = new LdapDirectoryIdentifier(url);

            _connection = new LdapConnection(serverId, credentials, authType);
            _connection.SessionOptions.ProtocolVersion = 3;
            _connection.Bind();
        }

        /// <summary>
        ///     Performs a search in the LDAP server. This method is generic in its return value to show the power
        ///     of searches. A less generic search method could be implemented to only search for users, for instance.
        /// </summary>
        /// <param name="baseDn">The distinguished name of the base node at which to start the search</param>
        /// <param name="ldapFilter">An LDAP filter as defined by RFC4515</param>
        /// <returns>A flat list of dictionaries which in turn include attributes and the distinguished name (DN)</returns>
        public List<Dictionary<string, string>> Search(string baseDn, string ldapFilter)
        {
            var request = new SearchRequest(baseDn, ldapFilter, SearchScope.Subtree, null);
            var response = (SearchResponse)_connection.SendRequest(request);

            var result = new List<Dictionary<string, string>>();

            if (response?.Entries == null) return result;

            foreach (SearchResultEntry entry in response?.Entries)
            {
                var dic = new Dictionary<string, string> { ["DN"] = entry.DistinguishedName };

                var attributesAttributeNames = entry?.Attributes?.AttributeNames;
                if (attributesAttributeNames != null)
                    foreach (string attrName in attributesAttributeNames)
                        //For simplicity, we ignore multi-value attributes
                        dic[attrName] = string.Join(",", entry.Attributes[attrName].GetValues(typeof(string)));

                result.Add(dic);
            }

            return result;
        }

        /// <summary>
        ///     Adds a user to the LDAP server database. This method is intentionally less generic than the search one to
        ///     make it easier to add meaningful information to the database.
        /// </summary>
        /// <param name="user">The user to add</param>
        public bool AddUser(UserModel user)
        {
            var sha1 = new SHA1Managed();
            var digest = Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(user.UserPassword)));

            var request = new AddRequest(user.DN,
                new DirectoryAttribute("cn", user.CN),
                new DirectoryAttribute("description", user.Description),
                new DirectoryAttribute("userPassword", "{SSHA}" + digest),
                new DirectoryAttribute("objectClass", "simpleSecurityObject", "organizationalRole"));
            try
            {
                var response = _connection.SendRequest(request);
                return response != null && response.ResultCode == ResultCode.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     This method shows how to delete anything by its distinguished name (DN).
        /// </summary>
        /// <param name="dn">Distinguished name of the entry to delete</param>
        public bool Delete(string dn)
        {
            var request = new DeleteRequest(dn);

            try
            {
                var response = _connection.SendRequest(request);
                return response != null && response.ResultCode == ResultCode.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
