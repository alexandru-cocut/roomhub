using RoomHub.Api.Common;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public static class CreateBookingEndpoint
{
    public static void MapCreateBookingEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/{roomId:guid}/bookings", async (
            Guid roomId,
            CreateBookingRequest request,
            ICommandHandler<CreateBookingCommand, Result<BookingCreatedResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateBookingCommand(
                roomId,
                request.BookedBy,
                request.StartUtc,
                request.EndUtc,
                request.Attendees);

            var result = await handler.HandleAsync(command, cancellationToken);
            if (result.IsFailure)
            {
                return result.Error.ToProblemDetails();
            }

            return Results.Created($"/api/bookings/{result.Value.Id}", result.Value);
        });
    }
}

