using CourseManagement.Common;
using CourseManagement.Models;
using CourseManagement.Models.ViewModels;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using CourseManagement.Constants;
using CourseManagement.Models.DataTransferObject;

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

            if (course == null)
            {
                return NotFound(new Response { Status = "Error", Message = "Could not find the course." });
            }

            var mentor = _dataContext.Users.FirstOrDefault(u => u.UserId == course.AccId);
            if (mentor is null)
            {
                return NotFound(new Response { Status = "Error", Message = "Could not find mentor." });
            }

            course.Mentor = mentor;

            return Ok(course);
        }

        [HttpPost]
        public IActionResult CreateCourse(CourseDto course)
        {
            try
            {
                var newCourse = new Course
                {
                    AccId = course.AccId,
                    CourseName = course.CourseName.Trim(),
                    CreatedDate = DateTime.Now,
                    Fee = course.Fee,
                    Status = course.Status.HasValue ? course.Status.Value : true,
                    Image = course.Image,
                    EnrollmentCount = course.EnrollmentCount ?? 1,
                };

                _dataContext.ValidateUpsert(newCourse);
                _dataContext.Courses.Add(newCourse);
                _dataContext.SaveChanges();

                return Ok(newCourse);
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
        [Route("{courseId}")]
        public IActionResult UpdateCourse(CourseDto course, int courseId)
        {
            try
            {
                var currentCourse = _dataContext.Courses.FirstOrDefault(c => c.CourseId == courseId);
                if (currentCourse == null)
                {
                    return NotFound(new Response
                    {
                        Status = "Error",
                        Message = $"Course with id {courseId} is not existed."
                    });
                }
                _dataContext.Courses.Entry(currentCourse).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                var updatedCourse = new Course
                {
                    CourseId = courseId,
                    AccId = course.AccId,
                    CourseName = course.CourseName.Trim(),
                    Fee = course.Fee,
                    Status = course.Status.HasValue ? course.Status.Value : true,
                    Image = course.Image,
                    EnrollmentCount = course.EnrollmentCount ?? 1,
                    CreatedDate = currentCourse.CreatedDate
                };


                _dataContext.ValidateUpsert(updatedCourse, currentCourse);
                course.CourseName = course.CourseName.Trim();
                _dataContext.Courses.Update(updatedCourse);
                _dataContext.SaveChanges();

                return Ok(updatedCourse);
            }
            catch (ValidationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
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
                    Message = $"There was an error when modifying a course. Error: {ex.Message}"
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
                        Message = $"Uploaded image for course {course.CourseName}.",
                        Object = course
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

        [HttpPost("materials/{courseId}")]
        public async Task<IActionResult> UploadCourseMaterials(IEnumerable<IFormFile> files, int courseId)
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

                if (!Directory.Exists($"{CourseBasePath}\\CourseMaterials\\{course.CourseId}"))
                {
                    Directory.CreateDirectory($"{CourseBasePath}\\CourseMaterials\\{course.CourseId}");
                }

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var filePath = $"{CourseBasePath}\\CourseMaterials\\{course.CourseId}\\{file.FileName}";
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

        [HttpGet]
        [Route("materials/{courseId}")]
        public IActionResult GetCourseMaterials(int courseId)
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

                var materials = _dataContext.CourseMaterials.Where(m => m.CourseId == courseId);
                var materialsView = materials.Select(x => new CourseMaterialViewModel
                {
                    CourseMaterialId = x.CourseMaterialId,
                    MaterialTitle = x.MaterialTitle,
                    MaterialDescription = x.MaterialDescription,
                    FilePath = x.FilePath,
                    ContentType = x.ContentType,
                    FileOrder = x.FileOrder,
                    IsActive = x.IsActive,
                    CreatedDate = x.CreatedDate,
                    CourseId = x.CourseId,
                    Course = course,
                    FullPath = $"{CourseBasePath}\\{x.FilePath}",
                });

                return Ok(materialsView);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = $"There was an error when getting course's materials. Error {ex.Message}."
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
                if (course == null)
                {
                    return NotFound(new Response
                    {
                        Status = "Error",
                        Message = $"Course with id {courseId} is not existed."
                    });
                }

                _dataContext.ValidateDelete(course);
                _dataContext.Courses.Remove(course);

                _dataContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = $"There was an error when deleting course. Error {ex.Message}."
                });
            }
        }

        [HttpPost]
        [Route("payment/{courseId}")]
        public IActionResult Payment(PaymentModel paymentModel, int courseId)
        {
            try
            {
                var course = _dataContext.Courses.FirstOrDefault(c => c.CourseId == courseId);
                var user = _dataContext.Users.FirstOrDefault(c => c.UserId == paymentModel.UserId);
                if (course == null)
                {
                    return NotFound(new Response
                    {
                        Status = "Error",
                        Message = $"Course with id {courseId} is not existed."
                    });
                }

                if (user is null)
                {
                    return NotFound(new Response
                    {
                        Status = "Error",
                        Message = $"User with id {paymentModel.UserId} is not existed."
                    });
                }

                if (_dataContext.CoursePayments.Any(p => p.CourseId == course.CourseId && p.AccId == user.UserId))
                {
                    return BadRequest(new Response
                    {
                        Status = "Error",
                        Message = $"User {user.UserName} has already pay for this course."
                    });
                }

                var validTos = paymentModel.ValidTo.Split('/').Select(x => int.Parse(x));
                var validToDate = new DateTime(validTos.Last(), validTos.First(), 1, 0, 0, 0);
                // compare the current date time with the last day of the month in valid to 
                if (validToDate.AddMonths(1).AddSeconds(-1) <= DateTime.Now)
                {
                    return BadRequest(new Response
                    {
                        Status = "Error",
                        Message = $"The card's valid date is in the past."
                    });
                }

                var payment = new CoursePayment
                {
                    CourseId = courseId,
                    AccId = user.UserId,
                    PaymentDate = DateTime.Now
                };

                _dataContext.CoursePayments.Add(payment);
                _dataContext.SaveChanges();

                return Ok(payment);
            }
            catch (ValidationException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
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
                    Message = $"There was an error when paying course. Error: {ex.Message}."
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
                _dataContext.ValidateUpsert(enrollment);
                _dataContext.Enrollments.Add(enrollment);
                _dataContext.SaveChanges();

                return Ok($"User {user.UserName} has succeeded enrolled course {course.CourseName}.");
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
    }
}
