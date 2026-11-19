namespace RoomHub.Api.Features.Rooms.CreateRoom;

public record CreateRoomCommand(string? Name, int Capacity);

public record RoomResponse(Guid Id, string Name, int Capacity);
