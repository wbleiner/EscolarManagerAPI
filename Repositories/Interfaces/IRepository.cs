using EsolarManagerAPI.Models.Interfaces;
using System.Linq.Expressions;

namespace EsolarManagerAPI.Repositories.Interfaces
{
    public interface IRepository< T> where T : IEntity
    {
        Task Add(T entity );
        Task Delete( T entity );
        Task Update( T entity );
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> Get(Expression<Func<T,bool>> predicate);
    }
}
