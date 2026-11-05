using Rest.Application.Dtos.MenuDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Interfaces.IServices
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuCategoryDto>> GetMenuAsync();
    }
}
