using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Entities;

namespace XVagas.Entity{
    public class User: BaseEntity{

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