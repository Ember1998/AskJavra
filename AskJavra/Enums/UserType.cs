namespace AskJavra.Enums
{
    public enum UserType
    {
        User,
        Admin
    }

    public static class UserTypeExtensions
    {
        public static UserType ConverStringToUserType(string value)
        {
            switch (value.ToLower())
            {
                case "admin":
                    return UserType.Admin;
                default:
                    return UserType.User;

            }
        }
    }
}
