using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Common;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Error error)
    {
        return Results.Problem(
            detail: error.Message,
            statusCode: error.StatusCode,
            title: error.Code);
    }
}

