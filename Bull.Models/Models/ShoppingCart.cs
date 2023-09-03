﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bull.Models.Models;

public class ShoppingCart
{
    public int Id { get; set; }
    public int BookId { get; set; }
    [ForeignKey("BookId")]
    [ValidateNever]
    public Book Book { get; set; }
    [Range(1, 1000, ErrorMessage="Please Enter the value between 1 and 1000")]
    public int Count { get; set; }
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }
    [NotMapped]
    public double Price { get; set; }
}