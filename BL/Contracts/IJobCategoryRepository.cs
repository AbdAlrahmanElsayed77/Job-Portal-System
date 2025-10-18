using BL.Dtos;
using Domains;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IJobCategoryRepository:IBaseServices<JobCategory,JobCategoryDto>
    {

        Task<List<(JobCategoryDto Category, int JobCount)>> GetCategoriesWithJobCountAsync();


        Task<List<JobCategoryDto>> GetAllCategoriesAsync();
    }
}