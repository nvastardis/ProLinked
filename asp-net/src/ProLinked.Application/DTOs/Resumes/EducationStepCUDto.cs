﻿namespace ProLinked.Application.DTOs.Resumes;

public class EducationStepCUDto
{
    public Guid ResumeId;
    public string School = null!;
    public string? Degree;
    public string? FieldOfStudy;
    public string? Grade;
    public string? Activities;
    public string? Description;
    public DateTime? StartDate;
    public DateTime? EndDate;
    public string[]? Skills;
}