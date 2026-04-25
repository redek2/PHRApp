using PHRApp.Models.DTOs;

namespace PHRApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<int> CreateCategoryAsync(CreateCategoryDto dto);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
    }
}
