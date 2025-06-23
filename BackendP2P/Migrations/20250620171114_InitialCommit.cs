using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendP2P.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
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
                name: "Assessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assessments_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assessments_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Students = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Topic = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaterialReadId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialWorkId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_AuthorId",
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
                name: "CommentModel",
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
                    table.PrimaryKey("PK_CommentModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentModel_Tasks_TaskModelId",
                        column: x => x.TaskModelId,
                        principalTable: "Tasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaterialReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialReads_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialWorks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Instructions = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialWorks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialWorks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
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
                    AttachmentPath = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solutions_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Solutions_Users_StudentId",
                        column: x => x.StudentId,
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
                    Level = table.Column<string>(type: "text", nullable: false),
                    MaterialWorkModelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriaAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriteriaAssignments_MaterialWorks_MaterialWorkModelId",
                        column: x => x.MaterialWorkModelId,
                        principalTable: "MaterialWorks",
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
                    Content = table.Column<string>(type: "text", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttachmentPath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionChecks", x => x.Id);
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
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SolutionCheckId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grades_SolutionChecks_SolutionCheckId",
                        column: x => x.SolutionCheckId,
                        principalTable: "SolutionChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grades_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Remarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SolutionCheckId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Remarks_SolutionChecks_SolutionCheckId",
                        column: x => x.SolutionCheckId,
                        principalTable: "SolutionChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Remarks_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_StudentId",
                table: "Assessments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_TeacherId",
                table: "Assessments",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentModel_TaskModelId",
                table: "CommentModel",
                column: "TaskModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaAssignments_MaterialWorkModelId",
                table: "CriteriaAssignments",
                column: "MaterialWorkModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_SolutionCheckId",
                table: "Grades",
                column: "SolutionCheckId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grades_StudentId",
                table: "Grades",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_TaskId",
                table: "Grades",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_TeacherId",
                table: "Grades",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReads_TaskId",
                table: "MaterialReads",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialWorks_TaskId",
                table: "MaterialWorks",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Remarks_AuthorId",
                table: "Remarks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Remarks_SolutionCheckId",
                table: "Remarks",
                column: "SolutionCheckId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_AuthorId",
                table: "SolutionChecks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_SolutionId",
                table: "SolutionChecks",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_StudentId",
                table: "Solutions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_TaskId",
                table: "Solutions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AuthorId",
                table: "Tasks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CourseId",
                table: "Tasks",
                column: "CourseId");

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
                name: "CommentModel");

            migrationBuilder.DropTable(
                name: "CriteriaAssignments");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "MaterialReads");

            migrationBuilder.DropTable(
                name: "Remarks");

            migrationBuilder.DropTable(
                name: "UsersCorses");

            migrationBuilder.DropTable(
                name: "MaterialWorks");

            migrationBuilder.DropTable(
                name: "SolutionChecks");

            migrationBuilder.DropTable(
                name: "Solutions");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
