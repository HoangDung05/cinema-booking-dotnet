using CinemaBooking.Api.Data;
using CinemaBooking.Api.DTOs.Showtimes;
using CinemaBooking.Api.Interfaces;
using CinemaBooking.Api.Models;
using CinemaBooking.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        private async Task<bool> MovieExistsAsync(int movieId)
        {
            return await _context.Movies.AnyAsync(m => m.Id == movieId);
        }

        private async Task<bool> RoomExistsAsync(int roomId)
        {
            return await _context.Rooms.AnyAsync(r => r.Id == roomId);
        }

        private async Task<bool> HasScheduleConflictAsync(int roomId, DateTime newStart, DateTime newEnd, int? excludeShowtimeId = null)
        {
            var schedules = await _context.Showtimes
                .Where(s => s.RoomId == roomId && s.StartTime < newEnd &&( s.Id != excludeShowtimeId || excludeShowtimeId == null ))
                .Select(s => new
                {
                    s.StartTime,
                    s.EndTime,
                    Duration = s.Movie.Duration
                })
                .AsNoTracking()
                .ToListAsync();

            return schedules.Any(s =>
            {
                if (s.EndTime == null && s.Duration <= 0)
                    throw new InvalidOperationException(
                        "Existing showtime has invalid movie duration");

                DateTime existingEnd = s.EndTime
                    ?? s.StartTime.AddMinutes(s.Duration);

                return newStart < existingEnd &&
                       newEnd > s.StartTime;
            });
        }

        private async Task<DateTime?> CalculateEndTimeAsync(int movieId, DateTime startTime)
        {
            var movie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(s => s.Id == movieId);
            if (movie == null)
                return null;
            if (movie.Duration <= 0)
                throw new InvalidOperationException("Movie duration must be greater than 0.");
            return startTime.AddMinutes(movie.Duration);
        }

        public async Task<Showtime> CreateShowtimeAsync(CreateShowtimeRequest request)
        {
            if (!await MovieExistsAsync(request.MovieId))
                throw new ArgumentException("Movie not found");
            if (!await RoomExistsAsync(request.RoomId))
                throw new ArgumentException("Room not found");
            DateTime endTime = (await CalculateEndTimeAsync(
                request.MovieId,
                request.StartTime!.Value))
                ?? throw new ArgumentException("Movie not found");
   
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted);
            Showtime showtime;
            
            try
            {
                await LockRoomAsync(request.RoomId);

                if (await HasScheduleConflictAsync(request.RoomId, request.StartTime.Value, endTime))
                {
                    throw new ShowtimeConflictException("Showtime schedule conflict");
                }

                showtime = new Showtime
                {
                    MovieId = request.MovieId,
                    RoomId = request.RoomId,
                    StartTime = request.StartTime!.Value,
                    EndTime = endTime,
                    Price = request.Price
                };

                _context.Showtimes.Add(showtime);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            return await _context.Showtimes
                    .Include(s => s.Movie)
                    .Include(s => s.Room)
                    .ThenInclude(s => s.Cinema)
                    .AsNoTracking()
                    .SingleAsync(s => s.Id == showtime.Id);
        }

        private async Task LockRoomAsync(int roomId)
        {
            await _context.Rooms.FromSqlInterpolated($"SELECT * FROM rooms WHERE id = {roomId} FOR UPDATE").SingleAsync();
        }

        private async Task<Showtime?> LockShowtimeAsync(int showtimeId)
        {
            return await _context.Showtimes.FromSqlInterpolated($"SELECT * FROM showtimes WHERE id = {showtimeId} FOR UPDATE").FirstOrDefaultAsync();
        }

        public async Task<Showtime?> UpdateShowtimeAsync(int id, UpdateShowtimeRequest request)
        {
            if (!await MovieExistsAsync(request.MovieId))
                throw new ArgumentException("Movie not found");
            if (!await RoomExistsAsync(request.RoomId))
                throw new ArgumentException("Room not found");
            DateTime endTime = (await CalculateEndTimeAsync(
                request.MovieId,
                request.StartTime!.Value))
                ?? throw new ArgumentException("Movie not found");
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var existingShowtime = await LockShowtimeAsync(id);

                if (existingShowtime == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }
                await LockRoomAsync(request.RoomId);
                if (await HasScheduleConflictAsync(request.RoomId, request.StartTime.Value, endTime, id))
                {
                    throw new ShowtimeConflictException("Showtime schedule conflict");
                }
                existingShowtime.MovieId = request.MovieId;
                existingShowtime.RoomId = request.RoomId;
                existingShowtime.StartTime = request.StartTime!.Value;
                existingShowtime.EndTime = endTime;
                existingShowtime.Price = request.Price;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            return await _context.Showtimes
                    .Include(s => s.Movie)
                    .Include(s => s.Room)
                    .ThenInclude(s => s.Cinema)
                    .AsNoTracking()
                    .SingleAsync(s => s.Id == id);
        }
    }
}
