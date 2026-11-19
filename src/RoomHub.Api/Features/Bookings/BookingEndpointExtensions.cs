using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.GetBooking;

namespace RoomHub.Api.Features.Bookings;

public static class BookingEndpointExtensions
{
    public static RouteGroupBuilder MapBookingEndpoints(this IEndpointRouteBuilder app, string prefix = "/api/bookings")
    {
        var group = app.MapGroup(prefix).WithTags("Bookings");
        group.MapGetBookingEndpoint();
        group.MapCancelBookingEndpoint();
        return group;
    }
}

