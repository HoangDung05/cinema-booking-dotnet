namespace CinemaBooking.Api.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int ShowtimeId { get; set; }
        public decimal? TotalPrice { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.PENDING;
        public DateTime? CreatedAt { get; set; }
        public int? VoucherId { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string? GuestEmail { get; set; }
        public Showtime Showtime { get; set; } = null!;
    }
}
