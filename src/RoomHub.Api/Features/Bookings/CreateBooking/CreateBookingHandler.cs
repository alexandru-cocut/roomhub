using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public class CreateBookingHandler(
    IRoomRepository roomRepository,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<CreateBookingCommand, Result<BookingCreatedResponse>>
{
    public async Task<Result<BookingCreatedResponse>> HandleAsync(CreateBookingCommand command, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
        {
            return Result.Failure<BookingCreatedResponse>(Error.NotFound("RoomNotFound", "The room does not exist."));
        }

        var bookingResult = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            timeProvider);

        if (bookingResult.IsFailure)
        {
            return Result.Failure<BookingCreatedResponse>(bookingResult.Error);
        }

        var booking = bookingResult.Value;

        var existingBookings = await bookingRepository.ListForRoomAsync(command.RoomId, cancellationToken);
        foreach (var existing in existingBookings)
        {
            if (existing.OverlapsWith(booking.StartUtc, booking.EndUtc))
            {
                return Result.Failure<BookingCreatedResponse>(Error.Conflict("RoomOccupied", "The room is occupied in this interval."));
            }
        }

        await bookingRepository.AddAsync(booking, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new BookingCreatedResponse(booking.Id, booking.RoomId, booking.Status.ToString()));
    }
}

