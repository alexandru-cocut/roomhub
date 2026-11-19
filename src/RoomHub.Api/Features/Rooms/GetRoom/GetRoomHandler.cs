using RoomHub.Api.Common;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public class GetRoomHandler(IRoomRepository roomRepository)
    : IQueryHandler<GetRoomQuery, Result<GetRoomResponse>>
{
    public async Task<Result<GetRoomResponse>> HandleAsync(GetRoomQuery query, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(query.Id, cancellationToken);
        if (room is null)
        {
            return Result.Failure<GetRoomResponse>(Error.NotFound("RoomNotFound", "The room does not exist."));
        }

        return Result.Success(new GetRoomResponse(room.Id, room.Name, room.Capacity));
    }
}

