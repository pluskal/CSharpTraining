namespace LDAPClient
{
    public class UserModel
    {
        public UserModel(string dn, string cn, string description, string userPassword)
        {
            DN = dn;
            CN = cn;
            Description = description;
            UserPassword = userPassword;
        }

        public string DN { get; }
        public string CN { get; }
        public string Description { get; }
        public string UserPassword { get; }
    }
}