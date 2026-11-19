using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.GetBooking;

namespace RoomHub.Api.Features.Bookings;

public static class BookingEndpointExtensions
{
    public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookings").WithTags("Bookings");
        group.MapGetBookingEndpoint();
        group.MapCancelBookingEndpoint();
    }
}
