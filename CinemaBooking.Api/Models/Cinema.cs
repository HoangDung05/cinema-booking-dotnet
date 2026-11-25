namespace CinemaBooking.Api.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();

    }
}
