using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XVagas.Entity{
    public class File{
        [Key]
        [Required]
        [Column(name: "ID")]
        public long Id { get; set; }
        
        [StringLength(150)]
		[Column(name: "NomeArquivo")]
        public string NomeArquivo { get; set; }   

        [Column(name: "UserId")]
        public long UserId { get; set; }   

		[ForeignKey("UserId")]
        public User User { get; set; }   
    }
}