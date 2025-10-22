using AutoMapper;
using BL.Dtos;
using BL.Dtos.AccountDtos;
using Domains;
using Domains.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Mapping
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            //CreateMap<TestClass, TestDto>().ReverseMap();


            // CreateMap<TestClass, TestDto>()
            //.ForMember(dest => dest.FullName,
            //    opt => opt.MapFrom(src => $"{src.FName} {src.LName}"))
            //.ReverseMap();
            //Account
            CreateMap<RegisterDto, ApplicationUser>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true));
            CreateMap<Application, ApplicationDto>().AfterMap((src, dst) => { dst.AppliedAt = src.CreatedDate ?? DateTime.MinValue; }).ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<JobCategory, JobCategoryDto>().ReverseMap();
            CreateMap<JobPost, JobPostDto>()
                .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
    //          .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt)) 
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
                .ForMember(dest => dest.JobCategory, opt => opt.MapFrom(src => src.JobCategory))
                .ForMember(dest => dest.JobType, opt => opt.MapFrom(src => src.JobType))
                .ReverseMap();
            CreateMap<JobType, JobTypeDto>().ReverseMap();
            CreateMap<CVFile, CVFileDto>()
                .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
                .ReverseMap();


            CreateMap<CVFile, ProfileCVFileDto>().ReverseMap()
                .ForMember(dest => dest.JobSeeker, opt => opt.Ignore());
            CreateMap<JobSeekerProfile, JobSeekerProfileDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore());
            CreateMap<SavedJob, SavedJobDto>()
                .ForMember(dest => dest.SavedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
                .ForMember(dest => dest.JobPost, opt => opt.MapFrom(src => src.JobPost))
                .ReverseMap();
            // SavedJob
            CreateMap<SavedJob, ProfileSavedJobDto>()
                .ForMember(dest => dest.SavedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobPost != null ? src.JobPost.Title : null))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.JobPost != null && src.JobPost.Company != null ? src.JobPost.Company.Name : null))
                .ReverseMap();
            CreateMap<EmployerProfile, EmployerProfileDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.CompanyLogoUrl, opt => opt.MapFrom(src => src.Company.LogoUrl))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
                .ReverseMap()
                .ForMember(dest => dest.Company, opt => opt.Ignore());
            // Application - Enhanced with display properties
            CreateMap<Application, ProfileApplicationDto>()
                .ForMember(dest => dest.AppliedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.MinValue))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.CurrentState))
                .ForMember(dest => dest.JobPostTitle, opt => opt.MapFrom(src => src.JobPost != null ? src.JobPost.Title : null))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.JobPost != null && src.JobPost.Company != null ? src.JobPost.Company.Name : null))
                .ForMember(dest => dest.CompanyLogoUrl, opt => opt.MapFrom(src => src.JobPost != null && src.JobPost.Company != null ? src.JobPost.Company.LogoUrl : null))
                .ReverseMap()
                .ForMember(dest => dest.CurrentState, opt => opt.MapFrom(src => src.Status));

        }
    }
}


