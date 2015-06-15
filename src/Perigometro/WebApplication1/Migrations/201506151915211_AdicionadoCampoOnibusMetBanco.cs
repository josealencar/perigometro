namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdicionadoCampoOnibusMetBanco : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Acidentes", "Onibus_Met", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Acidentes", "Onibus_Met");
        }
    }
}
