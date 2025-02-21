using FOMSDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOMSService
{
    public interface IUserservice
    {
        Task<IEnumerable<UserDTO>> GetUserAll();
        Task<UserDTO> GetUserById(int id);
        Task Create(UserDTO userDTO);
        Task Update(UserDTO userDTO);
        Task Delete(int id);
        Task<UserDTO?> Login(string email, string password);
        Task<UserDTO> Register(RegisterDTO registerDTO);  
    }
}
