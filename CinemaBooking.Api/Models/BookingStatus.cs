using System.Text.Json.Serialization;

namespace CinemaBooking.Api.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter<BookingStatus>))]
    public enum BookingStatus
    {
        PENDING,
        AWAITING_CONFIRMATION,
        PAID,
        CANCELLED
    }
}
