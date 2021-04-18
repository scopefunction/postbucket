using System;
using System.Net;
using System.Net.Mail;
using RestSharp;
using RestSharp.Authenticators;

namespace Postbucket.BLL
{
    public class EmailService
    {
        public static void Send(string body, string sender)
        {
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("ec6aeb3a72f130", "f54b3cbfad5e87"),
                EnableSsl = true
            };
            
            client.Send("mail@mergedigital.io", sender, "New Form Submission", body);
        }
     
        public static IRestResponse SendSimpleEmail(string body, string sender)
        {
            RestClient client = new RestClient ();
            client.BaseUrl = new Uri ("https://api.mailgun.net/v3/sandbox9c2e9e7b1d7843da86746125faf83950.mailgun.org");
            client.Authenticator =
            new HttpBasicAuthenticator ("api",
                "1ab3bc0a5acb8a2fc03680c600ba789a-1b6eb03d-13f17028");
            RestRequest request = new RestRequest ();
            request.AddParameter ("domain", "mail.merge.africa", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter ("from", "Excited User <mailgun@mail@merge.africa>");
            request.AddParameter ("to", sender);
            request.AddParameter ("subject", "Hello");
            request.AddParameter ("text", body);
            request.Method = Method.POST;
            return client.Execute (request);
        }
        
    }
}