using Bull.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bull.DataAccess.Repository.IRepository;

public interface IApplicationUserRepository: IRepository<ApplicationUser>
{
}