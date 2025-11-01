using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUserAndSeller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert Admin User
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

            // Insert Seller for Admin User
            migrationBuilder.Sql(@"
                INSERT INTO Sellers (UserId, ShopName, ShopDescription, ShopImageUrl, LogoUrl, Description, PhoneNumber, Address, Rating, TotalSales, IsVerified, IsActive, CreatedAt, UpdatedAt, QrCodeUrl)
                SELECT
                    UserId,
                    N'ร้านของ admin',
                    N'ร้านค้าของผู้ดูแลระบบ',
                    NULL,
                    NULL,
                    N'ร้านค้าของผู้ดูแลระบบ',
                    '0610000000',
                    NULL,
                    0.00,
                    0,
                    1,
                    1,
                    GETDATE(),
                    GETDATE(),
                    NULL
                FROM Users
                WHERE Email = N'admin@gmail.com'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete Seller first (due to foreign key constraint)
            migrationBuilder.Sql(@"
                DELETE FROM Sellers
                WHERE UserId IN (SELECT UserId FROM Users WHERE Email = N'admin@gmail.com')
            ");

            // Delete Admin User
            migrationBuilder.Sql(@"
                DELETE FROM Users WHERE Email = N'admin@gmail.com'
            ");
        }
    }
}
