using Microsoft.EntityFrameworkCore;
using Postbucket.Models;

namespace Postbucket
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
        {
            
        }
        public System.Data.Entity.DbSet<FormData> FormData { get; set; }
    }
}