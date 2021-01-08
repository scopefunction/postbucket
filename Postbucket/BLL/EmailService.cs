using System;
using RestSharp;
using RestSharp.Authenticators;

namespace Postbucket.BLL
{
    public class EmailService
    {
        public static void Send(string body, string recipient)
        {
            Console.WriteLine(SendSimpleEmail(body, recipient).StatusCode);
        }
     
        public static IRestResponse SendSimpleEmail(string body, string recipient)
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
            request.AddParameter ("to", recipient);
            request.AddParameter ("subject", "Hello");
            request.AddParameter ("text", body);
            request.Method = Method.POST;
            return client.Execute (request);
        }
        
    }
}