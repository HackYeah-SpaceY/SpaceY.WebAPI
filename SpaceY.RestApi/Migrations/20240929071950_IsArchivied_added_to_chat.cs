using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceY.RestApi.Migrations
{
    /// <inheritdoc />
    public partial class IsArchivied_added_to_chat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Chats",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Chats");
        }
    }
}
