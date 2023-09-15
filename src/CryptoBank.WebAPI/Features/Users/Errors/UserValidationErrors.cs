namespace CryptoBank.WebAPI.Features.Users.Errors;

public static class UserValidationErrors
{
    private const string Prefix = "users_validation_";
    
    public const string PasswordRequired = Prefix + "password_required";
    public const string PasswordToShort = Prefix + "password_to_short";
    public const string BirthDateRequired = Prefix + "birth_date_required";
    public const string DateCannotBeInTheFutureOrToday = Prefix + "date_cannot_be_in_the_future_or_today";
    public const string EmailRequired = Prefix + "email_required";
    public const string EmailFormatIsWrong = Prefix + "email_format_is_wrong";
    public const string EmailAlreadyExists = Prefix + "email_already_exists";
    public const string UserNotFound = Prefix + "user_not_found";
}