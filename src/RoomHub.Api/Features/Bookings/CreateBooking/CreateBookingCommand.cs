namespace RoomHub.Api.Features.Bookings.CreateBooking;

public record CreateBookingRequest(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

public record CreateBookingCommand(Guid RoomId, string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

public record BookingCreatedResponse(Guid Id, Guid RoomId, string Status);
