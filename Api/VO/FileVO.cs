using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.ViewObject;

namespace XVagas.VO{
    public class FilePDFVO: BaseVO{
        public string NomeArquivo { get; set; }   
        public long UserId { get; set; }   
        public UserVO User { get; set; }   
    }
}