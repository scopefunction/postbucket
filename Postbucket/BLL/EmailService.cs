using System;
using System.Net;
using System.Net.Mail;
using RestSharp;
using RestSharp.Authenticators;

namespace Postbucket.BLL
{
    public class EmailService
    {
        public static void Send(string body, string recipient)
        {
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("ec6aeb3a72f130", "f54b3cbfad5e87"),
                EnableSsl = true
            };
            
            client.Send("mail@mergedigital.io", recipient, "New Form Submission", body);
        }
     
        public static IRestResponse SendSimpleEmail(string body, string recipient)
        {
            RestClient client = new RestClient ();
            client.BaseUrl = new Uri ("https://api.mailgun.net/v3/sandbox9c2e9e7b1d7843da86746125faf83950.mailgun.org");
            
            client.Authenticator =
            new HttpBasicAuthenticator("api", Environment.GetEnvironmentVariable("MAILGUN_API_KEY") ?? string.Empty);
            RestRequest request = new RestRequest ();
            request.AddParameter ("domain", "mail.merge.africa", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter ("from", "Excited User <mailgun@mail@merge.africa>");
            request.AddParameter ("to", recipient);
            request.AddParameter ("subject", "Hello");
            request.AddParameter ("text", body);
            request.Method = Method.POST;
            return client.Execute (request);
        }
    }
}
