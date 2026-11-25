using Microsoft.EntityFrameworkCore;
using CinemaBooking.Api.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace CinemaBooking.Api.Data
{
    public class CinemaDbContext : DbContext
    {
        
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options)
        {
            
        }
        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Cinema> Cinemas { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Showtime> Showtimes { get; set; } = null!;

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

            modelBuilder.Entity<Cinema>(entity =>
            {
                entity.ToTable("cinemas");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                entity.Property(s => s.Name).HasMaxLength(100).IsRequired();
                entity.Property(s => s.Address).HasMaxLength(255).IsRequired(false);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("rooms");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                entity.Property(s => s.Name).HasMaxLength(50).IsRequired();
                entity.Property(s => s.CinemaId).HasColumnName("cinema_id").IsRequired();
                entity.HasOne(s => s.Cinema)
                      .WithMany(c => c.Rooms)
                      .HasForeignKey(s => s.CinemaId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(s => new { s.CinemaId, s.Name }).IsUnique();
            });

            modelBuilder.Entity<Showtime>(entity =>
            {
                entity.ToTable("showtimes");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                entity.Property(s => s.MovieId).HasColumnName("movie_id").IsRequired();
                entity.Property(s => s.RoomId).HasColumnName("room_id").IsRequired();
                entity.Property(s => s.StartTime).HasColumnName("start_time").HasColumnType("datetime").IsRequired();
                entity.Property(s => s.EndTime).HasColumnName("end_time").HasColumnType("datetime(6)").IsRequired(false);
                entity.Property(s => s.Price).HasColumnName("price").HasColumnType("decimal(38,2)").IsRequired();
                entity.HasOne(s => s.Movie)
                      .WithMany()
                      .HasForeignKey(s => s.MovieId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(s => s.Room)
                      .WithMany(r => r.Showtimes)
                      .HasForeignKey(s => s.RoomId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(s => new { s.RoomId, s.StartTime }).IsUnique();
            });
        }

        
    }
}
