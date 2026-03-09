namespace TradingSystemsMonitoring.RestAPI.Abstractions.Identity
{
    public class TsmUserLoginData
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class TsmUserToken
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }

    public class TsmUserRegisterData
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class OperationResult
    {
        public bool IsSucceeded { get; }
        public string[] Errors { get; }

        public OperationResult(bool isSucceeded, string[] errors)
        {
            IsSucceeded = isSucceeded;
            Errors = errors;
        }
    }
}
