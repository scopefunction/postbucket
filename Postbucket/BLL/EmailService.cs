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
            var client = new SmtpClient("smtp.hostinger.com", 587)
            {
                Credentials = new NetworkCredential("info@merge.africa", Environment.GetEnvironmentVariable("EMAIL_PASSWORD")),
                EnableSsl = false
            };
            
            client.Send("info@merge.africa", recipient, "New Form Submission", body);
        }
     
        public static IRestResponse SendSimpleEmail(string body, string recipient)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri("https://api.mailgun.net/v3/sandbox9c2e9e7b1d7843da86746125faf83950.mailgun.org"),
                Authenticator = new HttpBasicAuthenticator("api",
                    Environment.GetEnvironmentVariable("MAILGUN_API_KEY") ?? string.Empty)
            };

            var request = new RestRequest ();
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
