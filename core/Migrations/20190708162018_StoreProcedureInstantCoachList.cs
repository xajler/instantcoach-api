using Microsoft.EntityFrameworkCore.Migrations;

namespace core.Migrations
{
    public partial class StoreProcedureInstantCoachList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE [dbo].[InstantCoach_List]
	@Skip INT,
	@Take INT,
	@ShowCompleted BIT
AS
	WITH Result (RowNumber, Id, [Status], Reference, [Description], CreatedAt, UpdateAt, CommentsCount, EvaluatorName) AS
	(
		SELECT ROW_NUMBER() OVER (ORDER BY ic.CreatedAt DESC, ic.Status ASC) AS 'RowNumber',
		       ic.Id, ic.[Status], ic.Reference, ic.Description, ic.CreatedAt, ic.UpdatedAt, ic.CommentsCount, ic.EvaluatorName
		FROM InstantCoaches ic
        WHERE  (@ShowCompleted = 0 AND ic.[Status] < 5) OR (@ShowCompleted = 1 AND 1 = 1)
	)

	SELECT (SELECT CAST(MAX(RowNumber) AS INT) FROM Result) AS TotalCount, Id, [Status], Reference, [Description], CreatedAt, UpdateAt, CommentsCount, EvaluatorName
	FROM Result
	ORDER BY CreatedAt DESC, [Status] ASC
	OFFSET (@Skip * @Take) ROWS
	FETCH NEXT @Take ROWS ONLY
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[InstantCoach_List]");
        }
    }
}
