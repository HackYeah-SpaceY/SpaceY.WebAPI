using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceY.RestApi.Migrations
{
    /// <inheritdoc />
    public partial class CreatedAtchangedToModifiedAt_inChatEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Chats",
                newName: "ModifiedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Chats",
                newName: "CreatedAt");
        }
    }
}
