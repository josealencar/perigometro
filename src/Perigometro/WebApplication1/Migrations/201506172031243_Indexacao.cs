namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Indexacao : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Acidentes", "Ano", unique: false, name: "idx_AnoAcidente", clustered: false, anonymousArguments: null);
            CreateIndex("dbo.Acidentes", "Fatais", unique: false, name: "idx_FataisAcidente", clustered: false, anonymousArguments: null);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Acidentes", "idx_AnoAcidente", anonymousArguments: null);
            DropIndex("dbo.Acidentes", "idx_FataisAcidente", anonymousArguments: null);
        }
    }
}
