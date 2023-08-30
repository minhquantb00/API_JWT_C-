namespace API_JWT_C_.DataContext
{
    public class SourseData
    {
        public static string MyConnect()
        {
            return "server=QUANMOVIT\\SQLEXPRESS;database=jwt_api;integrated security = sspi; encrypt = true; trustservercertificate=true;";
        }
    }
}
