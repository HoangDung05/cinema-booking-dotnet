using System.ComponentModel.DataAnnotations;
using CinemaBooking.Api.Models;
namespace CinemaBooking.Api.DTOs.Movies
{
    public class CreateMovieRequest
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(1, int.MaxValue)]
        public int Duration { get; set; }

        public DateOnly? ReleaseDate { get; set; }

        [StringLength(255)]
        public string? PosterUrl { get; set; } = string.Empty;

        [StringLength(255)]
        public string? TrailerUrl { get; set; } = string.Empty;

        [Required]
        public MovieStatus? Status { get; set; }
    }
}
