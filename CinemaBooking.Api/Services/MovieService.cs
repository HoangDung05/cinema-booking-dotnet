using CinemaBooking.Api.Data;
using CinemaBooking.Api.Models;
using Microsoft.EntityFrameworkCore;
using CinemaBooking.Api.Exceptions;
using MySql.Data.MySqlClient;

namespace CinemaBooking.Api.Services
{
    public class MovieService : IMovieService
    {
        private readonly CinemaDbContext _context;
        public MovieService(CinemaDbContext context)
        {
            _context = context;
        }
        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movies.AsNoTracking().ToListAsync();
        }
        public async Task<Movie?> GetMovieByIdAsync(int id)
        {
            return await _context.Movies.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<List<Movie>> GetNowShowingMoviesAsync()
        {
            return await _context.Movies.AsNoTracking().Where(movie => movie.Status == MovieStatus.NOW_SHOWING).ToListAsync();
        }
        public async Task<List<Movie>> GetComingSoonMoviesAsync()
        {
            return await _context.Movies.AsNoTracking().Where(movie => movie.Status == MovieStatus.COMING_SOON).ToListAsync();
        }
        public async Task<List<Movie>> SearchMoviesAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<Movie>();
            keyword = keyword.Trim();
            return await _context.Movies.AsNoTracking().Where(movie => movie.Title.Contains(keyword)).ToListAsync();
        }
        public async Task<List<Movie>> FilterMoviesAsync(MovieStatus? status, DateOnly? date)
        {
            IQueryable<Movie> query = _context.Movies.AsNoTracking();
            if (status.HasValue)
                query = query.Where(movie => movie.Status == status.Value);
            if (date.HasValue)
                query = query.Where(movie => movie.ReleaseDate == date.Value);
            return await query.ToListAsync();
        }

        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie?> UpdateMovieAsync(int id, Movie movie)
        {
            var existingMovie = await _context.Movies.FindAsync(id);
            if (existingMovie == null)
                return null;
            existingMovie.Title = movie.Title;
            existingMovie.Description = movie.Description;
            existingMovie.Duration = movie.Duration;
            existingMovie.ReleaseDate = movie.ReleaseDate;
            existingMovie.PosterUrl = movie.PosterUrl;
            existingMovie.TrailerUrl = movie.TrailerUrl;
            existingMovie.Status = movie.Status;

            await _context.SaveChangesAsync();
            return existingMovie;
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return false;
            _context.Movies.Remove(movie);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is MySqlException mysqlEx && mysqlEx.Number == 1451)
            {
                throw new ResourceConflictException("Cannot delete the movie because it is referenced by other records.");
            }
          
            return true;
        }
    }
}
