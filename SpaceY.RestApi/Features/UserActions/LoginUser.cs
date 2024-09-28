using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SpaceY.RestApi.Contracts.Requests;
using SpaceY.RestApi.Contracts.Responses;
using SpaceY.RestApi.Entities;
using SpaceY.RestApi.Services;
using SpaceY.RestApi.Shared;

namespace SpaceY.RestApi.Features.UserActions;

public static class LoginUser
{
    internal static class ErrorCodes
    {
        public const string Validation = "UserLogin.Validation";
        public const string InvalidCredentials = "UserLogin.InvalidCredentials";
    }

    public class Command : IRequest<Result<LoginResponse>>
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }


    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Email)
                .EmailAddress()
                .NotEmpty();

            RuleFor(c => c.Password).NotEmpty();
        }
    }


    internal sealed class Handler : IRequestHandler<Command, Result<LoginResponse>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<Command> _validator;
        private readonly IAuthService _authService;

        public Handler(UserManager<User> userManager, IValidator<Command> validator, IAuthService authService)
        {
            _userManager = userManager;
            _validator = validator;
            _authService = authService;
        }

        public async Task<Result<LoginResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<LoginResponse>(new Error(
                   ErrorCodes.Validation,
                   validationResult.ToString()));
            }

            var user = await HandleLogin(_userManager, request.Email, request.Password);

            if (user is null)
            {
                return Result.Failure<LoginResponse>(new Error(
                    ErrorCodes.InvalidCredentials,
                    "Invalid email or password."));
            }

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _authService.GenerateTokenString(user, [.. roles]);


            var loginResponse = new LoginResponse
            {
                AccessToken = accessToken
            };

            return loginResponse;
        }

        private async Task<User?> HandleLogin(UserManager<User> userManager, string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return null;
            }

            var result = await userManager.CheckPasswordAsync(user, password);

            if (result is false)
            {
                return null;
            }

            return user;
        }
    }


}



public class LoginUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (LoginUserRequest request, ISender sender) =>
        {
            var command = request.Adapt<LoginUser.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        })
        .WithTags("User");

    }
}
