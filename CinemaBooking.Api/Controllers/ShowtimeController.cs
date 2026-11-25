using CinemaBooking.Api.DTOs.Showtimes;
using CinemaBooking.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBooking.Api.Controllers
{
    [ApiController]
    [Route("showtimes")]
    public class ShowtimeController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;
        public ShowtimeController(IShowtimeService showtimeService)
        {
            _showtimeService = showtimeService;
        }
        [HttpGet]
        public async Task<ActionResult<List<ShowtimeResponse>>> GetShowtimes()
        {
            var showtimes = await _showtimeService.GetAllShowtimesAsync();
            var response = showtimes.Select(s => new ShowtimeResponse
            {
                Id = s.Id,
                MovieTitle = s.Movie.Title,
                RoomName = s.Room.Name,
                CinemaName = s.Room.Cinema.Name,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Price = s.Price
            }).ToList();
            return Ok(response);
        }
    }
}
