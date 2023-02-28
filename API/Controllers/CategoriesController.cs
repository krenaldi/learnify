using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dto;
using AutoMapper;
using Entity;
using Entity.Interfaces;
using Entity.Specification;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class CategoriesController : BaseController
  {
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Category> _repository;
    public CategoriesController(IGenericRepository<Category> repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]

    public async Task<ActionResult<IReadOnlyList<Category>>> GetCategories()
    {
      var categories = await _repository.ListAllAsync();

      return Ok(_mapper.Map<IReadOnlyList<Category>, IReadOnlyList<CategoriesDto>>(categories));
    }

    [HttpGet("{id}")]

    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
      var spec = new CategoriesWithCoursesSpec(id);

      var category = await _repository.GetEntityWithSpec(spec);

      return _mapper.Map<Category, CategoryDto>(category);
    }
  }
}