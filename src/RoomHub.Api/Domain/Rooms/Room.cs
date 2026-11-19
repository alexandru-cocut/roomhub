using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Rooms;

public class Room
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }

    private Room() { }

    public Room(Guid id, string name, int capacity)
    {
        Id = id;
        Name = name;
        Capacity = capacity;
    }

    public static Result<Room> Create(string? name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Room>(Error.Validation("InvalidName", "Room name is required."));

        if (capacity <= 0)
            return Result.Failure<Room>(Error.Validation("InvalidCapacity", "Capacity must be positive."));

        return Result.Success(new Room(Guid.NewGuid(), name.Trim(), capacity));
    }
}
