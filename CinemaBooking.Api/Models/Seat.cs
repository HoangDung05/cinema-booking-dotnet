namespace CinemaBooking.Api.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;


    }
}
