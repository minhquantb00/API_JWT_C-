using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace API_JWT_C_.Handler.Email
{
    public class Validate
    {
        public static bool IsValidEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();
            return emailAttribute.IsValid(email);
        }
    }
}
