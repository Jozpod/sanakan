namespace Shinden
{
    public class UserAuth
    {
        public readonly string Nickname;
        public readonly string Password;

        public UserAuth(string Nickname, string Password)
        {
            this.Nickname = Nickname;
            this.Password = Password;
        }
    }
}