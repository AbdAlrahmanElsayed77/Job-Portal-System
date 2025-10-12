using AutoMapper;
using BL.Dtos;
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

            CreateMap<Application, ApplicationDto>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<JobCategory, JobCategoryDto>().ReverseMap();
            CreateMap<JobPost, JobPostDto>().ReverseMap();
            CreateMap<JobType, JobTypeDto>().ReverseMap();
            CreateMap<CVFile, CVFileDto>().ReverseMap();
            CreateMap<JobSeekerProfile, JobSeekerProfileDto>().ReverseMap();
            CreateMap<SavedJob, SavedJobDto>().ReverseMap();
            CreateMap<EmployerProfile, EmployerProfileDto>().ReverseMap();



        }
    }
}
