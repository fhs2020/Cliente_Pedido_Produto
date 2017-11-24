using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public string Customer { get; set; }
        public int ClienteID { get; set; }
        public decimal ValorTotal { get; set; }
        public string NomeConsultor { get; set; }

        public bool PagamentoBoleto  { get; set; }
        public bool PagamentoCartaoCredito { get; set; }
        public bool PagamentoCheque { get; set; }


        // Navigation property
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}