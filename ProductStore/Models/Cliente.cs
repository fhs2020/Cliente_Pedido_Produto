using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductStore.Models
{
    public class Cliente
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Nome é obrigatorio")]
        [StringLength(100)]
        public String Nome { get; set; }
        [Required(ErrorMessage = "Fazenda é obrigatorio")]
        [StringLength(100)]
        public String Fazenda { get; set; }
        [Required(ErrorMessage = "CPF é obrigatorio")]
        [StringLength(18)]
        public String CPF { get; set; }
        public int RG { get; set; }
        [Required(ErrorMessage = "Endereço é obrigatorio")]
        [StringLength(250)]
        public String EnderecoResidencial { get; set; }
        public String EnderecoRural { get; set; }
        public String Documento { get; set; }
        [Required(ErrorMessage = "Telefone é obrigatorio")]
        [StringLength(20)]
        public String Telefone { get; set; }
        [Required(ErrorMessage = "Celular é obrigatorio")]
        [StringLength(20)]
        public String Celular { get; set; }
        [Required(ErrorMessage = "Email é obrigatorio")]
        [StringLength(35)]
        public String Email { get; set; }
        public String UserId { get; set; }
        public String VendedorNome { get; set; }
    }
}