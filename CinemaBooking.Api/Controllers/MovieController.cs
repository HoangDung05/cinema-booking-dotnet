using Microsoft.AspNetCore.Mvc;
using CinemaBooking.Api.Models;
using CinemaBooking.Api.DTOs.Movies;
using CinemaBooking.Api.Exceptions;
using CinemaBooking.Api.Interfaces;

namespace CinemaBooking.Api.Controllers;

[ApiController]
[Route("movies")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }
    [HttpGet]
    public async Task<ActionResult<List<Movie>>> GetMovies([FromQuery] MovieStatus? status, [FromQuery] DateOnly? date)
    {
        var movies = await _movieService.FilterMoviesAsync(status, date);
        return Ok(movies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Movie>> GetMovieById(int id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);
        if (movie == null)
        {
            return NotFound();
        }
        return Ok(movie);
    }

    [HttpGet("now-showing")]
    public async Task<ActionResult<List<Movie>>> GetNowShowingMovies()
    {
        var movies = await _movieService.GetNowShowingMoviesAsync();
        return Ok(movies);
    }

    [HttpGet("coming-soon")]
    public async Task<ActionResult<List<Movie>>> GetComingSoonMovies()
    {
        var movies = await _movieService.GetComingSoonMoviesAsync();
        return Ok(movies);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<Movie>>> SearchMovies([FromQuery] string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest("Keyword cannot be null or whitespace.");
        keyword = keyword.Trim();
        var movies = await _movieService.SearchMoviesAsync(keyword);
        return Ok(movies);
    }

    [HttpPost]
    public async Task<ActionResult<Movie>> CreateMovie([FromBody] CreateMovieRequest request)
    {
        Movie movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            Duration = request.Duration,
            ReleaseDate = request.ReleaseDate,
            PosterUrl = request.PosterUrl,
            TrailerUrl = request.TrailerUrl,
            Status = request.Status!.Value
        };
        var createdMovie = await _movieService.CreateMovieAsync(movie);
        return CreatedAtAction(nameof(GetMovieById), new {id = createdMovie.Id}, createdMovie);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Movie>> UpdateMovie(int id, [FromBody] CreateMovieRequest request)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            Duration = request.Duration,
            ReleaseDate = request.ReleaseDate,
            PosterUrl = request.PosterUrl,
            TrailerUrl = request.TrailerUrl,
            Status = request.Status!.Value
        };

        var updatedMovie = await _movieService.UpdateMovieAsync(id, movie);
        if (updatedMovie == null)
        {
            return NotFound();
        }
        return Ok(updatedMovie);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        try
        {
            var result = await _movieService.DeleteMovieAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (ResourceConflictException ex)
        {
            return Conflict(new {message = ex.Message});
        }
    }
}
