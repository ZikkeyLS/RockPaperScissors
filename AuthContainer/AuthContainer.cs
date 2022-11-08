namespace RockPaperScissors
{
    public static class AuthContainer
    {
        public static readonly Dictionary<int, string> AuthUsers = new Dictionary<int, string>();

        public static void AddUser(int id, string value)
        {
            AuthUsers.Add(id, value);
        }

        public static void RemoveUser(int id)
        {
            AuthUsers.Remove(id);
        }

        public static string LogStatus(int id, string value)
        {
            AuthUsers.TryGetValue(id, out string final_value);

            if(value != null)
            {
                if (value == final_value)
                    return "correct";
                else
                    return "alreadyLoggedIn";
            }
            else
            {
                return "loggingIn";
            }
        }
    }
}
