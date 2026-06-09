using AutoMapper;
using HQVerse.Application.DTOs.Auth;
using HQVerse.Application.DTOs.Characters;
using HQVerse.Application.DTOs.ComicIssues;
using HQVerse.Application.DTOs.ComicSeries;
using HQVerse.Application.DTOs.Publishers;
using HQVerse.Application.DTOs.Reviews;
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

        // Review
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.UserAvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl))
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count));

        CreateMap<CreateReviewDto, Review>();
    }
}