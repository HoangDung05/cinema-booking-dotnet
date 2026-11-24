using Microsoft.EntityFrameworkCore;
using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace CinemaBooking.Api.Data
{
    public class CinemaDbContext : DbContext
    {
        
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options)
        {
            
        }
        public DbSet<Movie> Movies { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.ToTable("movies");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Id).ValueGeneratedOnAdd();

                entity.Property(s => s.Title).HasMaxLength(255).IsRequired();

                entity.Property(s => s.Duration).IsRequired();

                entity.Property(s => s.Description).HasColumnType("TEXT").IsRequired(false);

                var dateConverter = new ValueConverter<DateOnly, DateTime>(
                    date => date.ToDateTime(TimeOnly.MinValue),
                    dateTime => DateOnly.FromDateTime(dateTime)
                );
                entity.Property(s => s.ReleaseDate).HasColumnType("date").HasColumnName("release_date").IsRequired(false).HasConversion(dateConverter);
                

                entity.Property(s => s.PosterUrl).HasMaxLength(255).HasColumnName("poster_url").IsRequired(false);

                entity.Property(s => s.TrailerUrl).HasMaxLength(255).HasColumnName("trailer_url").IsRequired(false);

                entity.Property(s => s.Status).HasConversion<string>().HasDefaultValue(MovieStatus.COMING_SOON);
            });
        }
    }
}
