using RoomHub.Api.Common;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public static class GetBookingEndpoint
{
    public static void MapGetBookingEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
            Guid id,
            IQueryHandler<GetBookingQuery, Result<BookingDetailsResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetBookingQuery(id), cancellationToken);
            if (result.IsFailure)
            {
                return result.Error.ToProblemDetails();
            }

            return Results.Ok(result.Value);
        });
    }
}
