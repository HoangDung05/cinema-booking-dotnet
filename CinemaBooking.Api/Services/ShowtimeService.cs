using CinemaBooking.Api.Data;
using CinemaBooking.Api.Interfaces;
using CinemaBooking.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Api.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly CinemaDbContext _context;

        public ShowtimeService(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Showtime>> GetAllShowtimesAsync()
        {
            return await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ThenInclude(r => r.Cinema)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
