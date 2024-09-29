using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceY.RestApi.Contracts.Requests;
using SpaceY.RestApi.Database;
using SpaceY.RestApi.Shared;

namespace SpaceY.RestApi.Features.Chats;

public static class ChangeChatsArchivedStatus
{
    public class Command : IRequest<Result>
    {
        public Guid ChatId { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ChatId).NotEmpty();
        }
    }


    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly IValidator<Command> _validator;
        private readonly AppDbContext _dbContext;

        public Handler(IValidator<Command> validator,
            AppDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(new Error(
                   "CreateChat.Validation",
                   validationResult.ToString()));
            }

            var chat = await _dbContext.Chats.FirstOrDefaultAsync(c => c.Id == request.ChatId);

            chat!.IsArchived = !chat.IsArchived;

            await _dbContext.SaveChangesAsync();

            return Result.Success();    
        }
    }

}


public class ChangeIsArchivedEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("chats/{id:guid}/change-archived", async ([FromRoute] Guid id, ISender sender) =>
        {
            var command = new ChangeChatsArchivedStatus.Command {  ChatId = id };

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok();
        })
        .RequireAuthorization()
        .WithTags("Chats");

    }
}
