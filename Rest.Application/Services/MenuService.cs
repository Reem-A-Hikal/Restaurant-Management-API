using AutoMapper;
using Rest.Application.Dtos.MenuDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MenuService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MenuCategoryDto>> GetMenuAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetMenuAsync();
            return _mapper.Map<IEnumerable<MenuCategoryDto>>(categories);
        }
    }
}
