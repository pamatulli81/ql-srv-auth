using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;


namespace DatabridgeQlikConnect.Controllers
{
    [RoutePrefix("api/v0/qlik-db-auth")]
    public class QlikAuthController : ApiController
    {

        [Route("IsAlive")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult IsAlive()
        {

            return  Ok("I am alive!");

        }


        [Route("AuthQlikSession")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpGet]
        public async Task <IHttpActionResult> AuthQlikSession(string method, string server, string virtualProxy, string user, string userdirectory, string [] certs)
        {
            string session = Qlik.AuthQlikSession.GetSession(method, server, virtualProxy, user, userdirectory, certs);

            string[] getSessionArr = session.Split(new Char[] { ',' });
            string[] getSessionCode = getSessionArr[3].Split(new Char[] { ':' });

            DateTime now = DateTime.Now;
            var sessionId = getSessionCode[1].Trim(new Char[] { '"' });
            var QlikCookie = new CookieHeaderValue("X-Qlik-Session-" + virtualProxy, sessionId);
            QlikCookie.Expires = DateTime.MinValue;
            QlikCookie.HttpOnly = true;
            QlikCookie.Domain = getDomain(server);

            var resp = new HttpResponseMessage();
            resp.StatusCode = HttpStatusCode.OK;
            resp.Headers.AddCookies(new[] { QlikCookie });
            return ResponseMessage(resp);
        }

        [Route("AuthQlikTicket")]
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> AuthQlikTicket(string method, string server, string virtualProxy, string user, string userdirectory, string [] certs)
        {

            string ticket = Qlik.AuthTicket.GetTicket(method, server, virtualProxy, user, userdirectory, certs);
            string[] getTicket = ticket.Split(new Char[] { ',' });
            string[] getTicketCode = getTicket[3].Split(new Char[] { ':' });
            string r = getTicketCode[1].Trim(new Char[] { '"' });

            return Ok(r);

        }


        [Route("AuthQlikHeader")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> AuthQlikHeader(string method="GET", string server= "13.94.197.134", string virtualProxy="hdr", string hdrName= "hdr-user", string user="test2", string userdirectory="DB-I-QS-DEV", string redirectUrl= "https://13.94.197.134/hdr/hub")
        {
            var response = Qlik.AuthHeader.GetHeaderResponse(method, server, virtualProxy, hdrName, user, userdirectory);

            if (response.StatusCode == HttpStatusCode.OK)
                return Ok(response);
            else
                return StatusCode(HttpStatusCode.Forbidden); 
        }


        private string getDomain(string strServerAddress)
        {

            var pathElements = strServerAddress.Split('.');
            string result = null;
            for (int i = 1; i < pathElements.Length; i++)
            {
                result += pathElements[i] + ".";
            }
            result = result.TrimEnd(new[] { '.' });
            return result;
        }
    }
}