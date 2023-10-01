using EsolarManagerAPI.Models;
using EsolarManagerAPI.Repositories.Interfaces;
using FirebirdSql.Data.FirebirdClient;
using System.Data;
using System.Linq.Expressions;

namespace EsolarManagerAPI.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IConfiguration _configuration;

        public StudentRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private FbConnection GetConnection()
        {
            string connectionString = _configuration.GetConnectionString("Database");
            FbConnection connection = new(connectionString);
            return connection;
        }
        public async Task Add(Student entity)
        {
            using (var connection = GetConnection())
            {

                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync();
                }
                var transaction = await connection.BeginTransactionAsync();
                using (FbCommand command = new("INSERT INTO STUDENT (name, gender, cpf, date_of_birth) VALUES (@NAME, @GENDER, @CPF, @DATEOFBIRTH)", connection))
                {
                    command.Parameters.AddWithValue("@NAME", entity.Name);
                    command.Parameters.AddWithValue("@CPF", entity.CPF);
                    command.Parameters.AddWithValue("@GENDER", entity.Gender);
                    command.Parameters.AddWithValue("@DATEOFBIRTH", entity.DateOfBirth);
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        await transaction.CommitAsync();

                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                    finally
                    {
                        await transaction.DisposeAsync();
                        await connection.CloseAsync();
                    }
                }
            }

        }

        public async Task Delete(Student entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Student>> Get()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Student>> Get(Expression<Func<Student, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Student>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Student>> GetByCogetByContainingName(string partialName)
        {
            throw new NotImplementedException();
        }

        public Task<Student> GetByRegistration(int registrationId)
        {
            throw new NotImplementedException();
        }

        public Task Update(Student entity)
        {
            throw new NotImplementedException();
        }
    }
}
