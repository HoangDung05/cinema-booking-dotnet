using CinemaBooking.Api.DTOs.Showtimes;
using CinemaBooking.Api.Exceptions;
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
        
        [HttpPost]
        public async Task<ActionResult<ShowtimeResponse>> CreateShowtime([FromBody] CreateShowtimeRequest request)
        {
            try
            {
                var showtime = await _showtimeService.CreateShowtimeAsync(request);
                var response = new ShowtimeResponse
                {
                    Id = showtime.Id,
                    StartTime = showtime.StartTime,
                    EndTime = showtime.EndTime,
                    Price = showtime.Price,
                    MovieTitle = showtime.Movie.Title,
                    RoomName = showtime.Room.Name,
                    CinemaName = showtime.Room.Cinema.Name
                };
                return StatusCode(201, response);
            }
            catch (ShowtimeConflictException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ShowtimeResponse>> UpdateShowtime(int id, [FromBody] UpdateShowtimeRequest request)
        {
            try
            {
                var showtime = await _showtimeService.UpdateShowtimeAsync(id, request);
                if (showtime == null)
                {
                    return NotFound();
                }
                var response = new ShowtimeResponse
                {
                    Id = showtime.Id,
                    StartTime = showtime.StartTime,
                    EndTime = showtime.EndTime,
                    Price = showtime.Price,
                    MovieTitle = showtime.Movie.Title,
                    RoomName = showtime.Room.Name,
                    CinemaName = showtime.Room.Cinema.Name
                };
                return Ok(response);
            }
            catch (ShowtimeConflictException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteShowtime(int id)
        {
            var showtime = await _showtimeService.DeleteShowtimeAsync(id);
            if (!showtime)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
