using System.Linq.Expressions;

namespace Store.Services.IServices
{
    public interface IService<T> where T : class
    {
        Task<ResponseData> GetAllAsync(Expression<Func<T, object>>[]? includes = null);
        Task<ResponseData> GetAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? includes = null);
        Task<ResponseMessage> CreateAsync(T model);
        Task<ResponseMessage> UpdateAsync(T model);
        Task<ResponseMessage> DeleteAsync(int id);
    }
}
