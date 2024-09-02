namespace dotNETLearning
{
    public class Person(string email, string password, Role role)
    {
        public string email { get; set; } = email;
        public string password { get; set; } = password;

        public Role role { get; set; } = role;
    }

    public class Role(string name)
    {
        public string Name { get; set; } = name;
    }
}


