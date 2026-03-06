public class User
{
    public string UserName { get; set;}
    public string Password { get; set;}
    public Age Age { get; set;}

    public User(string UserName, string Password, Age Age)
    {
        this.UserName = UserName;
        this.Password = Password;
        this.Age = Age;
    }
}