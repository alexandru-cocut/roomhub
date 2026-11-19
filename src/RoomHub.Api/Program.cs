using System.Text.Json.Serialization;
using Asp.Versioning;
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

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("x-api-version")
    );
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomHubDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => "RoomHub API is running.");

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .ReportApiVersions()
    .Build();

app.MapRoomEndpoints("/api/rooms")
    .WithApiVersionSet(apiVersionSet)
    .HasApiVersion(new ApiVersion(1, 0));

app.MapBookingEndpoints("/api/bookings")
    .WithApiVersionSet(apiVersionSet)
    .HasApiVersion(new ApiVersion(1, 0));

app.MapRoomEndpoints("/api/v{version:apiVersion}/rooms")
    .WithApiVersionSet(apiVersionSet)
    .HasApiVersion(new ApiVersion(1, 0));

app.MapBookingEndpoints("/api/v{version:apiVersion}/bookings")
    .WithApiVersionSet(apiVersionSet)
    .HasApiVersion(new ApiVersion(1, 0));

app.Run();
