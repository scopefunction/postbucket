using System;
using System.Collections.Generic;

namespace Postbucket.BLL
{
    public class EmailRecipient
    {
        public static void SendEmail(Dictionary<string, string> form)
        {
            var recipient = "";
            try
            {
                 recipient = form?["recipient"];
            }
            catch (Exception e)
            {
                var innerException = new Exception().InnerException;
                if (innerException != null) throw innerException;
            }

            if (form == null) return;
            
            form.Remove("recipient");

            string emailValues = "";

            foreach (var value in form)
            {
                emailValues += $"{value.Key} {value.Value}";
            }

            EmailService.Send(emailValues, recipient);
        }
    }
}