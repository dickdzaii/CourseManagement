using CourseManagement.Common;
using CourseManagement.Models;
using CourseManagement.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Controllers
{
	[Route("api/[controller]s")]
	[ApiController]
	public class CourseController : ControllerBase
	{
		private readonly DataContext _dataContext;

		public CourseController(DataContext _dataContext)
		{
			this._dataContext = _dataContext;
		}

		[HttpGet()]
		public IActionResult GetCourses(string? search = "",
			float? minFee = null,
			float? maxFee = null,
			int? mentorId = null,
			int? pageIndex = 1,
			int? pageSize = 10)
		{
			try
			{
				var courses = _dataContext.Courses.Where(course =>
				(string.IsNullOrWhiteSpace(search) || course.CourseName.ToLower().Contains(search.ToLower()))
				&& ((minFee == null || course.Fee >= minFee) && (maxFee == null || course.Fee <= maxFee))
				&& (mentorId == null || course.AccId == mentorId))
				.Skip(pageIndex.Value - 1).Take(pageSize.Value);

				return courses?.Count() > 0 ? Ok(courses) : NotFound("No course found");
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = $"there was an error when getting the course. Error: {ex.Message}" });
			}

		}

		[HttpGet("{id}")]
		public IActionResult GetCourseById(int id)
		{
			var course = _dataContext.Courses.FirstOrDefault(c => c.CourseId == id);

			return course != null ? Ok(course) : NotFound(new Response { Status = "Error", Message = "Could not find the course." });
		}

		[HttpPost("course")]
		public IActionResult CreateCourse(Course course)
		{
			try
			{
				course.CourseId = 0;
                _dataContext.ValidateUpsert(course);
				_dataContext.Courses.Add(course);
				_dataContext.SaveChanges();
				return Ok(course);
			}
            catch (ValidationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = $"{ex.Message}" });

            }
            catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = $"There was an error when creating a course.Error: {ex.Message}" });
				throw;
			}
		}

		[HttpPut]
		[Route("{id}")]
		public IActionResult UpdateCourse(Course course)
		{
            try
            {
				var currentCourse = _dataContext.Courses.FirstOrDefault(c => c.CourseId == course.CourseId);
                _dataContext.Courses.Entry(currentCourse).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                _dataContext.ValidateUpsert(course, currentCourse);

                _dataContext.Courses.Update(course);
                _dataContext.SaveChanges();

                return Ok(course);
            }
            catch (ValidationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = $"{ex.Message}" });

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "There was an error when modifying a course." });
                throw;
            }
        }

        [HttpDelete]
        [Route("{courseId}")]
        public IActionResult DeleteCourse(int courseId)
        {
            try
            {
				var course = _dataContext.Courses.FirstOrDefault( c => c.CourseId == courseId);
				if (course != null)
				{
					_dataContext.Courses.Remove(course);
                    _dataContext.SaveChanges();
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "There was an error when deleting a course." });
                throw;
            }
        }

		[HttpPost]
		[Route("enroll/{courseId}")]
		public IActionResult EnrollmentCourse([FromBody] int userId, int courseId)
		{
			try
			{
				return Ok();
			}
			catch (Exception)
			{
				throw;
			}
		}

		[HttpPost]
		[Route("payment")]
		public IActionResult Payment()
		{
			return Ok();
		}
    }
}
