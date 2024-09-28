using Carter;
using FluentValidation;
using Flurl.Http;
using Mapster;
using MediatR;
using SpaceY.RestApi.Contracts.Dtos;
using SpaceY.RestApi.Contracts.Requests;
using SpaceY.RestApi.Database;
using SpaceY.RestApi.Entities;
using SpaceY.RestApi.Services;
using SpaceY.RestApi.Shared;

namespace SpaceY.RestApi.Features.Chats;

public static class CreateChat
{
    public class Command : IRequest<Result<Guid>>
    {
        public string Url { get; set; } = default!;
        public string Title { get; set; } = default!;
        public MessageDto Message { get; set; } = default!;
    }
     
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Url)
                .NotEmpty();

            RuleFor(x => x.Title)
               .NotEmpty();

            RuleFor(x => x.Message)
                .NotEmpty();
        }
    }


    internal sealed class Handler : IRequestHandler<Command, Result<Guid>>
    {
        private readonly IUserContext _userContext;
        private readonly IValidator<Command> _validator;
        private readonly AppDbContext _dbContext;
        private readonly IPythonService _pythonService;

        public Handler(IUserContext userContext,
            IValidator<Command> validator,
            AppDbContext dbContext,
            IPythonService pythonService)
        {
            _userContext = userContext;
            _validator = validator;
            _dbContext = dbContext;
            _pythonService = pythonService;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(new Error(
                   "CreateChat.Validation",
                   validationResult.ToString()));
            }

            var userId = _userContext.GetCurrentUserId();


            var chat = new Chat
            {
                ModifiedAt = DateTime.UtcNow,
                Title = request.Title,
                Id = Guid.NewGuid(),
                Url = request.Url,
                UserId = userId!,
            };

            var result = await _pythonService.CreateChatAsync(chat.Id, request.Url);

            if(result.status == "error")
            {
                return Result.Failure<Guid>(new Error(
                 "CreateChat.ExternalApiError",
                 "Some error occured."));
            }

            var message = new Message
            {
                ChatId = chat.Id,
                Content = request.Message.Content,
                IsFromUser = request.Message.IsFromUser
            };

            var response = await _pythonService.SendMessageAsync(request.Message.Content, chat.Id);

            if(response.Status == "error")
            {
                return Result.Failure<Guid>(new Error(
                 "CreateChat.ResponseFromPthonFailed",
                 "Some error occured."));
            }

            var responseMessage = new Message
            {
                ChatId = chat.Id,
                Content = response.Content!,
                IsFromUser = false,
                Id = Guid.NewGuid()
            };


            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _dbContext.Chats.AddAsync(chat, cancellationToken);
                await _dbContext.Messages.AddAsync(message, cancellationToken);
                await _dbContext.Messages.AddAsync(responseMessage, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return Result.Success(chat.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure<Guid>(new Error(
                   "CreateChat.TransactionFailed",
                   ex.Message));
            }
        }

    }   
}

public class CreateChatEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("chats", async (CreateChatRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateChat.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(new { chatId = result.Value});
        })
        .RequireAuthorization()
        .WithTags("Chats");

    }
}
