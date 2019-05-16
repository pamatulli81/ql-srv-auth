using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace DatabridgeQlikConnect.Qlik
{
    public static class AuthHeader
    {
        public static HttpWebResponse GetHeaderResponse(string method, string server, string virtualProxy, string hdrName, string user, string userdirectory)
        {
            string url = "https://" + server + "/" + virtualProxy + "/hub";

            string Xrfkey = Util.GenerateXrfKey();

            //PAM: Ignore Certificate Error Message
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?Xrfkey=" + Xrfkey);
            request.Method = method;
            request.Accept = "application/json";
            request.Headers.Add("X-Qlik-Xrfkey", Xrfkey);
            request.Headers.Add(hdrName, userdirectory + "\\" + user);

            // make the web request and return the content
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;

        }
    }
}