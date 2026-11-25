namespace CinemaBooking.Api.DTOs.Showtimes
{
    public class ShowtimeResponse
    {
        public int Id { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string CinemaName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal Price { get; set; }

    }
}
