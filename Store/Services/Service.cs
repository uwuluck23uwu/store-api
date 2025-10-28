using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Store.Services
{
    public class Service<T> : IServices.IService<T> where T : class
    {
        protected readonly ApplicationDbContext _db;
        protected readonly IMapper _mapper;
        protected readonly DbSet<T> _dbSet;

        public Service(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _dbSet = _db.Set<T>();
        }

        public virtual async Task<ResponseData> GetAllAsync(Expression<Func<T, object>>[]? includes = null)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (includes != null)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                var items = await query.ToListAsync();

                return new ResponseData(
                    statusCode: HttpStatusCode.OK,
                    taskStatus: true,
                    message: "Retrieved successfully",
                    data: items
                );
            }
            catch (Exception ex)
            {
                return new ResponseData(
                    statusCode: HttpStatusCode.InternalServerError,
                    taskStatus: false,
                    message: $"Error: {ex.Message}",
                    data: null
                );
            }
        }

        public virtual async Task<ResponseData> GetAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? includes = null)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (includes != null)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }

                var item = await query.FirstOrDefaultAsync(predicate);

                if (item == null)
                {
                    return new ResponseData(
                        statusCode: HttpStatusCode.NotFound,
                        taskStatus: false,
                        message: "Not found",
                        data: null
                    );
                }

                return new ResponseData(
                    statusCode: HttpStatusCode.OK,
                    taskStatus: true,
                    message: "Retrieved successfully",
                    data: item
                );
            }
            catch (Exception ex)
            {
                return new ResponseData(
                    statusCode: HttpStatusCode.InternalServerError,
                    taskStatus: false,
                    message: $"Error: {ex.Message}",
                    data: null
                );
            }
        }

        public virtual async Task<ResponseMessage> CreateAsync(T model)
        {
            try
            {
                await _dbSet.AddAsync(model);
                await SaveAsync();

                return new ResponseMessage(
                    statusCode: HttpStatusCode.Created,
                    taskStatus: true,
                    message: "Created successfully"
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.InternalServerError,
                    taskStatus: false,
                    message: $"Error: {ex.Message}"
                );
            }
        }

        public virtual async Task<ResponseMessage> UpdateAsync(T model)
        {
            try
            {
                _dbSet.Attach(model);
                _db.Entry(model).State = EntityState.Modified;
                await SaveAsync();

                return new ResponseMessage(
                    statusCode: HttpStatusCode.OK,
                    taskStatus: true,
                    message: "Updated successfully"
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.InternalServerError,
                    taskStatus: false,
                    message: $"Error: {ex.Message}"
                );
            }
        }

        public virtual async Task<ResponseMessage> DeleteAsync(int id)
        {
            try
            {
                var idProperty = typeof(T).GetProperties()
                    .FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));

                if (idProperty == null)
                {
                    return new ResponseMessage(
                        statusCode: HttpStatusCode.BadRequest,
                        taskStatus: false,
                        message: "Invalid entity: No Id property found"
                    );
                }

                var parameter = Expression.Parameter(typeof(T), "e");
                var property = Expression.Property(parameter, idProperty);
                var constant = Expression.Constant(id);
                var equality = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

                var entity = await _dbSet.FirstOrDefaultAsync(lambda);

                if (entity == null)
                {
                    return new ResponseMessage(
                        statusCode: HttpStatusCode.NotFound,
                        taskStatus: false,
                        message: "Not found"
                    );
                }

                _dbSet.Remove(entity);
                await SaveAsync();

                return new ResponseMessage(
                    statusCode: HttpStatusCode.OK,
                    taskStatus: true,
                    message: "Deleted successfully"
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.InternalServerError,
                    taskStatus: false,
                    message: $"Error: {ex.Message}"
                );
            }
        }

        protected async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        protected async Task<string> GenerateOrderNumberAsync()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string prefix = $"ORD-{datePart}-";

            var lastOrder = await _db.Orders
                .Where(o => o.OrderNumber.StartsWith(prefix))
                .OrderByDescending(o => o.OrderNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastOrder != null && lastOrder.OrderNumber.Length > prefix.Length)
            {
                string numberPart = lastOrder.OrderNumber.Substring(prefix.Length);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:D4}";
        }
    }
}
