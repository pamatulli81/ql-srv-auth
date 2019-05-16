using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace DatabridgeQlikConnect.Qlik
{
    public static class Util
    {

        public static string GenerateXrfKey()
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var chars = new char[16];
            var rd = new Random();

            for (int i = 0; i < 16; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public static HttpWebRequest AddCertificates(HttpWebRequest request, string[] certs)
        {

            if(certs.Length > 0)
            {
                foreach(var cert in certs) { 
                    X509Certificate2 _cert = new X509Certificate2(cert);
                    request.ClientCertificates.Add(_cert);
                }
            }
            else
            {
                X509Certificate2 _pfxCert = null;
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                _pfxCert = store.Certificates.Cast<X509Certificate2>().FirstOrDefault(c => c.FriendlyName == "QlikClient");
                store.Close();
                request.ClientCertificates.Add(_pfxCert);
            }   
         

            return request;

        }
    }
}