using RoomHub.Api.Common;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    public static void MapGetRoomEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetRoomQuery, Result<GetRoomResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetRoomQuery(id), cancellationToken);
            if (result.IsFailure)
            {
                return result.Error.ToProblemDetails();
            }

            return Results.Ok(result.Value);
        });
    }
}
