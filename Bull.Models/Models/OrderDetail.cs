﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bull.Models.Models;

public class OrderDetail
{
    public int Id { get; set; }
    [Required]
    public int OrderHeaderId { get; set; }
    [ForeignKey("OrderHeaderId")]
    [ValidateNever]
    public OrderHeader OrderHeader { get; set; }

    public int BookId { get; set; }
    [ForeignKey("BookId")]
    [ValidateNever]
    public Book Book { get; set; }

    public int Count { get; set; }
    public double Price { get; set; }
}