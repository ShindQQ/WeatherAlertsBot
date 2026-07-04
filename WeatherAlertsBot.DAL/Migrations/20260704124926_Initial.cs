using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WeatherAlertsBot.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriberCommands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommandName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberCommands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.ChatId);
                });

            migrationBuilder.CreateTable(
                name: "SubscriberSubscriberCommand",
                columns: table => new
                {
                    CommandsId = table.Column<int>(type: "integer", nullable: false),
                    SubscribersChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberSubscriberCommand", x => new { x.CommandsId, x.SubscribersChatId });
                    table.ForeignKey(
                        name: "FK_SubscriberSubscriberCommand_SubscriberCommands_CommandsId",
                        column: x => x.CommandsId,
                        principalTable: "SubscriberCommands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriberSubscriberCommand_Subscribers_SubscribersChatId",
                        column: x => x.SubscribersChatId,
                        principalTable: "Subscribers",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberSubscriberCommand_SubscribersChatId",
                table: "SubscriberSubscriberCommand",
                column: "SubscribersChatId");
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
