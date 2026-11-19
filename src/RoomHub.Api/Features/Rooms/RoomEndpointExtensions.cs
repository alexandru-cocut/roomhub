using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoom;

namespace RoomHub.Api.Features.Rooms;

public static class RoomEndpointExtensions
{
    public static void MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rooms").WithTags("Rooms");
        group.MapCreateRoomEndpoint();
        group.MapGetRoomEndpoint();
        group.MapCreateBookingEndpoint();
    }
}
