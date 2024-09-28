using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceY.RestApi.Contracts.Dtos;
using SpaceY.RestApi.Database;
using SpaceY.RestApi.Services;
using SpaceY.RestApi.Shared;

namespace SpaceY.RestApi.Features.Chats;

public static class GetChats
{
    public class Query : IRequest<Result<List<ChatSummaryDto>>>  {  }

    internal sealed class Handler : IRequestHandler<Query, Result<List<ChatSummaryDto>>>
    {
        private readonly IUserContext _userContext;
        private readonly AppDbContext _dbContext;

        public Handler(IUserContext userContext,
            AppDbContext dbContext)
        {
            _userContext = userContext;
            _dbContext = dbContext;
        }

        public async Task<Result<List<ChatSummaryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();

            var dtos = await _dbContext.Chats
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.ModifiedAt)
                .Select(c => new ChatSummaryDto
                {
                    Id = c.Id,
                    Title = c.Title,
                }).ToListAsync();

            return Result.Success(dtos);
        }
    }
}


public class GetChatsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("chats", async (ISender sender) =>
        {
            var query = new GetChats.Query();

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
