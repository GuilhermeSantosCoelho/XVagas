using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Entities;

namespace XVagas.Entity{
    public class FilePDF: BaseEntity{
        [StringLength(150)]
		[Column(name: "NomeArquivo")]
        public string NomeArquivo { get; set; }   

        [Column(name: "UserId")]
        public long UserId { get; set; }   

		[ForeignKey("UserId")]
        public User User { get; set; }   
    }
}