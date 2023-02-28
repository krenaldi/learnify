using System;
using System.Linq.Expressions;

namespace Entity.Specification
{
  public class CoursesFiltersCountSpec : BaseSpecification<Course>
  {
    public CoursesFiltersCountSpec(CourseParams courseParams) : base(x =>
    (string.IsNullOrEmpty(courseParams.Search) || x.Title.ToLower().Contains(courseParams.Search)) && (!courseParams.CategoryId.HasValue || x.CategoryId == courseParams.CategoryId))
    {
    }
  }
}