namespace CinemaBooking.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Duration { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public string? PosterUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public MovieStatus Status { get; set; } = MovieStatus.COMING_SOON;
        public Movie(int id, string title, string? description, int duration, DateOnly? releaseDate, string? posterUrl, string? trailerUrl, MovieStatus status)
        {
            Id = id;
            Title = title;
            Description = description;
            Duration = duration;
            ReleaseDate = releaseDate;
            PosterUrl = posterUrl;
            TrailerUrl = trailerUrl;
            Status = status;
        }
        public Movie()
        {
        }
    }
}
