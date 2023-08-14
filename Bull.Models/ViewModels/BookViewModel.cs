using Bull.Models.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bull.Models.ViewModels;

public class BookViewModel
{
    public Book Book { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> CategoryList { get; set; }
}