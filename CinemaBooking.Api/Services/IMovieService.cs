using System;
using CinemaBooking.Models;

namespace CinemaBooking.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAllMoviesAsync();
        Task<Movie?> GetMovieByIdAsync(int id);
        Task<List<Movie>> GetNowShowingMoviesAsync();
        Task<List<Movie>> GetComingSoonMoviesAsync();
        Task<List<Movie>> SearchMoviesAsync(string keyword);
        Task<List<Movie>> FilterMoviesAsync(MovieStatus? status, DateOnly? date);
        Task<Movie> CreateMovieAsync(Movie movie);
        Task<Movie?> UpdateMovieAsync(int id, Movie movie);
        Task<bool> DeleteMovieAsync(int id);
    }
}
