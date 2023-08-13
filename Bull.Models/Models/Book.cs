﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bull.Models.Models;

public class Book
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string? Title { get; set; }
    public string? Description { get; set; }
    
    [Required]
    public string? Isbn { get; set; }
    
    [Required]
    public string? Author { get; set; }
    
    [Required]
    [DisplayName("List Price")]
    [Range(1, 10000)]
    public double ListPrice { get; set; }
    
    [Required]
    [DisplayName("Price for 1-50")]
    [Range(1, 10000)]
    public double Price { get; set; }
    
    [Required]
    [DisplayName("Price for 50+")]
    [Range(1, 10000)]
    public double Price50 { get; set; }
    
    [Required]
    [DisplayName("Price for 100+")]
    [Range(1, 10000)]
    public double Price100 { get; set; }

    public int CategoryId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    [ForeignKey("CategoryId")]
    public Category Category { get; set; }
}