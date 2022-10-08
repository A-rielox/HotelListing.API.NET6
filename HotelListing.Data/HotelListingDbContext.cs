using HotelListing.API.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HotelListing.API.Data
{
    //public class HotelListingDbContext : DbContext
    // al cambiarme a IdentityDbContext debo migrar otra vez
    public class HotelListingDbContext : IdentityDbContext<ApiUser>
    {
        // estas options son las q paso en Program.cs
        public HotelListingDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        //
        // para el seeding de la DB
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // el seeding de los roles, con lo de RoleConfiguration.cs
            // al poner uno nuevo de estos tengo q migrar
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            modelBuilder.ApplyConfiguration(new CountryConfiguration());

            modelBuilder.ApplyConfiguration(new HotelConfiguration());
        }
    }

    public class HotelListingDbContextFactory : IDesignTimeDbContextFactory<HotelListingDbContext>
    {
        public HotelListingDbContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<HotelListingDbContext>();
            var conn = config.GetConnectionString("HotelListingDbConnectionString");
            optionsBuilder.UseSqlServer(conn);
            return new HotelListingDbContext(optionsBuilder.Options);
        }
    }
}


// EL SEEDING ACA MISMO

//protected override void OnModelCreating(ModelBuilder modelBuilder)
//{
//    base.OnModelCreating(modelBuilder);
//
//    modelBuilder.Entity<Country>().HasData(
//        new Country
//        {
//            Id = 1,
//            Name = "Jamaica",
//            ShortName = "JM"
//        },
//        new Country
//        {
//            Id = 2,
//            Name = "Bahamas",
//            ShortName = "BS"
//        },
//        new Country
//        {
//            Id = 3,
//            Name = "Cayman Island",
//            ShortName = "CI"
//        }
//    );
//
//    modelBuilder.Entity<Hotel>().HasData(
//        new Hotel
//        {
//            Id = 1,
//            Name = "Sandals Resort and Spa",
//            Address = "Negril",
//            CountryId = 1,
//            Rating = 4.5
//        },
//        new Hotel
//        {
//            Id = 2,
//            Name = "Comfort Suites",
//            Address = "George Town",
//            CountryId = 3,
//            Rating = 4.3
//        },
//        new Hotel
//        {
//            Id = 3,
//            Name = "Grand Palldium",
//            Address = "Nassua",
//            CountryId = 2,
//            Rating = 4
//        }
//    );
//}
//