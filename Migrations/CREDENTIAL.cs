using FluentMigrator;

namespace mtdg_tpp.Migrations
{
    [Migration(0003)]
    public class CREDENTIAL : Migration
    {
        public override void Up()
        {
            Create.Table("CREDENTIAL")
                .WithColumn("CREDENTIAL_ID").AsInt64().PrimaryKey().Identity()
                .WithColumn("CREDENTIAL_PROVIDER_ID").AsInt64().Nullable()
                .WithColumn("CREDENTIAL_PROVIDER_TYPE_ID").AsInt64().Nullable()
                .WithColumn("CREDENTIAL_PROGRAM_ID").AsInt64().Nullable()
                .WithColumn("CREDENTIAL_REQUIREMENT").AsCustom("TEXT").Nullable()
                .WithColumn("CREDENTIAL_STATUS").AsString(2).WithDefaultValue("A")
                .WithColumn("CREDENTIAL_CRT_DT").AsDateTime().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("CREDENTIAL_UPDT_DT").AsDateTime().Nullable().WithDefault(SystemMethods.CurrentDateTime);

            Insert.IntoTable("CREDENTIAL")
            .Row(new
            {
                CREDENTIAL_ID = 9,
                CREDENTIAL_PROVIDER_ID = 9,
                CREDENTIAL_PROVIDER_TYPE_ID = -1,
                CREDENTIAL_PROGRAM_ID = 8,
                CREDENTIAL_REQUIREMENT = "[{\"Endpoint\": \"http://localhost:5002/api/Todo/UserTodo/\", \"Description\": \"Get User Todo by User ID\", \"Method\": \"GET\", \"Action\": \"GetUserTodos\", \"Service\": \"TodoService\"}]",
                CREDENTIAL_STATUS = "A",
                CREDENTIAL_CRT_DT = SystemMethods.CurrentDateTime,
                CREDENTIAL_UPDT_DT = SystemMethods.CurrentDateTime
            })
            .Row(new
            {
                CREDENTIAL_ID = 10,
                CREDENTIAL_PROVIDER_ID = 10,
                CREDENTIAL_PROVIDER_TYPE_ID = -1,
                CREDENTIAL_PROGRAM_ID = 8,
                CREDENTIAL_REQUIREMENT = "[{\"Endpoint\": \"http://localhost:5002/api/Todo/NewTodo\", \"Description\": \"Create User Todo\", \"Method\": \"POST\", \"Action\": \"NewTodo\", \"Service\": \"TodoService\"}]",
                CREDENTIAL_STATUS = "A",
                CREDENTIAL_CRT_DT = SystemMethods.CurrentDateTime,
                CREDENTIAL_UPDT_DT = SystemMethods.CurrentDateTime
            })
            .Row(new
            {
                CREDENTIAL_ID = 11,
                CREDENTIAL_PROVIDER_ID = 11,
                CREDENTIAL_PROVIDER_TYPE_ID = -1,
                CREDENTIAL_PROGRAM_ID = 8,
                CREDENTIAL_REQUIREMENT = "[{\"Endpoint\": \"http://localhost:5002/api/Todo/Update\", \"Description\": \"Update User Todo\", \"Method\": \"PUT\", \"Action\": \"UpdateTodo\", \"Service\": \"TodoService\"}]",
                CREDENTIAL_STATUS = "A",
                CREDENTIAL_CRT_DT = SystemMethods.CurrentDateTime,
                CREDENTIAL_UPDT_DT = SystemMethods.CurrentDateTime
            })
            .Row(new
            {
                CREDENTIAL_ID = 12,
                CREDENTIAL_PROVIDER_ID = 12,
                CREDENTIAL_PROVIDER_TYPE_ID = -1,
                CREDENTIAL_PROGRAM_ID = 8,
                CREDENTIAL_REQUIREMENT = "[{\"Endpoint\": \"http://localhost:5002/api/Todo/TodoID/\", \"Description\": \"Get Todo By Todo Id\", \"Method\": \"GET\", \"Action\": \"GetTodoById\", \"Service\": \"TodoService\"}]",
                CREDENTIAL_STATUS = "A",
                CREDENTIAL_CRT_DT = SystemMethods.CurrentDateTime,
                CREDENTIAL_UPDT_DT = SystemMethods.CurrentDateTime
            })
            .Row(new
            {
                CREDENTIAL_ID = 13,
                CREDENTIAL_PROVIDER_ID = 13,
                CREDENTIAL_PROVIDER_TYPE_ID = -1,
                CREDENTIAL_PROGRAM_ID = 8,
                CREDENTIAL_REQUIREMENT = "[{\"Endpoint\": \"http://localhost:5002/api/Todo/Delete/\", \"Description\": \"Delete User Todo by Todo ID\", \"Method\": \"DELETE\", \"Action\": \"DeleteTodo\", \"Service\": \"TodoService\"}]",
                CREDENTIAL_STATUS = "A",
                CREDENTIAL_CRT_DT = SystemMethods.CurrentDateTime,
                CREDENTIAL_UPDT_DT = SystemMethods.CurrentDateTime
            })
            .Row(new
            {
                CREDENTIAL_ID = 14,
                CREDENTIAL_PROVIDER_ID = 14,
                CREDENTIAL_PROVIDER_TYPE_ID = -1,
                CREDENTIAL_PROGRAM_ID = 8,
                CREDENTIAL_REQUIREMENT = "[{\"Endpoint\": \"http://localhost:5002/api/Todo/IsCompleted\", \"Description\": \"Mark Todo as Completed by Todo id with params\", \"Method\": \"PUT\", \"Action\": \"MarkAsCompleted\", \"Service\": \"TodoService\"}]",
                CREDENTIAL_STATUS = "A",
                CREDENTIAL_CRT_DT = SystemMethods.CurrentDateTime,
                CREDENTIAL_UPDT_DT = SystemMethods.CurrentDateTime
            });



        }
        public override void Down()
        {
            Delete.Table("CREDENTIAL");
        }
    }
}
