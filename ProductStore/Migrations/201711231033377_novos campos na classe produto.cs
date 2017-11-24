namespace ProductStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class novoscamposnaclasseproduto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "DisponivelEmEstoque", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "Quantidade", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Quantidade");
            DropColumn("dbo.Products", "DisponivelEmEstoque");
        }
    }
}
