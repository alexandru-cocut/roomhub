using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Features.Bookings;
using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBooking;
using RoomHub.Api.Features.Rooms;
using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoom;
using RoomHub.Api.Infrastructure.Persistence;
using RoomHub.Api.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton(TimeProvider.System);

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "roomhub.db");
builder.Services.AddDbContext<RoomHubDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RoomHubDbContext>());
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddScoped<ICommandHandler<CreateRoomCommand, Result<RoomResponse>>, CreateRoomHandler>();
builder.Services.AddScoped<IQueryHandler<GetRoomQuery, Result<GetRoomResponse>>, GetRoomHandler>();
builder.Services.AddScoped<ICommandHandler<CreateBookingCommand, Result<BookingCreatedResponse>>, CreateBookingHandler>();
builder.Services.AddScoped<IQueryHandler<GetBookingQuery, Result<BookingDetailsResponse>>, GetBookingHandler>();
builder.Services.AddScoped<ICommandHandler<CancelBookingCommand>, CancelBookingHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomHubDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => "RoomHub API is running.");
app.MapRoomEndpoints();
app.MapBookingEndpoints();

app.Run();
