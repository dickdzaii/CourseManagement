using CourseManagement.Models;
using System.Net;

namespace CourseManagement.Middlewares
{
    public class CourseMiddleware
    {
        internal const string ROUTE_KEY = "courseId";

        readonly RequestDelegate _next;

        public CourseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DataContext dbContext)
        {
            context.Request.RouteValues.TryGetValue(ROUTE_KEY, out var value);

            if (value is not null)
            {
                var validId = int.TryParse(value.ToString(), out var courseId);
                if (!validId)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                var vertex = dbContext.Courses.FirstOrDefault(c => c.CourseId == courseId);

                if (vertex is null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

            }

            await _next(context);
        }
    }
}
