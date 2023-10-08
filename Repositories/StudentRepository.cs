using EsolarManagerAPI.Models;
using EsolarManagerAPI.Models.Enum;
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
                using (FbCommand command = new("INSERT INTO STUDENTS(name, gender, cpf, date_of_birth) VALUES (@NAME, @GENDER, @CPF, @DATEOFBIRTH)", connection))
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
        public async Task Update(Student entity)
        {
            using (var connection = GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync();
                }
                using var command = new FbCommand("UPDATE STUDENTS SET NAME = @NAME, GENDER = @GENDER, CPF = @CPF, DATE_OF_BIRTH = @DATE_OF_BIRTH  WHERE REGISTRATION = @REGISTRATION", connection);
                command.Parameters.AddWithValue("@NAME", entity.Name);
                command.Parameters.AddWithValue("@GENDER", entity.Gender);
                command.Parameters.AddWithValue("@CPF", entity.CPF);
                command.Parameters.AddWithValue("@DATE_OF_BIRTH", entity.DateOfBirth);

                var transaction = await connection.BeginTransactionAsync();
                try
                {
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected < 1)
                    {
                        throw new Exception();
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw new Exception();
                }
                finally
                {
                    transaction.Dispose();
                }
            }
        }
        public async Task Delete(Student entity)
        {
            using (var connection = GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync();
                }
                using var command = new FbCommand("DELETE FROM STUDENTS WHERE REGISTRATION = @REGISTRATION", connection);
                command.Parameters.AddWithValue("@REGISTRATION", entity.Registration);
                var rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected < 1)
                {
                    throw new Exception();
                }
            }
        }



        public async Task<IEnumerable<Student>> GetAll()
        {
            var students = new List<Student>();
            using (var connection = GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync();
                }
                using var command = new FbCommand("SELECT * FROM STUDENTS", connection);
                var dataReader = await command.ExecuteReaderAsync();
                while (await dataReader.ReadAsync())
                {
                    Student student = new(
                        registration: (int)dataReader.GetInt64("registration"),
                        name: dataReader.GetString("name"),
                        gender: (Gender)dataReader.GetInt64("gender"),
                        cpf: dataReader.GetString("cpf"),
                        dateOfBirth: dataReader.GetDateTime("date_of_birth")
                    );
                    students.Add(student);
                }
            }
            return students;
        }
        public async Task<IEnumerable<Student>> Get(Expression<Func<Student, bool>> predicate)
        {
            var allStudents = await GetAll();
            return allStudents.Where(predicate.Compile());
        }


        public async Task<IEnumerable<Student>> GetByContainingName(string partialName)
        {
            var allStudents = await GetAll();
            var studentsContainingName = allStudents.Where(student => student.Name.ToLower().Contains(partialName));

            return studentsContainingName;
        }

        public async Task<Student> GetByRegistration(int registrationId)
        {
            var allStudents = await GetAll();
            var studentFound = allStudents.FirstOrDefault(student => student.Registration.Equals(registrationId));
            return studentFound;
        }

        public async Task<IEnumerable<Student>> GetByPagination(int currentPage, int pageSize)
        {
            int firstRow = pageSize * (currentPage - 1);
            int lastRow = firstRow + pageSize; 
            var students = new List<Student>();
            using (var connection = GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync();
                }
                using var command = new FbCommand("SELECT FROM STUDENTS ROWS @FIRSTROW TO @LASTROW", connection);
                command.Parameters.AddWithValue("@FIRSTROW", firstRow);
                command.Parameters.AddWithValue("@LASTROW", lastRow);

                var dataReader = await command.ExecuteReaderAsync();
                while (await dataReader.ReadAsync())
                {
                    Student student = new(
                        registration: (int)dataReader.GetInt64("registration"),
                        name: dataReader.GetString("name"),
                        gender: (Gender)dataReader.GetInt64("gender"),
                        cpf: dataReader.GetString("cpf"),
                        dateOfBirth: dataReader.GetDateTime("date_of_birth")
                    );
                    students.Add(student);
                }

            }
            return students;
        }
    }
}
