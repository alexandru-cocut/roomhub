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
        // When the room does not exist, 404 is returned before any validation
        var room = await roomRepository.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
        {
            return Result.Failure<BookingCreatedResponse>(Error.NotFound("RoomNotFound", "The room does not exist."));
        }

        // Domain validation (R2, R3, R4, R5, R6, R10)
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

        // R9: Participant cannot have more than 2 active bookings per day across all rooms together.
        var activeBookingsToday = await bookingRepository.CountActiveForParticipantOnDateAsync(
            booking.BookedBy,
            booking.StartUtc,
            cancellationToken);

        if (activeBookingsToday >= 2)
        {
            return Result.Failure<BookingCreatedResponse>(
                Error.Conflict("DailyBookingLimitExceeded", "A participant cannot have more than 2 active bookings per day."));
        }

        // R7: Overlap check
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
