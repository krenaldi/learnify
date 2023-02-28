using System;
using System.Linq.Expressions;

namespace Entity.Specification
{
  public class CategoriesWithCoursesSpec : BaseSpecification<Category>
  {
    public CategoriesWithCoursesSpec(int id) : base(x => x.Id == id)
    {
      IncludeMethod(c => c.Courses);
      SortMethod(x => x.Id);

    }
  }
}