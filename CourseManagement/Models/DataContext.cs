using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Models
{
	public class DataContext : DbContext
	{
		public DataContext()
		{
		}

		public DataContext(DbContextOptions<DataContext> options)
			: base(options)
		{

		}

		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<Role> Roles { get; set; }
		public virtual DbSet<UserRole> UserRoles { get; set; }
		public virtual DbSet<Course> Courses { get; set; }
		public virtual DbSet<CourseMaterial> CourseMaterials { get; set; }
		public virtual DbSet<Enrollment> Enrollments { get; set; }
		public virtual DbSet<Feedback> Feedbacks { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer("Name=ConnectionStrings:ConnStr");
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<User>(entity =>
			{
				entity.ToTable("User");
			});

			modelBuilder.Entity<Role>(entity =>
			{
				entity.ToTable("Role");
			});

			modelBuilder.Entity<UserRole>(entity =>
			{
				entity.ToTable("UserRole");
			});

			modelBuilder.Entity<Course>(entity =>
			{
				entity.ToTable("Course");
			});

            modelBuilder.Entity<CourseMaterial>(entity =>
            {
                entity.ToTable("CourseMaterial");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollment");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");
            });
        }
	}
}
