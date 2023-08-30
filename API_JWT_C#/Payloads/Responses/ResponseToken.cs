namespace API_JWT_C_.Payloads.Responses
{
    public class ResponseToken
    {
        public string TokenType { get; set; } = "Bearer";
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
