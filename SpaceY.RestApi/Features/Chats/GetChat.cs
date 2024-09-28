using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceY.RestApi.Contracts.Dtos;
using SpaceY.RestApi.Contracts.Responses;
using SpaceY.RestApi.Database;
using SpaceY.RestApi.Services;
using SpaceY.RestApi.Shared;

namespace SpaceY.RestApi.Features.Chats;

public static class GetChat
{
    public class Query : IRequest<Result<ChatDto>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<ChatDto>>
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserContext _userContext;

        public Handler(AppDbContext dbContext,
            IUserContext userContext)
        {
            _dbContext = dbContext;
            _userContext = userContext;
        }

        public async Task<Result<ChatDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var userid = _userContext.GetCurrentUserId();

            var chat = await _dbContext.Chats
                .Include(c => c.Screenshots)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == request.Id);

            if(chat!.UserId != userid)
            {
                return Result.Failure<ChatDto>(new Error(
                   "CreateChat.NotAllowed",
                   "Invalid email or password."));
            }

            var dto = chat.Adapt<ChatDto>();

            return Result.Success(dto);
        }
    }
}



public class GetChatByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("chats/{id:guid}", async ([FromRoute] Guid id,ISender sender) =>
        {
            var query = new GetChat.Query { Id = id };

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        })
        .RequireAuthorization()
        .WithTags("Chats");

    }
}
