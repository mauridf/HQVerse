using FluentValidation;
using HQVerse.Application.DTOs.Auth;
using HQVerse.Application.DTOs.Characters;
using HQVerse.Application.DTOs.Collections;
using HQVerse.Application.DTOs.ComicIssues;
using HQVerse.Application.DTOs.ComicSeries;
using HQVerse.Application.DTOs.Creators;
using HQVerse.Application.DTOs.Favorites;
using HQVerse.Application.DTOs.Publishers;
using HQVerse.Application.DTOs.Reviews;
using HQVerse.Application.DTOs.Scans;
using HQVerse.Application.DTOs.StoryArcs;
using HQVerse.Application.DTOs.Teams;
using HQVerse.Application.DTOs.Universes;

namespace HQVerse.Application.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
    }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class CreatePublisherDtoValidator : AbstractValidator<CreatePublisherDto>
{
    public CreatePublisherDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
    }
}

public class CreateCharacterDtoValidator : AbstractValidator<CreateCharacterDto>
{
    public CreateCharacterDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
    }
}

public class CreateComicSeriesDtoValidator : AbstractValidator<CreateComicSeriesDto>
{
    public CreateComicSeriesDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
    }
}

public class CreateComicIssueDtoValidator : AbstractValidator<CreateComicIssueDto>
{
    public CreateComicIssueDtoValidator()
    {
        RuleFor(x => x.SeriesId).GreaterThan(0);
        RuleFor(x => x.IssueNumber).NotEmpty().MaximumLength(50);
    }
}

public class CreateTeamDtoValidator : AbstractValidator<CreateTeamDto>
{
    public CreateTeamDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
    }
}

public class CreateUniverseDtoValidator : AbstractValidator<CreateUniverseDto>
{
    public CreateUniverseDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
    }
}

public class CreateCreatorDtoValidator : AbstractValidator<CreateCreatorDto>
{
    public CreateCreatorDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
    }
}

public class CreateStoryArcDtoValidator : AbstractValidator<CreateStoryArcDto>
{
    public CreateStoryArcDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
    }
}

public class CreateScanGroupDtoValidator : AbstractValidator<CreateScanGroupDto>
{
    public CreateScanGroupDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
    }
}

public class CreateScanDtoValidator : AbstractValidator<CreateScanDto>
{
    public CreateScanDtoValidator()
    {
        RuleFor(x => x.IssueId).GreaterThan(0);
        RuleFor(x => x.Language).NotEmpty().MaximumLength(10);
    }
}

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(x => x.IssueId).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Rating).InclusiveBetween(1, 10);
    }
}

public class CreateCollectionDtoValidator : AbstractValidator<CreateCollectionDto>
{
    public CreateCollectionDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
    }
}

public class AddIssueToCollectionDtoValidator : AbstractValidator<AddIssueToCollectionDto>
{
    public AddIssueToCollectionDtoValidator()
    {
        RuleFor(x => x.IssueId).GreaterThan(0);
        RuleFor(x => x.Rating).InclusiveBetween(1, 10).When(x => x.Rating.HasValue);
    }
}

public class AddFavoriteDtoValidator : AbstractValidator<AddFavoriteDto>
{
    public AddFavoriteDtoValidator()
    {
        RuleFor(x => x.EntityType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EntityId).GreaterThan(0);
    }
}

public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}
