namespace Cross_platform_development_technologies.Contracts
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public string TokenType { get; set; } = "Bearer";
    }
}