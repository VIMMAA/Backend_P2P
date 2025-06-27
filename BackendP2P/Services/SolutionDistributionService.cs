using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Domain.Enums;

public class SolutionDistributionService
{
    private readonly ApplicationContext _context;

    public SolutionDistributionService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task DistributeSolutionsAfterDeadline()
    {

        var expiredMaterialWorks = await _context.MaterialWorks
        .Where(mw => mw.Deadline < DateTime.UtcNow)
        .Include(mw => mw.CriteriaAssignments)      
        .ToListAsync();


        foreach (var task in expiredMaterialWorks)
        {
            if (!task.SolutionsDistributed)
            {
                await DistributeSolutionsForTask(task);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task DistributeSolutionsForTask(MaterialWorkModel task)
    {
        var IsTeacher = false;
       

        var solutions = await _context.Solutions
        .Where(s => s.TaskId == task.Id)  
        .Include(s => s.Student)         
        .Include(s => s.AttachedFiles)    
        .ToListAsync();

        var onTimeSolutions = solutions.Where(s => s.SubmissionTime <= task.Deadline).ToList();
        var lateSolutions = solutions.Where(s => s.SubmissionTime > task.Deadline).ToList();

        var submitters = onTimeSolutions
            .Select(s => s.Student)
            .ToList();

        var checkAssignments = new List<SolutionForCheckModel>();

            if (task.Check == Check.P2P && onTimeSolutions.Count > 1 && (submitters.Count - 1 >= task.SolutionsToCheckN))
            {
                foreach (var solution in onTimeSolutions)
                {
                    var availableReviewers = submitters
                        .Where(u => u.Id != solution.StudentId)
                        .OrderBy(x => Guid.NewGuid())
                        .ToList();

                    var existingChecks = checkAssignments
                        .Count(c => c.SolutionId == solution.Id);

                    var neededChecks = task.SolutionsToCheckN - existingChecks;
                    

                    for (int i = 0; i < neededChecks && i < availableReviewers.Count; i++)
                {
                     var newAssessments = task.CriteriaAssignments
                    .Select(criteria => new AssessmentModel
                    {
                        Id = Guid.NewGuid(),
                        Score = 0, 
                        MaxScore = criteria.CountScore, 
                        Remark = criteria.Conditions, 
                    })
                    .ToList();
                    checkAssignments.Add(new SolutionForCheckModel
                    {
                        Assements = newAssessments,
                        Id = Guid.NewGuid(),
                        SolutionId = solution.Id,
                        AuthortId = availableReviewers[i].Id,
                        DueTime = DateTime.UtcNow.AddDays(7),
                        Comment = string.Empty
                    });
                }
                }
            }
        else
        {
            IsTeacher = true;
            lateSolutions = solutions;
        }

        foreach (var lateSolution in lateSolutions)
            {
                 var newAssessments = task.CriteriaAssignments
                .Select(criteria => new AssessmentModel
                {
                    Id = Guid.NewGuid(),
                    MaxScore = criteria.CountScore, 
                    Remark = criteria.Conditions, 
                })
                .ToList();

                checkAssignments.Add(new SolutionForCheckModel
                {
                    Assements = newAssessments,
                    Id = Guid.NewGuid(),
                    SolutionId = lateSolution.Id,
                    AuthortId = task.AuthorId,
                    DueTime = DateTime.UtcNow.AddDays(7),
                    Comment = ""
                });
                
            }

        await _context.PackageChecks.AddAsync(new PackageCheckModel
        {
            Id = Guid.NewGuid(),
            SolutionForCheckTasks = checkAssignments,
            TaskId = task.Id,
            Deadline = DateTime.UtcNow.AddDays(7),
            IsTeacher = IsTeacher,
            IsProcessed = false
        });

        task.SolutionsDistributed = true;
    }
}