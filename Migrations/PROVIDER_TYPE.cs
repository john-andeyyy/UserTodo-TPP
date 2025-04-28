using FluentMigrator;
using System;

namespace mtdg_tpp.Migrations
{
    [Migration(0005)]
    public class AddProviderTypeTable : Migration
    {
        public override void Up()
        {
            Create.Table("PROVIDER_TYPE")
                 .WithColumn("PROVIDER_TYPE_ID").AsInt64().NotNullable()
                 .WithColumn("PROVIDER_TYPE_CODE").AsString(20).Nullable()
                 .WithColumn("PROVIDER_TYPE_NAME").AsString(100).Nullable()
                 .WithColumn("PROVIDER_TYPE_DESCRIPTION").AsString(200).Nullable()
                 .WithColumn("PROVIDER_TYPE_CREATE_DT").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
                 .WithColumn("PROVIDER_TYPE_UPDATE_DT").AsDateTime().Nullable().WithDefault(SystemMethods.CurrentDateTime)
                 .WithColumn("PROVIDER_TYPE_STATUS").AsString(1).NotNullable().WithDefaultValue("A");
            

            Insert.IntoTable("PROVIDER_TYPE").Row(new
            {
                PROVIDER_TYPE_ID = 1,
                PROVIDER_TYPE_CODE = "MONEYTRANS",
                PROVIDER_TYPE_NAME = "Money Transfer",
                PROVIDER_TYPE_DESCRIPTION = "For Money Transfer API",
                PROVIDER_TYPE_CREATE_DT = new DateTime(2021, 4, 7, 17, 53, 27),
                PROVIDER_TYPE_UPDATE_DT = (DateTime?)null,
                PROVIDER_TYPE_STATUS = "A"
            })
                .Row(new
                {
                    PROVIDER_TYPE_ID = 2,
                    PROVIDER_TYPE_CODE = "BILLPAY",
                    PROVIDER_TYPE_NAME = "Billpay Transaction",
                    PROVIDER_TYPE_DESCRIPTION = "For Billpay Transfer API",
                    PROVIDER_TYPE_CREATE_DT = new DateTime(2021, 4, 7, 17, 54, 08),
                    PROVIDER_TYPE_UPDATE_DT = (DateTime?)null,
                    PROVIDER_TYPE_STATUS = "A"
                })
                .Row(new
                {
                    PROVIDER_TYPE_ID = 3,
                    PROVIDER_TYPE_CODE = "TOPUP",
                    PROVIDER_TYPE_NAME = "Mobile Topup",
                    PROVIDER_TYPE_DESCRIPTION = "For Mobile Topup",
                    PROVIDER_TYPE_CREATE_DT = new DateTime(2021, 4, 7, 20, 40, 46),
                    PROVIDER_TYPE_UPDATE_DT = (DateTime?)null,
                    PROVIDER_TYPE_STATUS = "A"
                })
                .Row(new
                {
                    PROVIDER_TYPE_ID = 4,
                    PROVIDER_TYPE_CODE = "CARD",
                    PROVIDER_TYPE_NAME = "Card Management Module",
                    PROVIDER_TYPE_DESCRIPTION = "For Card Management Module with Debit, Credit, Registration Functions",
                    PROVIDER_TYPE_CREATE_DT = new DateTime(2021, 4, 7, 20, 41, 42),
                    PROVIDER_TYPE_UPDATE_DT = (DateTime?)null,
                    PROVIDER_TYPE_STATUS = "A"
                })
                .Row(new
                {
                    PROVIDER_TYPE_ID = 6,
                    PROVIDER_TYPE_CODE = "PAYMENT",
                    PROVIDER_TYPE_NAME = "Payment Processor",
                    PROVIDER_TYPE_DESCRIPTION = "Payment Processor",
                    PROVIDER_TYPE_CREATE_DT = new DateTime(2021, 4, 16, 17, 10, 18),
                    PROVIDER_TYPE_UPDATE_DT = (DateTime?)null,
                    PROVIDER_TYPE_STATUS = "A"
                });

        }

        public override void Down()
        {
            Delete.Table("PROVIDER_TYPE");
        }
    }
}
