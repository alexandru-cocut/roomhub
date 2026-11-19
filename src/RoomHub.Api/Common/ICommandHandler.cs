namespace RoomHub.Api.Common;

public interface ICommandHandler<in TCommand, TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand>
{
    Task<Domain.Common.Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
