namespace dotNETLearning
{
    public class Person(string email, string password, Role role, string City, string Company)
    {
        public string email { get; set; } = email;
        public string password { get; set; } = password;
        public Role role { get; set; } = role;
        
        public string City { get; set; } = City;
        public string Company { get; set; } = Company;
    }

    public class Role(string name)
    {
        public string Name { get; set; } = name;
    }
}


