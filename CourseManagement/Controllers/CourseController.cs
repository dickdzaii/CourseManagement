using CourseManagement.Common;
using CourseManagement.Models;
using CourseManagement.Models.ViewModels;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using CourseManagement.Constants;

namespace CourseManagement.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly string CourseBasePath = "D:\\Images\\Courses";

        public CourseController(DataContext _dataContext)
        {
            this._dataContext = _dataContext;
        }

        [HttpGet]
        public IActionResult GetCourses(string? search = "",
            float? minFee = null,
            float? maxFee = null,
            int? mentorId = null,
            string? orderByColumn = null,
            bool? isAscending = true,
            int? pageIndex = 1,
            int? pageSize = 10)
        {
            try
            {
                var courses = _dataContext.Courses.Where(course =>
                (string.IsNullOrWhiteSpace(search) || course.CourseName.ToLower().Contains(search.ToLower()))
                && ((minFee == null || course.Fee >= minFee) && (maxFee == null || course.Fee <= maxFee))
                && (mentorId == null || course.AccId == mentorId));

                if (!string.IsNullOrWhiteSpace(orderByColumn))
                {
                    courses = courses.OrderBy($"{orderByColumn} {(isAscending.Value == false ? $"{OrderByState.Descending}" : $"{OrderByState.Ascending}")}");

                }

                courses = courses.Skip(pageIndex.Value - 1).Take(pageSize.Value);

                return courses?.Count() > 0 ? Ok(courses) : NotFound("No courses found.");
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

        [HttpPost]
        public IActionResult CreateCourse(Course course)
        {
            try
            {
                course.CourseId = 0;
                course.CourseName = course.CourseName.Trim();
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
                course.CourseName = course.CourseName.Trim();
                _dataContext.Courses.Update(course);
                _dataContext.SaveChanges();

                return Ok(course);
            }
            catch (ValidationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                {
                    Status = "Error",
                    Message = $"{ex.Message}"
                });

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "There was an error when modifying a course."
                });
            }
        }

        [HttpPost]
        [Route("upload-image/{courseId}")]
        public async Task<IActionResult> UploadCourseImage(IFormFile courseImage, int courseId)
        {
            try
            {
                var course = _dataContext.Courses.FirstOrDefault(c => c.CourseId == courseId);
                if (course is null)
                {
                    return NotFound(new Response
                    {
                        Status = "Error",
                        Message = $"Course with id {courseId} is not existed."
                    });
                }

                if (courseImage.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(course.Image))
                    {
                        var oldImagePath = $"{CourseBasePath}\\{course.Image}";

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    if (!Directory.Exists(CourseBasePath))
                    {
                        System.IO.Directory.CreateDirectory(CourseBasePath);
                    }

                    var filePath = $"{CourseBasePath}\\{courseImage.FileName}";
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await courseImage.CopyToAsync(stream);
                    }

                    course.Image = courseImage.FileName;
                    _dataContext.Courses.Update(course);
                    _dataContext.SaveChanges();

                    return Ok(new Response
                    {
                        Status = "Success",
                        Message = $"Uploaded image for course {course.CourseName}."
                    });
                }

                return BadRequest(new Response
                {
                    Status = "Success",
                    Message = $"Invalid uploaded file."
                });

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "There was an error when modifying a course."
                });
            }
        }

        [HttpPost("upload-material/{courseId}")]
        public async Task<IActionResult> UploadCourseMaterial(IEnumerable<IFormFile> files, int courseId)
        {
            try
            {
                var course = _dataContext.Courses.FirstOrDefault(c => c.CourseId == courseId);
                if (course is null)
                {
                    return NotFound(new Response
                    {
                        Status = "Error",
                        Message = $"Course with id {courseId} is not existed."
                    });
                }

                if (!Directory.Exists($"{CourseBasePath}\\CourseMaterials\\{course.CourseName}"))
                {
                    Directory.CreateDirectory($"{CourseBasePath}\\CourseMaterials\\{course.CourseName}");
                }

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var filePath = $"{CourseBasePath}\\CourseMaterials\\{course.CourseName}\\{file.FileName}";
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var material = new CourseMaterial
                        {
                            CourseId = courseId,
                            FilePath = file.FileName,
                            IsActive = true,
                            MaterialTitle = "",
                            FileOrder = 1,
                            ContentType = file.ContentType
                        };

                        _dataContext.CourseMaterials.Add(material);
                    }
                }

                _dataContext.SaveChanges();

                return Ok(new Response
                {
                    Status = "Success",
                    Message = $"Uploaded marterials for course {course.CourseName}."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = $"There was an error when adding course materials. Error: {ex.Message}"
                });
            }
        }

        [HttpDelete]
        [Route("{courseId}")]
        public IActionResult DeleteCourse(int courseId)
        {
            try
            {
                var course = _dataContext.Courses.FirstOrDefault(c => c.CourseId == courseId);
                if (course != null)
                {
                    _dataContext.ValidateDelete(course);
                    _dataContext.Courses.Remove(course);

                    _dataContext.SaveChanges();
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "There was an error when deleting course."
                });
            }
        }

        [HttpPost]
        [Route("enroll/{courseId}")]
        public IActionResult EnrollCourse([FromBody] int userId, int courseId)
        {
            try
            {
                var user = _dataContext.Users.FirstOrDefault(c => c.UserId == userId);
                var course = _dataContext.Courses.FirstOrDefault(c => c.CourseId == courseId);
                if (user is null)
                {
                    return NotFound(new Response
                    {
                        Status = "Error",
                        Message = "User not found."
                    });
                }

                if (course is null)
                {
                    return NotFound(new Response
                    {
                        Status = "Error",
                        Message = "Course not found."
                    });
                }

                var enrollment = new Enrollment
                {
                    CourseId = courseId,
                    AccId = userId,
                    TotalHour = 0
                };

                // todo: add enrollment validation
                _dataContext.Enrollments.Add(enrollment);
                _dataContext.SaveChanges();

                return Ok($"User {user.UserName} has enrolled course {course.CourseName}.");
            }
            catch (ValidationException ex)
            {
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = $"{ex.Message}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = $"There was an error when enrolling course. Error: {ex.Message}"
                });
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
