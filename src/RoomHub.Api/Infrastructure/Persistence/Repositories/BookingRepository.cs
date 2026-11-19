using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

public class BookingRepository(RoomHubDbContext dbContext) : IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> ListForRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var bookings = await dbContext.Bookings
            .Where(b => b.RoomId == roomId)
            .ToListAsync(cancellationToken);

        return bookings;
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await dbContext.Bookings.AddAsync(booking, cancellationToken);
    }
}
