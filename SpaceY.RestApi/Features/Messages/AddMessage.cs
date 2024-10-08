﻿using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using SpaceY.RestApi.Contracts.Dtos;
using SpaceY.RestApi.Database;
using SpaceY.RestApi.Entities;
using SpaceY.RestApi.Services;
using Flurl;
using SpaceY.RestApi.Shared;
using System.Text.Json.Serialization;
using Flurl.Http;
using Mapster;
using Carter;
using SpaceY.RestApi.Features.Chats;
using SpaceY.RestApi.Contracts.Requests;
using SpaceY.RestApi.Contracts.Responses;

namespace SpaceY.RestApi.Features.Messages;

public static class AddMessage
{

    public class Command : IRequest<Result<AddMessageResponse>>
    {
        public Guid ChatId { get; set; }
        public string Content { get; set; } = default!;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ChatId)
                .NotEmpty();

            RuleFor(c => c.ChatId)
                .NotEmpty();
        }
    }


    internal sealed class Handler : IRequestHandler<Command, Result<AddMessageResponse>>
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserContext _userContext;
        private readonly IPythonService _pythonService;

        public Handler(AppDbContext dbContext,
            IUserContext userContext,
            IPythonService pythonService)
        {
            _dbContext = dbContext;
            _userContext = userContext;
            _pythonService = pythonService;
        }


        public async Task<Result<AddMessageResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = request.ChatId,
                Content = request.Content,
                IsFromUser = true
            };


            await _dbContext.Messages.AddAsync(message);

            var response = await _pythonService.SendMessageAsync(request.Content, request.ChatId);

            if (response.Status == "error")
            {
                return Result.Failure<AddMessageResponse>(new Error(
                 "CreateChat.ExternalApiError",
                 "Some error occured."));
            }


            var screenshot = new Screenshot
            {
                ChatId = request.ChatId,
                FilePath = response.ScreenshotPath!,
            };

            await _dbContext.Screenshots.AddAsync(screenshot);

            var responseMessage = new Message
            {
                ChatId = request.ChatId,
                Content = response.Content!,
                IsFromUser = false,
                Id = Guid.NewGuid()
            };

            await _dbContext.Messages.AddAsync(responseMessage);

            await _dbContext.SaveChangesAsync();

            var messages = await _dbContext.Messages.Where(m => m.ChatId == request.ChatId)
                .OrderBy(m => m.SentAt)
                .ToListAsync() ;

            var screenshots = await _dbContext.Screenshots.Where(s => s.ChatId == request.ChatId)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync();

            var messagesDtos = messages.Adapt<List<MessageDto>>();
            var screenshotsDtos = screenshots.Adapt<List<ScreenshotDto>>();

            var addMessageRespons = new AddMessageResponse
            {
                Messages = messagesDtos,
                Screenshots = screenshotsDtos
            };

            return Result.Success(addMessageRespons);
        }
    }
}


public class AddMessageEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("chats/messages", async (AddMessageRequest request ,ISender sender) =>
        {
            var command = request.Adapt<AddMessage.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        })
        .RequireAuthorization()
        .WithTags("Messages");

    }
}
