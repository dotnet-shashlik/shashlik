using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Guc.NLogger.Mysql.Migrations
{
    public partial class SbtLog0001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User = table.Column<string>(maxLength: 255, nullable: true),
                    ClientId = table.Column<string>(maxLength: 255, nullable: true),
                    LogTime = table.Column<DateTime>(nullable: false),
                    Level = table.Column<string>(maxLength: 255, nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Logger = table.Column<string>(maxLength: 255, nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    ClientIp = table.Column<string>(maxLength: 255, nullable: true),
                    Method = table.Column<string>(maxLength: 255, nullable: true),
                    RequestUrl = table.Column<string>(nullable: true),
                    RequestBody = table.Column<string>(nullable: true),
                    RequestQueryString = table.Column<string>(nullable: true),
                    RequestFormData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User = table.Column<string>(maxLength: 255, nullable: true),
                    ClientId = table.Column<string>(maxLength: 255, nullable: true),
                    Logger = table.Column<string>(maxLength: 255, nullable: true),
                    LogTime = table.Column<DateTime>(nullable: false),
                    Level = table.Column<string>(maxLength: 255, nullable: true),
                    Message = table.Column<string>(nullable: true),
                    ClientIp = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User = table.Column<string>(maxLength: 255, nullable: true),
                    ClientId = table.Column<string>(maxLength: 255, nullable: true),
                    LogTime = table.Column<DateTime>(nullable: false),
                    Level = table.Column<string>(maxLength: 255, nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Logger = table.Column<string>(maxLength: 255, nullable: true),
                    ClientIp = table.Column<string>(maxLength: 255, nullable: true),
                    Method = table.Column<string>(maxLength: 255, nullable: true),
                    RequestUrl = table.Column<string>(nullable: true),
                    RequestBody = table.Column<string>(nullable: true),
                    RequestQueryString = table.Column<string>(nullable: true),
                    RequestFormData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User = table.Column<string>(maxLength: 255, nullable: true),
                    ClientId = table.Column<string>(maxLength: 255, nullable: true),
                    Logger = table.Column<string>(maxLength: 255, nullable: true),
                    LogTime = table.Column<DateTime>(nullable: false),
                    Level = table.Column<string>(maxLength: 255, nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    ClientIp = table.Column<string>(maxLength: 255, nullable: true),
                    Method = table.Column<string>(maxLength: 255, nullable: true),
                    RequestUrl = table.Column<string>(nullable: true),
                    RequestBody = table.Column<string>(nullable: true),
                    RequestQueryString = table.Column<string>(nullable: true),
                    RequestFormData = table.Column<string>(nullable: true),
                    Response = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "LoginLogs");

            migrationBuilder.DropTable(
                name: "OperationLogs");

            migrationBuilder.DropTable(
                name: "RequestLogs");
        }
    }
}
