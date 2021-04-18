using System.Collections.Generic;

namespace Postbucket.Models
{
    public class FormSubmission
    {
        private Dictionary<string, string> _submissionValues;
        public FormSubmission()
        {
            _submissionValues = new Dictionary<string, string>();
        }

        public void AddToSubmissions(string key, string value)
        {
            _submissionValues.Add(key, value);
        }

        public void Remove(string key)
        {
            _submissionValues.Remove(key);
        }

        public Dictionary<string, string> Return()
        {
            return _submissionValues;
        }
    }
}