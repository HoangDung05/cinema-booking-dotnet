namespace CinemaBooking.Api.Models
{
    public class Showtime
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal Price { get; set; }
        public Movie Movie { get; set; } = null!;
        public Room Room { get; set; } = null!;
        
    }
}
