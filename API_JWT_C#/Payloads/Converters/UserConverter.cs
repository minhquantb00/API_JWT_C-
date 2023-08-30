using API_JWT_C_.Entities;
using API_JWT_C_.Payloads.DTOs;

namespace API_JWT_C_.Payloads.Converters
{
    public class UserConverter
    {
        public UserDTO EntityToDTO(User user)
        {
            return new UserDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                DateOfBirth = user.DateOfBirth,
                FirstName = user.FirstName,
                Gender = user.Gender,
                LastName = user.LastName
            };
        }
    }
}
