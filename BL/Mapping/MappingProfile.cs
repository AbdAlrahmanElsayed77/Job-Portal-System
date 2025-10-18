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
            CreateMap<Application, ApplicationDto>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<JobCategory, JobCategoryDto>().ReverseMap();
            CreateMap<JobPost, JobPostDto>().ReverseMap();
            CreateMap<JobType, JobTypeDto>().ReverseMap();
            CreateMap<CVFile, CVFileDto>().ReverseMap()
                .ForMember(dest => dest.JobSeeker, opt => opt.Ignore());
            CreateMap<JobSeekerProfile, JobSeekerProfileDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore());
            CreateMap<SavedJob, SavedJobDto>().ReverseMap();
            CreateMap<EmployerProfile, EmployerProfileDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ReverseMap();

        }
    }
}



//using AutoMapper;
//using BL.Dtos;
//using Domains;
//using Domains.UserModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BL.Mapping
//{
//    public class MappingProfile : Profile
//    {
//        public MappingProfile()
//        {
//            //CreateMap<TestClass, TestDto>().ReverseMap();
//            // CreateMap<TestClass, TestDto>()
//            //.ForMember(dest => dest.FullName,
//            //    opt => opt.MapFrom(src => $"{src.FName} {src.LName}"))
//            //.ReverseMap();

//            // ✅ Application Mappings
//            CreateMap<Application, ApplicationDto>()
//                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.CurrentState))
//                .ForMember(dest => dest.AppliedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
//                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedDate))
//                .ReverseMap()
//                .ForMember(dest => dest.CurrentState, opt => opt.MapFrom(src => src.Status));

//            CreateMap<ApplicationUser, ApplicationUserDto>()
//                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.UserName ?? ""))
//                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
//                .ReverseMap();

//            // ✅ Company Mappings
//            CreateMap<Company, CompanyDto>()
//                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
//                .ReverseMap();

//            CreateMap<JobCategory, JobCategoryDto>().ReverseMap();

//            // ✅ JobPost Mappings
//            CreateMap<JobPost, JobPostDto>()
//                .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
//                .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore())
//                .ReverseMap();

//            CreateMap<JobType, JobTypeDto>().ReverseMap();

//            // ✅ CVFile Mappings
//            CreateMap<CVFile, CVFileDto>()
//                .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
//                .ReverseMap();

//            // ✅ JobSeekerProfile Mappings
//            CreateMap<JobSeekerProfile, JobSeekerProfileDto>()
//                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
//                .ReverseMap();

//            // ✅ SavedJob Mappings
//            CreateMap<SavedJob, SavedJobDto>()
//                .ForMember(dest => dest.SavedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
//                .ReverseMap();

//            // ✅ EmployerProfile Mappings
//            CreateMap<EmployerProfile, EmployerProfileDto>()
//                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate ?? DateTime.Now))
//                .ForMember(dest => dest.Phone, opt => opt.Ignore())
//                .ReverseMap();
//        }
//    }
//}