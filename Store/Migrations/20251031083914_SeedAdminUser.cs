using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO Users (Name, Email, Phone, PhoneNumber, PasswordHash, Role, FirstName, LastName, IsActive, CreatedAt, UpdatedAt, ImageUrl, Password)
                VALUES (
                    N'admin app',
                    N'admin@gmail.com',
                    '0610000000',
                    '0610000000',
                    N'$2a$11$VREWqxqwPAO.gjVri2Y9UegeJIylP18GCKolHZYEjDsrhI/sDgR22',
                    N'Admin',
                    N'admin',
                    N'app',
                    1,
                    GETDATE(),
                    GETDATE(),
                    NULL,
                    NULL
                )
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM Users WHERE Email = N'admin@gmail.com'
            ");
        }
    }
}
