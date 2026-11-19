using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoom;

namespace RoomHub.Api.Features.Rooms;

public static class RoomEndpointExtensions
{
    public static RouteGroupBuilder MapRoomEndpoints(this IEndpointRouteBuilder app, string prefix = "/api/rooms")
    {
        var group = app.MapGroup(prefix).WithTags("Rooms");
        group.MapCreateRoomEndpoint();
        group.MapGetRoomEndpoint();
        group.MapCreateBookingEndpoint();
        return group;
    }
}

