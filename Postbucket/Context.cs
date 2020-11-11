using System.Data.Entity;
using Postbucket.Models;

namespace Postbucket
{
    public class Context : DbContext
    {
        public DbSet<FormData> FormData { get; set; }
    }
}