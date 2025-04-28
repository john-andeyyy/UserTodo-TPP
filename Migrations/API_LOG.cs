using FluentMigrator;

namespace mtdg_tpp.Migrations
{
    [Migration(0001)]
    public class API_LOG : Migration
    {
        public override void Up()
        {
            Create.Table("API_LOG")
                .WithColumn("API_ID").AsInt64().PrimaryKey().Identity()
                .WithColumn("API_METHOD_NAME").AsString(70).Nullable()
                .WithColumn("API_PARAMETERS").AsCustom("TEXT").Nullable()
                .WithColumn("API_RESPONSE").AsCustom("TEXT").Nullable()
                .WithColumn("API_IP_ADDRESS").AsString(30).Nullable()
                .WithColumn("CREATE_DATE").AsDateTime().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("API_GUID").AsString(80).Nullable();
        }

        public override void Down()
        {
            Delete.Table("API_LOG");
        }
    }


}
