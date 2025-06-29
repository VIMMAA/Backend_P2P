using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeachersCode = table.Column<string>(type: "text", nullable: false),
                    StudentsCode = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    Chapter = table.Column<string>(type: "text", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    Audience = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackageChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    IsTeacher = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageChecks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Birthday = table.Column<DateOnly>(type: "date", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TaskModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Remark = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grades_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grades_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaterialReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialReads_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialReads_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialWorks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Check = table.Column<int>(type: "integer", nullable: false),
                    Penalty = table.Column<double>(type: "double precision", nullable: false),
                    SolutionsToCheckN = table.Column<int>(type: "integer", nullable: false),
                    SolutionsDistributed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialWorks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialWorks_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialWorks_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Remarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Remarks_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersCorses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersCorses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersCorses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersCorses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriteriaAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Conditions = table.Column<string>(type: "text", nullable: false),
                    CountScore = table.Column<int>(type: "integer", nullable: false),
                    MaterialWorkModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    GradeModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriaAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriteriaAssignments_Grades_GradeModelId",
                        column: x => x.GradeModelId,
                        principalTable: "Grades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CriteriaAssignments_MaterialWorks_MaterialWorkModelId",
                        column: x => x.MaterialWorkModelId,
                        principalTable: "MaterialWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Solutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialWorkModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solutions_MaterialWorks_MaterialWorkModelId",
                        column: x => x.MaterialWorkModelId,
                        principalTable: "MaterialWorks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Solutions_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttachedFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialWorkId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialReadId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReadModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachedFiles_MaterialReads_ReadModelId",
                        column: x => x.ReadModelId,
                        principalTable: "MaterialReads",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttachedFiles_MaterialWorks_WorkModelId",
                        column: x => x.WorkModelId,
                        principalTable: "MaterialWorks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttachedFiles_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Numb = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Theme = table.Column<string>(type: "text", nullable: false),
                    StudentRep = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolutionChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    GradeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RemarkId = table.Column<Guid>(type: "uuid", nullable: true),
                    AttachmentPath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolutionChecks_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolutionChecks_Remarks_RemarkId",
                        column: x => x.RemarkId,
                        principalTable: "Remarks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolutionChecks_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolutionChecks_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolutionForChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    AuthortId = table.Column<Guid>(type: "uuid", nullable: false),
                    DueTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsChecked = table.Column<bool>(type: "boolean", nullable: false),
                    PackageCheckModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionForChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolutionForChecks_PackageChecks_PackageCheckModelId",
                        column: x => x.PackageCheckModelId,
                        principalTable: "PackageChecks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolutionForChecks_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: true),
                    MaxScore = table.Column<int>(type: "integer", nullable: false),
                    Remark = table.Column<string>(type: "text", nullable: false),
                    SolutionForCheckModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assessments_SolutionForChecks_SolutionForCheckModelId",
                        column: x => x.SolutionForCheckModelId,
                        principalTable: "SolutionForChecks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_SolutionForCheckModelId",
                table: "Assessments",
                column: "SolutionForCheckModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachedFiles_ReadModelId",
                table: "AttachedFiles",
                column: "ReadModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachedFiles_SolutionId",
                table: "AttachedFiles",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachedFiles_WorkModelId",
                table: "AttachedFiles",
                column: "WorkModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TaskModelId",
                table: "Comments",
                column: "TaskModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaAssignments_GradeModelId",
                table: "CriteriaAssignments",
                column: "GradeModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaAssignments_MaterialWorkModelId",
                table: "CriteriaAssignments",
                column: "MaterialWorkModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_StudentId",
                table: "Grades",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_TeacherId",
                table: "Grades",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReads_AuthorId",
                table: "MaterialReads",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReads_CourseId",
                table: "MaterialReads",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialWorks_AuthorId",
                table: "MaterialWorks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialWorks_CourseId",
                table: "MaterialWorks",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Remarks_AuthorId",
                table: "Remarks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SolutionId",
                table: "Reports",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_AuthorId",
                table: "SolutionChecks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_GradeId",
                table: "SolutionChecks",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_RemarkId",
                table: "SolutionChecks",
                column: "RemarkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_SolutionId",
                table: "SolutionChecks",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionForChecks_PackageCheckModelId",
                table: "SolutionForChecks",
                column: "PackageCheckModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionForChecks_SolutionId",
                table: "SolutionForChecks",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_MaterialWorkModelId",
                table: "Solutions",
                column: "MaterialWorkModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_StudentId",
                table: "Solutions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_TaskId",
                table: "Solutions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersCorses_CourseId",
                table: "UsersCorses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersCorses_UserId",
                table: "UsersCorses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assessments");

            migrationBuilder.DropTable(
                name: "AttachedFiles");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "CriteriaAssignments");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "SolutionChecks");

            migrationBuilder.DropTable(
                name: "UsersCorses");

            migrationBuilder.DropTable(
                name: "SolutionForChecks");

            migrationBuilder.DropTable(
                name: "MaterialReads");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Remarks");

            migrationBuilder.DropTable(
                name: "PackageChecks");

            migrationBuilder.DropTable(
                name: "Solutions");

            migrationBuilder.DropTable(
                name: "MaterialWorks");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
