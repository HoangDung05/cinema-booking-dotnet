using CinemaBooking.Api.Models;

namespace CinemaBooking.Api.Interfaces
{
    public interface IShowtimeService
    {
        Task<List<Showtime>> GetAllShowtimesAsync();
    }
}
