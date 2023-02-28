using System;
using System.Linq.Expressions;

namespace Entity.Specification
{
  public class CoursesWithCategoriesSpec : BaseSpecification<Course>
  {
    public CoursesWithCategoriesSpec(CourseParams courseParams) : base(x =>
    (string.IsNullOrEmpty(courseParams.Search) || x.Title.ToLower().Contains(courseParams.Search)) &&
    (!courseParams.CategoryId.HasValue || x.CategoryId == courseParams.CategoryId) && (x.Published == true))
    {
      IncludeMethod(x => x.Category);
      IncludeMethod(c => c.Requirements);
      IncludeMethod(c => c.Learnings);
      ApplyPagination(courseParams.PageSize, courseParams.PageSize * (courseParams.PageIndex - 1));

      if (!string.IsNullOrEmpty(courseParams.Sort))
      {
        switch (courseParams.Sort)
        {
          case "priceAsc":
            SortMethod(c => c.Price);
            break;
          case "priceDesc":
            SortByDescMethod(c => c.Price);
            break;
          default:
            SortMethod(c => c.Title);
            break;
        }
      }
    }

    public CoursesWithCategoriesSpec(Guid id) : base(x => x.Id == id)
    {
      IncludeMethod(c => c.Requirements);
      IncludeMethod(c => c.Learnings);
      IncludeMethod(c => c.Category);
      SortMethod(x => x.Id);
    }
  }
}