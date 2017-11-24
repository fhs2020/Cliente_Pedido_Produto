using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductStore.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Total { get; set; }
        public string ProdutoNome { get; set; }
        public decimal? ValorTotal { get; set; }
        public decimal? ValorProdudo { get; set; }


        public decimal? BasicoPrimeiroAnoMensal { get; set; }
        public decimal? BasicoSegundoAnoMensal { get; set; }
        public decimal? BasicoTerceiroAnoMensal { get; set; }
        public decimal? BasicoQuartoAnoMensal { get; set; }

        public bool? SensorBasico { get; set; }
        public bool? SensorPremium { get; set; }

        public decimal? PremiumPrimeiroAnoMensal { get; set; }
        public decimal? PremiumSegundoAnoMensal { get; set; }
        public decimal? PremiumTerceiroAnoMensal { get; set; }
        public decimal? PremiumQuartoAnoMensal { get; set; }
    }
}