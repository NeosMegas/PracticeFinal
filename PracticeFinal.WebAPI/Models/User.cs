namespace PracticeFinal.WebAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string[]? Roles { get; set; }

        public User() { }

        public User(int id, string name, string email, string password, string[] roles)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Roles = roles;
        }
    }
}
