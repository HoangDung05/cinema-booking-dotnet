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
        public DbSet<Seat> Seats { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<BookingDetail> BookingDetails { get; set; } = null!;

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

            modelBuilder.Entity<Seat>(entity => 
            { 
                entity.ToTable("seats");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                entity.Property(s => s.SeatNumber).HasColumnName("seat_number").HasMaxLength(10).IsRequired();
                entity.Property(s => s.RoomId).HasColumnName("room_id").IsRequired();
                entity.HasOne(s => s.Room)
                      .WithMany(r => r.Seats)
                      .HasForeignKey(s => s.RoomId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(s => new { s.RoomId, s.SeatNumber }).IsUnique();
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("bookings");
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Id).ValueGeneratedOnAdd();
                entity.Property(b => b.UserId).HasColumnName("user_id").IsRequired(false);
                entity.Property(b => b.ShowtimeId).HasColumnName("showtime_id").IsRequired();
                entity.Property(b => b.TotalPrice).HasColumnName("total_price").HasColumnType("decimal(38,2)").IsRequired(false);
                entity.Property(b => b.Status).HasColumnName("status").HasConversion<string>().IsRequired().HasDefaultValue(BookingStatus.PENDING);
                entity.Property(b => b.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired(false);
                entity.Property(b => b.VoucherId).HasColumnName("voucher_id").IsRequired(false);
                entity.Property(b => b.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(38,2)").IsRequired(false);
                entity.Property(b => b.GuestEmail).HasColumnName("guest_email").HasMaxLength(255).IsRequired(false);
                entity.HasOne(b => b.Showtime)
                      .WithMany()
                      .HasForeignKey(b => b.ShowtimeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BookingDetail>(entity =>
            {
                entity.ToTable("booking_details");
                entity.HasKey(bd => bd.Id);
                entity.Property(bd => bd.Id).ValueGeneratedOnAdd();
                entity.Property(bd => bd.BookingId).HasColumnName("booking_id").IsRequired();
                entity.Property(bd => bd.SeatId).HasColumnName("seat_id").IsRequired();
                entity.Property(bd => bd.PriceAtBooking).HasColumnName("price_at_booking").HasColumnType("decimal(38,2)").IsRequired();
                entity.HasOne(bd => bd.Booking)
                      .WithMany()
                      .HasForeignKey(bd => bd.BookingId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(bd => bd.Seat)
                      .WithMany()
                      .HasForeignKey(bd => bd.SeatId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(bd => new { bd.BookingId, bd.SeatId }).IsUnique();
            });
        }

        
        
    }
}
