using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public class GetBookingHandler(IBookingRepository bookingRepository)
    : IQueryHandler<GetBookingQuery, Result<BookingDetailsResponse>>
{
    public async Task<Result<BookingDetailsResponse>> HandleAsync(GetBookingQuery query, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(query.Id, cancellationToken);
        if (booking is null)
        {
            return Result.Failure<BookingDetailsResponse>(Error.NotFound("BookingNotFound", "The booking does not exist."));
        }

        return Result.Success(new BookingDetailsResponse(
            booking.Id,
            booking.RoomId,
            booking.BookedBy,
            booking.StartUtc,
            booking.EndUtc,
            booking.Attendees,
            booking.Status.ToString()));
    }
}
