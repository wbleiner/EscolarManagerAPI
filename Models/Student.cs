using EsolarManagerAPI.Models.Enum;
using EsolarManagerAPI.Models.Interfaces;

namespace EsolarManagerAPI.Models
{
    public class Student: IEntity
    {
        public Student(int registration, string name, string? cpf, DateTime dateOfBirth, Gender gender)
        {
            Registration = registration;
            Name = name;
            CPF = cpf;
            DateOfBirth = dateOfBirth;
            Gender = gender;
        }

        public int Registration { get; set; }
        public string Name { get; set; }
        public string? CPF { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
    }
}
