using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace testAgileMapper
{
//dotnet ef migrations add init

//dotnet ef database update

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(a =>
            {
                a.ToTable(nameof(User).ToLowerInvariant());
                a.HasKey(b => b.Id);

                a.Property(b => b.Id).HasColumnName("id");
                a.Property(b => b.Name).HasColumnName("name");

                a.HasData(Data());
            });
        }

        private static IEnumerable<User> Data()
        {
            yield return new User
            {
                Id = Guid.NewGuid(),
                Name = "abc"
            };

            yield return new User
            {
                Id = Guid.NewGuid(),
                Name = "cde"
            };
        }
    }

    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
        }
    }
}