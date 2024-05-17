namespace AskJavra.Enums
{
    public enum UserType
    {
        User,
        Admin,
        Employee
    }

    public static class UserTypeExtensions
    {
        public static UserType ConverStringToUserType(string value)
        {
            switch (value.ToLower())
            {
                case "admin":
                    return UserType.Admin;
                case "employee":
                    return UserType.Employee;
                default:
                    return UserType.User;

            }
        }
    }
}
