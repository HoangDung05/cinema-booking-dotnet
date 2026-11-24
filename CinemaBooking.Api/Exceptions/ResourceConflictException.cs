namespace CinemaBooking.Api.Exceptions
{
    public class ResourceConflictException : Exception
    {
        public ResourceConflictException(string message) : base(message)
        {
        }
    }
}
