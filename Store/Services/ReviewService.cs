using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;
using Store.Data;

namespace Store.Services;

public class ReviewService : Service<Review>, IServices.IReviewService
{
    private readonly IServices.ISellerService _sellerService;
    private readonly IConfiguration _configuration;

    public ReviewService(ApplicationDbContext db, AutoMapper.IMapper mapper, IServices.ISellerService sellerService, IConfiguration configuration)
        : base(db, mapper)
    {
        _sellerService = sellerService;
        _configuration = configuration;
    }

    public async Task<ResponseData> GetByProductIdAsync(int productId)
    {
        try
        {
            var reviews = await _db.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var reviewDtos = _mapper.Map<List<ReviewDTO>>(reviews);

            // Note: Image URLs are kept as relative paths (e.g., "/uploads/users/image.jpg")
            // The client will convert them to absolute URLs using convertImageUrl() helper

            // Calculate average rating
            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Reviews retrieved successfully",
                new
                {
                    Reviews = reviewDtos,
                    AverageRating = averageRating,
                    TotalReviews = reviews.Count
                }
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

    public async Task<bool> CanReviewAsync(int userId, int productId, int orderId)
    {
        try
        {
            // Check if order exists and belongs to user
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
                return false;

            // Check if order is delivered
            if (order.Status != "Delivered")
                return false;

            // Check if product is in the order
            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
            if (orderItem == null)
                return false;

            // Check if user already reviewed this product for this order
            var existingReview = await _db.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId && r.OrderId == orderId);

            return existingReview == null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<ResponseMessage> CreateAsync(int userId, ReviewCreateDTO dto)
    {
        try
        {
            // Validate that user can review only if orderId is provided
            if (dto.OrderId.HasValue)
            {
                var canReview = await CanReviewAsync(userId, dto.ProductId, dto.OrderId.Value);
                if (!canReview)
                {
                    return new ResponseMessage(
                    System.Net.HttpStatusCode.BadRequest,
                    false,
                    "Cannot review: Order must be delivered, product must be purchased, or already reviewed"
                );
                }
            }
            // Allow multiple reviews per user per product when no OrderId is provided

            // Create review
            var review = _mapper.Map<Review>(dto);
            review.UserId = userId;
            review.CreatedAt = DateTime.Now;
            review.UpdatedAt = DateTime.Now;

            await _db.Reviews.AddAsync(review);
            await _db.SaveChangesAsync();

            // Update product rating
            await UpdateProductRatingAsync(dto.ProductId);

            // Update seller rating
            var product = await _db.Products.FindAsync(dto.ProductId);
            if (product != null)
            {
                await _sellerService.UpdateRatingAsync(product.SellerId);
            }

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Review created successfully"
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

    public async Task<ResponseMessage> UpdateAsync(int id, int userId, ReviewUpdateDTO dto)
    {
        try
        {
            var review = await _db.Reviews.FindAsync(id);

            if (review == null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Review not found"
            );
            }

            // Check ownership
            if (review.UserId != userId)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.Forbidden,
                false,
                "You don't have permission to update this review"
            );
            }

            // Update fields
            review.Rating = dto.Rating;
            review.Comment = dto.Comment ?? review.Comment;
            review.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            // Update product rating
            await UpdateProductRatingAsync(review.ProductId);

            // Update seller rating
            var product = await _db.Products.FindAsync(review.ProductId);
            if (product != null)
            {
                await _sellerService.UpdateRatingAsync(product.SellerId);
            }

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Review updated successfully"
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

    public async Task<ResponseMessage> DeleteAsync(int id, int userId)
    {
        try
        {
            var review = await _db.Reviews.FindAsync(id);

            if (review == null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Review not found"
            );
            }

            // Check ownership or admin
            if (review.UserId != userId)
            {
                var user = await _db.Users.FindAsync(userId);
                if (user?.Role != "Admin")
                {
                    return new ResponseMessage(
                System.Net.HttpStatusCode.Forbidden,
                false,
                "You don't have permission to delete this review"
            );
                }
            }

            var productId = review.ProductId;
            var product = await _db.Products.FindAsync(productId);

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();

            // Update product rating
            await UpdateProductRatingAsync(productId);

            // Update seller rating
            if (product != null)
            {
                await _sellerService.UpdateRatingAsync(product.SellerId);
            }

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Review deleted successfully"
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

    private async Task UpdateProductRatingAsync(int productId)
    {
        try
        {
            var reviews = await _db.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            var product = await _db.Products.FindAsync(productId);
            if (product != null)
            {
                if (reviews.Any())
                {
                    product.Rating = (decimal)reviews.Average(r => r.Rating);
                }
                else
                {
                    product.Rating = 0;
                }

                product.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }
        catch (Exception)
        {
            // Log error but don't throw
        }
    }
}
