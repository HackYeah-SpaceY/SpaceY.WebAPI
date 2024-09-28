﻿using Carter;
using FluentValidation;
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
    public class Command : IRequest<Result>
    {
        public string Url { get; set; } = default!;
        public MessageDto Message { get; set; } = default!;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Url)
                .NotEmpty();

            RuleFor(x => x.Message)
                .NotEmpty();
        }
    }


    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly IUserContext _userContext;
        private readonly IValidator<Command> _validator;
        private readonly AppDbContext _dbContext;

        public Handler(IUserContext userContext,
            IValidator<Command> validator,
            AppDbContext dbContext)
        {
            _userContext = userContext;
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure(new Error(
                   "CreateChat.Validation",
                   validationResult.ToString()));
            }

            var userId = _userContext.GetCurrentUserId();

            var chat = new Chat
            {
                CreatedAt = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Url = request.Url,
                UserId = userId!,
            };

            var message = new Message
            {
                ChatId = chat.Id,
                Content = request.Message.Content,
                IsFromUser = request.Message.IsFromUser
            };

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _dbContext.Chats.AddAsync(chat, cancellationToken);
                await _dbContext.Messages.AddAsync(message, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure(new Error(
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

            return Results.Ok();
        })
        .RequireAuthorization()
        .WithTags("Chats");

    }
}
