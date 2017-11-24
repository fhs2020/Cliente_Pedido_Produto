using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProductStore.Models
{
    public class Product
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public bool DisponivelEmEstoque { get; set; }
        public int Quantidade { get; set; }
    }
}