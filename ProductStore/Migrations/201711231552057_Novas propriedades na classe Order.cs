namespace ProductStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NovaspropriedadesnaclasseOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "PagamentoBoleto", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "PagamentoCartaoCredito", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "PagamentoCheque", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "PagamentoCheque");
            DropColumn("dbo.Orders", "PagamentoCartaoCredito");
            DropColumn("dbo.Orders", "PagamentoBoleto");
        }
    }
}
