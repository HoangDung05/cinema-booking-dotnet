using System.Text.Json.Serialization;

namespace CinemaBooking.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter<MovieStatus>))]
    public enum MovieStatus
    {
        COMING_SOON,
        NOW_SHOWING,
        ENDED
    }
}
