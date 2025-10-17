using BL.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IJobCategoryRepository
    {

        Task<List<(JobCategoryDto Category, int JobCount)>> GetCategoriesWithJobCountAsync();


        Task<List<JobCategoryDto>> GetAllCategoriesAsync();
    }
}