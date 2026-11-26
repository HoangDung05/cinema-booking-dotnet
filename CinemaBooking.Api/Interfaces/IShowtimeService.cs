using CinemaBooking.Api.DTOs.Showtimes;
using CinemaBooking.Api.Models;

namespace CinemaBooking.Api.Interfaces
{
    public interface IShowtimeService
    {
        Task<List<Showtime>> GetAllShowtimesAsync();
        Task<Showtime> CreateShowtimeAsync(CreateShowtimeRequest request);
        Task<Showtime?> UpdateShowtimeAsync(int id, UpdateShowtimeRequest request);
        Task<bool> DeleteShowtimeAsync(int id); 
    }
}
