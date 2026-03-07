namespace PatientStudy.Services
{
    public interface IAuthService
    {
        Task<bool> ValidateCredentialsAsync(string username, string password);
        string? CurrentUser { get; }
    }
}
