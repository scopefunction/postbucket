using System;
using System.ComponentModel.DataAnnotations;

namespace Postbucket.Models
{
    public class FormData
    {
        [Key]
        public int Id { get; set; }
        
        [DataType(DataType.Text)]
        public string SerializedData { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}