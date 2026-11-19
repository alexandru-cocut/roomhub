using RoomHub.Api.Common;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public static class CreateRoomEndpoint
{
    public static void MapCreateRoomEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateRoomCommand command,
            ICommandHandler<CreateRoomCommand, Result<RoomResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(command, cancellationToken);
            if (result.IsFailure)
            {
                return result.Error.ToProblemDetails();
            }

            return Results.Created($"/api/rooms/{result.Value.Id}", result.Value);
        });
    }
}

