
using Microsoft.EntityFrameworkCore;

namespace XVagas.Models{
    public class DatabaseContext: DbContext{
        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
        {
        }

        public DbSet<User> Users {get; set;}
    }
}