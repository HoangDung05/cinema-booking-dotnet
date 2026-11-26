namespace CinemaBooking.Api.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; } = null!;
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
