using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Api.DTOs.Showtimes
{
    public class UpdateShowtimeRequest
    {
        [Range(1, int.MaxValue)]
        public int MovieId { get; set; }
        [Range(1, int.MaxValue)]
        public int RoomId { get; set; }
        [Required]
        public DateTime? StartTime { get; set; }
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }
    }
}
