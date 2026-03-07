namespace PatientStudy.Services
{
    public class AuthService : IAuthService
    {
        public string? CurrentUser { get; private set; }

        public Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            // 示例：请替换为真实校验（API/DB/哈希密码等）
            var ok = !string.IsNullOrWhiteSpace(username) && username == "admin" && password == "admin";
            if (ok) CurrentUser = username;
            return Task.FromResult(ok);
        }
    }
}
