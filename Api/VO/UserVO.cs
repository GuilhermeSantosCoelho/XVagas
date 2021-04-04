using Base.ViewObject;
namespace XVagas.VO{
    public class UserVO : BaseVO{
        public string NomeCompleto { get; set; }   
        public string Email { get; set; }   
        public string Password { get; set; }   
    }
}