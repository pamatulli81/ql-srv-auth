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
    public static class AuthTicket
    {
        public static string GetTicket(string method, string server, string virtualProxy, string user, string userdirectory, string [] certs)
        {
           
            //PAM: Ignore Certificate Error Message for Self Signed Certificates
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string url = "https://" + server + ":4243/qps/" + virtualProxy + "/ticket";

            string Xrfkey = Util.GenerateXrfKey();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?Xrfkey=" + Xrfkey);
            request = Util.AddCertificates(request, certs);

            request.Method = method;
            request.Accept = "application/json";
            request.Headers.Add("X-Qlik-Xrfkey", Xrfkey);

            string body = "{ 'UserId':'" + user + "','UserDirectory':'" + userdirectory + "',";
            body += "'Attributes': [],";
            body += "}";
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

            if (!string.IsNullOrEmpty(body))
            {
                request.ContentType = "application/json";
                request.ContentLength = bodyBytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                requestStream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            return stream != null ? new StreamReader(stream).ReadToEnd() : string.Empty;

        }
    }
}