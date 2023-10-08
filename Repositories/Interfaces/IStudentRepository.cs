using EsolarManagerAPI.Models;

namespace EsolarManagerAPI.Repositories.Interfaces
{
    public interface IStudentRepository:IRepository<Student>
    {
       Task<Student> GetByRegistration(int registrationId);
       Task<IEnumerable<Student>> GetByContainingName(string partialName);

       Task<IEnumerable<Student>> GetByPagination(int currentPage, int pageSize);
    }
}
