using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XVagas.Entity{
    public class User{

        [Key]
        [Required]
        [Column(name: "ID")]
        public long Id { get; set; }
        
        [StringLength(150)]
		[Column(name: "NomeCompleto")]
        public string NomeCompleto { get; set; }   

        [StringLength(100)]
		[Column(name: "Email")]
        public string Email { get; set; }   

        [StringLength(20)]
		[Column(name: "Password")]
        public string Password { get; set; }   
    }
}