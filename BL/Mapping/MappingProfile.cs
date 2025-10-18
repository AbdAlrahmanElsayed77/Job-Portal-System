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
            CreateMap<JobPost, JobPostDto>().AfterMap((src, dst) => { dst.PublishedAt = src.CreatedDate; }).ReverseMap();
            CreateMap<JobType, JobTypeDto>().ReverseMap();
            CreateMap<CVFile, CVFileDto>().ReverseMap();
            CreateMap<JobSeekerProfile, JobSeekerProfileDto>().ReverseMap();
            CreateMap<SavedJob, SavedJobDto>().ReverseMap();
            CreateMap<EmployerProfile, EmployerProfileDto>().ReverseMap();



        }
    }
}
