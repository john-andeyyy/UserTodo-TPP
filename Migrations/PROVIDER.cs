using FluentMigrator;
using System;

namespace mtdg_tpp.Migrations
{
    [Migration(0004)]
    public class AddProviderTable : Migration
    {
        public override void Up()
        {
            Create.Table("PROVIDER")
                 .WithColumn("PROVIDER_ID").AsInt64().NotNullable() 
                 .WithColumn("PROVIDER_CODE").AsString(20).Nullable()
                 .WithColumn("PROVIDER_NAME").AsString(100).Nullable()
                 .WithColumn("PROVIDER_DESCRIPTION").AsString(200).Nullable()
                 .WithColumn("PROVIDER_CREATE_DT").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                 .WithColumn("PROVIDER_UPDATE_DT").AsDateTime().Nullable().WithDefault(SystemMethods.CurrentDateTime)
                 .WithColumn("PROVIDER_STATUS").AsString(1).NotNullable().WithDefaultValue("A");




            Insert.IntoTable("PROVIDER").Row(new
            {
                PROVIDER_ID = 4,
                PROVIDER_CODE = "QOLO",
                PROVIDER_NAME = "QOLO",
                PROVIDER_DESCRIPTION = "For Qolo API Register, Debit and Credit Transactions",
                PROVIDER_CREATE_DT = new DateTime(2021, 4, 9, 22, 38, 5),
                PROVIDER_UPDATE_DT = new DateTime(2021, 4, 13, 14, 16, 41),
                PROVIDER_STATUS = "A"
            })
                .Row(new
                {
                    PROVIDER_ID = 5,
                    PROVIDER_CODE = "NMI",
                    PROVIDER_NAME = "NMI Payment Gateway",
                    PROVIDER_DESCRIPTION = "NMI Payment Gateway Processor",
                    PROVIDER_CREATE_DT = new DateTime(2021, 4, 16, 17, 14, 22),
                    PROVIDER_UPDATE_DT = (DateTime?)null, 
                    PROVIDER_STATUS = "A"
                })
                .Row(new
                {
                    PROVIDER_ID = 6,
                    PROVIDER_CODE = "equifax",
                    PROVIDER_NAME = "Equiax",
                    PROVIDER_DESCRIPTION = "Equifax",
                    PROVIDER_CREATE_DT = new DateTime(2021, 5, 14, 23, 1, 23),
                    PROVIDER_UPDATE_DT = new DateTime(2021, 5, 14, 23, 1, 49),
                    PROVIDER_STATUS = "A"
                })
                .Row(new
                {
                    PROVIDER_ID = 7,
                    PROVIDER_CODE = "mtx",
                    PROVIDER_NAME = "mtx",
                    PROVIDER_DESCRIPTION = "Marker Trax",
                    PROVIDER_CREATE_DT = new DateTime(2022, 11, 21, 19, 49, 27),
                    PROVIDER_UPDATE_DT = (DateTime?)null,
                    PROVIDER_STATUS = "A"
                })
                .Row(new
                {
                    PROVIDER_ID = 8,
                    PROVIDER_CODE = "koin",
                    PROVIDER_NAME = "koin",
                    PROVIDER_DESCRIPTION = "Koin Wallet",
                    PROVIDER_CREATE_DT = new DateTime(2022, 11, 21, 19, 49, 27),
                    PROVIDER_UPDATE_DT = (DateTime?)null,
                    PROVIDER_STATUS = "A"
                });

        }

        public override void Down()
        {
            Delete.Table("PROVIDER");
        }
    }
}
