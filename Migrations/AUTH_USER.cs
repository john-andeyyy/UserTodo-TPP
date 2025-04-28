using FluentMigrator;
using System;

namespace mtdg_tpp.Migrations
{
    [Migration(0002)]
    public class AUTH_USER : Migration
    {
        public override void Up()
        {
            Create.Table("AUTH_USER")
                 .WithColumn("AUTH_USER_ID").AsInt64().PrimaryKey().Identity()
                 .WithColumn("AUTH_USER_NAME").AsString(50).Nullable()
                 .WithColumn("AUTH_USER_DESCRIPTION").AsString(250).Nullable()
                 .WithColumn("AUTH_USER_KEY").AsString(250).Nullable()
                 .WithColumn("AUTH_USER_LOGIN").AsString(80).Nullable()
                 .WithColumn("AUTH_USER_PASSWORD").AsString(100).Nullable()
                 .WithColumn("AUTH_USER_CREATE_DATE").AsDateTime().Nullable()
                 .WithColumn("AUTH_USER_UPDATE_DATE").AsDateTime().Nullable().WithDefault(SystemMethods.CurrentDateTime)
                 .WithColumn("AUTH_USER_CREATE_BY").AsString(80).Nullable()
                 .WithColumn("AUTH_USER_UPDATE_BY").AsString(80).Nullable()
                 .WithColumn("AUTH_USER_STATUS").AsString(2).NotNullable().WithDefaultValue("A");

            Insert.IntoTable("AUTH_USER").Row(new
            {
                AUTH_USER_ID = 1,
                AUTH_USER_NAME = "Auth User 1",
                AUTH_USER_DESCRIPTION = "Auth User 1",
                AUTH_USER_KEY = "V2D6MPPWD7Z5NPCT",
                AUTH_USER_LOGIN = "USER1",
                AUTH_USER_PASSWORD = "93246038d91f02b45aefd4b883edff31b67a00ce",
                AUTH_USER_CREATE_DATE = (DateTime?)null,
                AUTH_USER_UPDATE_DATE = new DateTime(2021, 4, 7, 15, 44, 57),
                AUTH_USER_CREATE_BY = "null",
                AUTH_USER_UPDATE_BY = "null",
                AUTH_USER_STATUS = "A"
            });


        }

        public override void Down()
        {
            Delete.Table("AUTH_USER");
        }
    }
}
