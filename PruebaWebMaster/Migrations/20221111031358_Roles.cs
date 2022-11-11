using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaWebMaster.Migrations
{
    /// <inheritdoc />
    public partial class Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS(SELECT Id from AspNetRoles WHERE Id = 'c11fe42d-bf8e-4f48-bdc2-b3296b10e743')
                                 BEGIN
                                    INSERT INTO AspNetRoles(Id, [Name], [NormalizedName])
                                    VALUES('c11fe42d-bf8e-4f48-bdc2-b3296b10e743', 'admin', 'ADMIN')
                                 END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM AspNetRoles WHERE Id = 'c11fe42d-bf8e-4f48-bdc2-b3296b10e743'");
        }
    }
}
