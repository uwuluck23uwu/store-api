using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;
using Store.Data;

namespace Store.Services;

public class CategoryService : Service<Category>, IServices.ICategoryService
{
    private readonly IServices.IImageService _imageService;

    public CategoryService(ApplicationDbContext db, AutoMapper.IMapper mapper, IServices.IImageService imageService)
        : base(db, mapper)
    {
        _imageService = imageService;
    }

    public async Task<ResponseData> GetAllAsync()
    {
        try
        {
            // Get all categories (both active and inactive) for admin management
            var categories = await _db.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            var categoryDtos = _mapper.Map<List<CategoryDTO>>(categories);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Categories retrieved successfully",
                categoryDtos
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseData> GetByIdAsync(int id)
    {
        try
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Category not found",
                    null
                );
            }

            var categoryDto = _mapper.Map<CategoryDTO>(category);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Category retrieved successfully",
                categoryDto
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseMessage> CreateAsync(CategoryCreateDTO dto)
    {
        try
        {
            // Check if category with same name already exists
            var existingCategory = await _db.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName);

            if (existingCategory != null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.BadRequest,
                    false,
                    "Category with this name already exists"
                );
            }

            var category = _mapper.Map<Category>(dto);

            // Handle image upload
            if (dto.Image != null)
            {
                var uploadResult = await _imageService.UploadImageAsync(dto.Image, "categories");
                if (uploadResult.TaskStatus && uploadResult.Data != null)
                {
                    var imageData = uploadResult.Data as dynamic;
                    category.ImageUrl = imageData?.ImageUrl;
                }
            }

            category.CreatedAt = DateTime.Now;
            category.UpdatedAt = DateTime.Now;

            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Category created successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    public async Task<ResponseMessage> UpdateAsync(int id, CategoryUpdateDTO dto)
    {
        try
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Category not found"
                );
            }

            // Check if another category with the same name exists
            if (dto.CategoryName != null)
            {
                var existingCategory = await _db.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName && c.CategoryId != id);

                if (existingCategory != null)
                {
                    return new ResponseMessage(
                        System.Net.HttpStatusCode.BadRequest,
                        false,
                        "Another category with this name already exists"
                    );
                }
            }

            // Handle image upload
            if (dto.Image != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(category.ImageUrl);
                }

                // Upload new image
                var uploadResult = await _imageService.UploadImageAsync(dto.Image, "categories");
                if (uploadResult.TaskStatus && uploadResult.Data != null)
                {
                    var imageData = uploadResult.Data as dynamic;
                    category.ImageUrl = imageData?.ImageUrl;
                }
            }

            // Update fields
            category.CategoryName = dto.CategoryName ?? category.CategoryName;
            category.Description = dto.Description ?? category.Description;
            category.IsActive = dto.IsActive;
            category.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Category updated successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    public async Task<ResponseMessage> DeleteAsync(int id)
    {
        try
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Category not found"
                );
            }

            // Check if category has products
            var hasProducts = await _db.Products.AnyAsync(p => p.CategoryId == id);

            if (hasProducts)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.BadRequest,
                    false,
                    "Cannot delete category with associated products"
                );
            }

            // Delete image if exists
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                await _imageService.DeleteImageAsync(category.ImageUrl);
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Category deleted successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    public async Task<ResponseData> GetActiveCategoriesAsync()
    {
        try
        {
            var categories = await _db.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            var categoryDtos = _mapper.Map<List<CategoryDTO>>(categories);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Active categories retrieved successfully",
                categoryDtos
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseMessage> ToggleActiveStatusAsync(int id)
    {
        try
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Category not found"
                );
            }

            // Check if category has active products
            var hasActiveProducts = await _db.Products
                .AnyAsync(p => p.CategoryId == id && p.IsActive);

            if (hasActiveProducts)
            {
                // Has active products - only toggle status (soft delete)
                category.IsActive = !category.IsActive;
                category.UpdatedAt = DateTime.Now;

                await _db.SaveChangesAsync();

                return new ResponseMessage(
                    System.Net.HttpStatusCode.OK,
                    true,
                    $"Category {(category.IsActive ? "enabled" : "disabled")} successfully"
                );
            }
            else
            {
                // No active products - hard delete
                // Delete image if exists
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(category.ImageUrl);
                }

                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();

                return new ResponseMessage(
                    System.Net.HttpStatusCode.OK,
                    true,
                    "Category deleted successfully"
                );
            }
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }
}
