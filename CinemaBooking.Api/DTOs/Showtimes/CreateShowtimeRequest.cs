using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.Api.DTOs.Showtimes
{
    public class CreateShowtimeRequest
    {
        [Range(1, int.MaxValue)]
        public int MovieId { get; set; }

        [Range(1, int.MaxValue)]
        public int RoomId { get; set; }

        [Required]
        public DateTime? StartTime { get; set; }

        [Range(typeof(decimal), "0.01", "10000000")]
        public Decimal Price { get; set; }
    }
}
