using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherAlertsBot.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SubscriberCommands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CommandName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberCommands", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.ChatId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SubscriberSubscriberCommand",
                columns: table => new
                {
                    CommandsId = table.Column<int>(type: "int", nullable: false),
                    SubsrcibersChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberSubscriberCommand", x => new { x.CommandsId, x.SubsrcibersChatId });
                    table.ForeignKey(
                        name: "FK_SubscriberSubscriberCommand_SubscriberCommands_CommandsId",
                        column: x => x.CommandsId,
                        principalTable: "SubscriberCommands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriberSubscriberCommand_Subscribers_SubsrcibersChatId",
                        column: x => x.SubsrcibersChatId,
                        principalTable: "Subscribers",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberSubscriberCommand_SubsrcibersChatId",
                table: "SubscriberSubscriberCommand",
                column: "SubsrcibersChatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriberSubscriberCommand");

            migrationBuilder.DropTable(
                name: "SubscriberCommands");

            migrationBuilder.DropTable(
                name: "Subscribers");
        }
    }
}
