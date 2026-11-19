namespace RoomHub.Api.Features.Rooms.GetRoom;

public record GetRoomQuery(Guid Id);

public record GetRoomResponse(Guid Id, string Name, int Capacity);

