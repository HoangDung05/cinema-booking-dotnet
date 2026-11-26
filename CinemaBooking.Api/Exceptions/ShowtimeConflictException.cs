namespace CinemaBooking.Api.Exceptions
{
    public class ShowtimeConflictException : Exception
    {
        public ShowtimeConflictException(string message) : base(message)
        {
        }
    }
}
