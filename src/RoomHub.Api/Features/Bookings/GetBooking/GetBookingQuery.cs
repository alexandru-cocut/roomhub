namespace RoomHub.Api.Features.Bookings.GetBooking;

public record GetBookingQuery(Guid Id);

public record BookingDetailsResponse(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    string Status);
