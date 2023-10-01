using EsolarManagerAPI.Models;

namespace EsolarManagerAPI.Repositories.Interfaces
{
    public interface IStudentRepository:IRepository<Student>
    {
       Task<Student> GetByRegistration(int registrationId);
       Task<IEnumerable<Student>> GetByCogetByContainingName(string partialName);
        Task Add(Student entity);
    }
}
