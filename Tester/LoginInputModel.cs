namespace RSMS.Models.InputModels.Login
{
    public class LoginInputModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }


        public LoginInputModel(string username,
                               string password)
        {
            this.Username = username;
            this.Password = password;
        }


        public bool Validate()
        {
            if (String.IsNullOrEmpty(this.Username)) return false;
            if (String.IsNullOrEmpty(this.Password)) return false;

            return true;
        }
    }
}
