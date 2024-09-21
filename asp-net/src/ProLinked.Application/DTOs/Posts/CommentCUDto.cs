﻿using Microsoft.AspNetCore.Http;

namespace ProLinked.Application.DTOs.Posts;

public class CommentCUDto
{
    public Guid? ParentId;
    public string? Text;
    public IFormFile? Media { get; set; }
}