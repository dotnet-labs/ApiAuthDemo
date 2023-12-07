namespace ApiAuthDemo.Services;

public interface IUserService
{
    bool IsValidUser(string userName, string password);
}

public class UserService(ILogger<UserService> logger) : IUserService
{
    // you can inject database for user validation
    public bool IsValidUser(string userName, string password)
    {
        logger.LogInformation("Validating user [{userName}]", userName);
        if (string.IsNullOrWhiteSpace(userName))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }
        return true;
    }
}