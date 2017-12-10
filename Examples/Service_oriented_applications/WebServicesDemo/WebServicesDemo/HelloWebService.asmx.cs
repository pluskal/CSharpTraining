using System.ComponentModel;
using System.Web.Services;

namespace WebServicesDemo
{
    /// <summary>
    ///     Summary description for HelloWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class HelloWebService : WebService
    {
        [WebMethod]
        public string GetMessage(string name)
        {
            return $"Hello {name}";
        }
    }
}