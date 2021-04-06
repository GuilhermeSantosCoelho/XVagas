
using System;
using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using XVagas.Globals;

namespace XVagas.Entity{
    public class DatabaseContext: DbContext{
        public IConfiguration Configuration { get; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
        {
        }

        public DatabaseContext()        
        { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            if (!optionsBuilder.IsConfigured)
            {
                Console.WriteLine(Globals.Globals.Configuration.GetSection("ConnectionString"));
                optionsBuilder.UseMySql(Globals.Globals.Configuration.GetSection("ConnectionString").Value);
            }
        }

        public DbSet<User> Users {get; set;}
        public DbSet<FilePDF> Files {get; set;}
    }
}