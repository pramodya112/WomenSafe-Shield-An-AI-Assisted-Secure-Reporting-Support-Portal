using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WomenEmpower.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbuseConfidenceScore",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "AnalyzedAt",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "SentimentLabel",
                table: "AIAnalyses");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AIAnalyses",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Reports",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Reports",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReportId",
                table: "Evidences",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Evidences",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ReportId",
                table: "AIAnalyses",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AnalysedAt",
                table: "AIAnalyses",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "DetectedLanguage",
                table: "AIAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecommendedAction",
                table: "AIAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskLevel",
                table: "AIAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SentimentScore",
                table: "AIAnalyses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "AIAnalyses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "AnalysedAt",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "DetectedLanguage",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "RecommendedAction",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "RiskLevel",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "SentimentScore",
                table: "AIAnalyses");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "AIAnalyses");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AIAnalyses",
                newName: "id");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reports",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReportId",
                table: "Evidences",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Evidences",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReportId",
                table: "AIAnalyses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "AbuseConfidenceScore",
                table: "AIAnalyses",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "AnalyzedAt",
                table: "AIAnalyses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SentimentLabel",
                table: "AIAnalyses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
