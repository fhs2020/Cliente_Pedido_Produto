namespace ProductStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CampoEndereçoobrigatorio : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Clientes", "EnderecoResidencial", c => c.String(nullable: false, maxLength: 250, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Clientes", "EnderecoResidencial", c => c.String(unicode: false));
        }
    }
}
