using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public class Booking
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    private Booking() { }

    public Booking(
        Guid id,
        Guid roomId,
        string bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        BookingStatus status)
    {
        Id = id;
        RoomId = roomId;
        BookedBy = bookedBy;
        StartUtc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
        EndUtc = DateTime.SpecifyKind(endUtc, DateTimeKind.Utc);
        Attendees = attendees;
        Status = status;
    }

    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        TimeProvider timeProvider)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))
            return Result.Failure<Booking>(Error.Validation("InvalidBookedBy", "BookedBy is required."));

        startUtc = DateTime.SpecifyKind(startUtc.ToUniversalTime(), DateTimeKind.Utc);
        endUtc = DateTime.SpecifyKind(endUtc.ToUniversalTime(), DateTimeKind.Utc);

        if (endUtc <= startUtc)
            return Result.Failure<Booking>(Error.Validation("InvalidRange", "The end must be after the start."));

        if (attendees <= 0)
            return Result.Failure<Booking>(Error.Validation("InvalidAttendees", "Attendees must be positive."));

        if (attendees > room.Capacity)
            return Result.Failure<Booking>(Error.Validation("AttendeesExceedCapacity", "The room does not have enough seats."));

        var now = timeProvider.GetUtcNow().UtcDateTime;
        if (startUtc < now)
            return Result.Failure<Booking>(Error.Validation("InvalidStart", "Booking cannot start in the past."));

        // R6 & R10: Rooms with capacity > 10 can be booked for at most 2 hours; <= 10 at most 4 hours.
        var maxDuration = room.Capacity > 10 ? TimeSpan.FromHours(2) : TimeSpan.FromHours(4);
        if ((endUtc - startUtc) > maxDuration)
        {
            var msg = room.Capacity > 10
                ? "Rooms with a capacity greater than 10 can be booked for at most 2 hours."
                : "Booking duration cannot exceed 4 hours.";
            return Result.Failure<Booking>(Error.Validation("DurationExceeded", msg));
        }

        return Result.Success(new Booking(
            Guid.NewGuid(),
            room.Id,
            bookedBy.Trim(),
            startUtc,
            endUtc,
            attendees,
            BookingStatus.Active));
    }

    public Result Cancel(TimeProvider timeProvider)
    {
        if (Status == BookingStatus.Cancelled)
            return Result.Failure(Error.Conflict("AlreadyCancelled", "The booking is already cancelled."));

        // R11: A booking can be cancelled only if it starts at least 60 minutes from now.
        var now = timeProvider.GetUtcNow().UtcDateTime;
        if (StartUtc < now.AddMinutes(60))
            return Result.Failure(Error.Conflict("CancellationWindowClosed", "A booking can only be cancelled at least 60 minutes before its start time."));

        Status = BookingStatus.Cancelled;
        return Result.Success();
    }

    public bool OverlapsWith(DateTime start, DateTime end)
    {
        if (Status != BookingStatus.Active)
            return false;

        return StartUtc < end && start < EndUtc;
    }
}
