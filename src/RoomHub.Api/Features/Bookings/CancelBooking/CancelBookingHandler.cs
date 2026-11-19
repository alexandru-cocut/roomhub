using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public class CancelBookingHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CancelBookingCommand>
{
    public async Task<Result> HandleAsync(CancelBookingCommand command, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(command.Id, cancellationToken);
        if (booking is null)
        {
            return Result.Failure(Error.NotFound("BookingNotFound", "The booking does not exist."));
        }

        var cancelResult = booking.Cancel();
        if (cancelResult.IsFailure)
        {
            return cancelResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
