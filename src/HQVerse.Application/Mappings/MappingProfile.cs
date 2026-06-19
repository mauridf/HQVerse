using AutoMapper;
using HQVerse.Application.DTOs.Auth;
using HQVerse.Application.DTOs.Characters;
using HQVerse.Application.DTOs.ComicIssues;
using HQVerse.Application.DTOs.ComicSeries;
using HQVerse.Application.DTOs.Creators;
using HQVerse.Application.DTOs.Favorites;
using HQVerse.Application.DTOs.Publishers;
using HQVerse.Application.DTOs.Reviews;
using HQVerse.Application.DTOs.StoryArcs;
using HQVerse.Application.DTOs.Teams;
using HQVerse.Application.DTOs.Universes;
using HQVerse.Domain.Entities;

namespace HQVerse.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Publisher
        CreateMap<Publisher, PublisherDto>();
        CreateMap<CreatePublisherDto, Publisher>();
        CreateMap<UpdatePublisherDto, Publisher>();

        // Character
        CreateMap<Character, CharacterDto>();
        CreateMap<CreateCharacterDto, Character>();

        // ComicSeries
        CreateMap<ComicSeries, ComicSeriesDto>();
        CreateMap<CreateComicSeriesDto, ComicSeries>();

        // ComicIssue
        CreateMap<ComicIssue, ComicIssueDto>()
            .ForMember(dest => dest.SeriesName, opt => opt.MapFrom(src => src.Series.Name))
            .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewCount, opt => opt.Ignore());

        CreateMap<ComicIssue, ComicIssueDetailDto>()
            .ForMember(dest => dest.SeriesName, opt => opt.MapFrom(src => src.Series.Name))
            .ForMember(dest => dest.Characters, opt => opt.Ignore())
            .ForMember(dest => dest.Teams, opt => opt.Ignore())
            .ForMember(dest => dest.Creators, opt => opt.Ignore())
            .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewCount, opt => opt.Ignore());

        CreateMap<CreateComicIssueDto, ComicIssue>();

        // User
        CreateMap<User, UserDto>();
        CreateMap<UpdateUserDto, User>();

        // Review
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.UserAvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl))
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count));

        CreateMap<CreateReviewDto, Review>();

        // Team
        CreateMap<Team, TeamDto>();
        CreateMap<CreateTeamDto, Team>();
        CreateMap<UpdateTeamDto, Team>();

        // Creator
        CreateMap<Creator, CreatorDto>();
        CreateMap<CreateCreatorDto, Creator>();
        CreateMap<UpdateCreatorDto, Creator>();
        CreateMap<CreatorRole, CreatorRoleDto>();

        // StoryArc
        CreateMap<StoryArc, StoryArcDto>();
        CreateMap<StoryArc, StoryArcDetailDto>();
        CreateMap<CreateStoryArcDto, StoryArc>();
        CreateMap<UpdateStoryArcDto, StoryArc>();

        // Universe
        CreateMap<Universe, UniverseDto>();
        CreateMap<CreateUniverseDto, Universe>();
        CreateMap<UpdateUniverseDto, Universe>();
    }
}