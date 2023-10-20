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
		public virtual DbSet<CoursePayment> CoursePayments { get; set; }

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
                entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId);
                entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId);
            });

			modelBuilder.Entity<Course>(entity =>
			{
				entity.ToTable("Course");
				entity.HasOne(d => d.Mentor).WithMany(p => p.Mentors)
				.HasForeignKey(d => d.AccId);
            });

            modelBuilder.Entity<CourseMaterial>(entity =>
            {
                entity.ToTable("CourseMaterial");
                entity.HasOne(d => d.Course).WithMany(p => p.CourseMaterials)
                .HasForeignKey(d => d.CourseId);
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollment");
                entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId);
                entity.HasOne(d => d.Customer).WithMany(p => p.Customers)
                .HasForeignKey(d => d.AccId);
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");
                entity.HasOne(d => d.Enrollment).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.EnrollmentId);
            });

            modelBuilder.Entity<CoursePayment>(entity =>
            {
                entity.ToTable("CoursePayment");
                entity.HasOne(d => d.Course).WithMany(p => p.CoursePayments)
                .HasForeignKey(d => d.CourseId);
                entity.HasOne(d => d.Customer).WithMany(p => p.CustomerPayments)
                .HasForeignKey(d => d.AccId);
            });
        }
	}
}
