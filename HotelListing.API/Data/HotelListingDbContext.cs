using HotelListing.API.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
}
