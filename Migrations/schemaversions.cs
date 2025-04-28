using FluentMigrator;

namespace mtdg_tpp.Migrations
{
    [Migration(0006)]
    public class SchemaVersions : Migration
    {
        public override void Up()
        {
            Create.Table("schemaversions")
                .WithColumn("schemaversionid").AsInt32().NotNullable()
                .WithColumn("scriptname").AsString(255).NotNullable()
                .WithColumn("applied").AsCustom("TIMESTAMP").NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Insert.IntoTable("schemaversions").Row(new
            {
                schemaversionid = 1,
                scriptname = "mtdg-tpp.Scripts.001DBInit.sql",
                applied = "2023-07-10 09:58:59"
            })
                .Row(new
                {
                    schemaversionid = 2,
                    scriptname = "mtdg-tpp.Scripts.002AppVersion.sql",
                    applied = "2023-07-10 09:58:59"
                })
                .Row(new
                {
                    schemaversionid = 3,
                    scriptname = "mtdg-tpp.Scripts.003InitialVersionService.sql",
                    applied = "2023-07-10 09:58:59"
                });
        }

        public override void Down()
        {
            Delete.Table("schemaversions");
        }
    }
}
