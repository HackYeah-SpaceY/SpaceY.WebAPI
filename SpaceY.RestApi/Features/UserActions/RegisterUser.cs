using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SpaceY.RestApi.Database;
using SpaceY.RestApi.Entities;
using SpaceY.RestApi.Shared;

namespace SpaceY.RestApi.Features.UserActions;

public static class RegisterUser
{
    internal static class ErrorCodes
    {
        public const string Validation = "UserRegister.Validation";
        public const string EmailInUse = "UserRegister.EmailAlreadyInUser";
        public const string Failed = "UserRegister.RegisterFailed";
    }

    public class Command : IRequest<Result>
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
    }


    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(c => c.Password)
              .NotEmpty()
              .MinimumLength(8)
              .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
              .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
              .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
              .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.")
              .Matches(@"^[^\s]*$").WithMessage("Password must not contain whitespace characters.");


            RuleFor(c => c.ConfirmPassword)
                .Equal(c => c.Password).WithMessage("Passwords must match.")
                .NotEmpty();
        }
    }


    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<Command> _validator;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<Handler> _logger;

        public Handler(UserManager<User> userManager,
            IValidator<Command> validator,
            AppDbContext dbContext,
            ILogger<Handler> logger)
        {
            _userManager = userManager;
            _validator = validator;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing register request.");

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure(new Error(
                    ErrorCodes.Validation,
                    validationResult.ToString()));
            }

            var isUnique = await _userManager.FindByEmailAsync(request.Email);

            if (isUnique is not null)
            {
                _logger.LogInformation("Processing register request failed, account with email: {@email} already exist.", request.Email);

                return Result.Failure(new Error(
                  ErrorCodes.EmailInUse,
                  "This email is already in use."));

            }

            var user = new User { Email = request.Email, UserName = request.Email };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Errors.Any())
            {
                return Result.Failure(new Error(
                  ErrorCodes.Failed,
                  "There was an error while registering a user."));
            }

            _logger.LogInformation("New user has registered {@User}.", user);

            return Result.Success();
        }
    }

}


public class RegisterUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", async (RegisterUser.Command command, ISender sender) =>
        {
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok();
        })
        .WithTags("User");

    }
}
