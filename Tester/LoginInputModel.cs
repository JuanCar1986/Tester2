namespace Tester
{
    public class LoginInputModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }


        public LoginInputModel(string username,
                               string password)
        {
            Username = username;
            Password = password;
        }


        public bool Validate()
        {
            if (string.IsNullOrEmpty(Username)) return false;
            if (string.IsNullOrEmpty(Password)) return false;

            return true;
        }
    }
}
