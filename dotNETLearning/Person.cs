using Microsoft.AspNetCore.Authorization;

namespace dotNETLearning
{
    public class Person(string email, string password, int year, Role role, string City, string Company)
    {
        public string email { get; set; } = email;
        public string password { get; set; } = password;
        public int year { get; set; } = year;
        public Role role { get; set; } = role;
        
        public string City { get; set; } = City;
        public string Company { get; set; } = Company;
    }

    public class Role(string name)
    {
        public string Name { get; set; } = name;
    }

    public class AgeRequirement(int year) : IAuthorizationRequirement
    {
        public int age { get; set; } = year;
    }
}


