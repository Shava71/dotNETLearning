namespace dotNETLearning
{
    public class Person(string email, string password)
    {
        public string email { get; set; } = email;
        public string password { get; set; } = password;
    }
}
