using RoomHub.Api.Common;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public class CreateRoomHandler(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateRoomCommand, Result<RoomResponse>>
{
    public async Task<Result<RoomResponse>> HandleAsync(CreateRoomCommand command, CancellationToken cancellationToken = default)
    {
        var roomResult = Room.Create(command.Name, command.Capacity);
        if (roomResult.IsFailure)
        {
            return Result.Failure<RoomResponse>(roomResult.Error);
        }

        var room = roomResult.Value;
        await roomRepository.AddAsync(room, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new RoomResponse(room.Id, room.Name, room.Capacity));
    }
}
