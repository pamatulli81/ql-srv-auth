using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace DatabridgeQlikConnect.Qlik
{
    public static class AuthQlikSession
    {

        public static string GetSession(string method, string server, string virtualProxy, string user, string userdirectory)
        {
            X509Certificate2 certificateFoo = null;
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            certificateFoo = store.Certificates.Cast<X509Certificate2>().FirstOrDefault(c => c.FriendlyName == "QlikClient");
            store.Close();

            //PAM: Ignore Certificate Error Message for Self Signed Certificates
            ServicePointManager.ServerCertificateValidationCallback = delegate {
                return true;
            };

            string url = "https://" + server + ":4243/qps/" + virtualProxy + "/session";

            string Xrfkey = Util.GenerateXrfKey();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?Xrfkey=" + Xrfkey);
            request.ClientCertificates.Add(certificateFoo);
            request.Method = method;
            request.Accept = "application/json";
            request.Headers.Add("X-Qlik-Xrfkey", Xrfkey);

            string body = "{ 'UserId':'" + user + "','UserDirectory':'" + userdirectory + "',";
            body += "'Attributes': [],";
            body += "'SessionId': '" + GetNewSessionId() + "'";
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

        private static string GetNewSessionId()
        {
            //Create SessionID
            SessionIDManager Manager = new SessionIDManager();
            string NewID = Manager.CreateSessionID(HttpContext.Current);
            string OldID = HttpContext.Current.Session.SessionID;
            bool redirected = false;
            bool IsAdded = false;
            Manager.SaveSessionID(HttpContext.Current, NewID, out redirected, out IsAdded);
            return NewID;
        }

    }
}