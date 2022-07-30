using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Alexandria.Api.Data
{
    public partial class AlexandriaDbContext : IdentityDbContext<ApiUser> // Comment: Identity context is binded to ApiUser which inherits from default IdentityUser
    {
        public AlexandriaDbContext()
        {
        }

        public AlexandriaDbContext(DbContextOptions<AlexandriaDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; } = null!;
        public virtual DbSet<Book> Books { get; set; } = null!;

        #region Legacy - It is already included in Program.cs
        /*        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-BIQA9C7;Database=AlexandriaDb;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }*/ 
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Comment: This is needed because we are now inheriting from IdentityDbContext and not DbContext (default)

            modelBuilder.Entity<Author>(entity =>
            {
                entity.Property(e => e.Bio).HasMaxLength(250);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EA2CB7BA9D")
                    .IsUnique();

                entity.Property(e => e.Image).HasMaxLength(100);

                entity.Property(e => e.Isbn)
                    .HasMaxLength(50)
                    .HasColumnName("ISBN");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Summary).HasMaxLength(250);

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("FK_Books_Authors");
            });

            #region Seed Roles
            // Comment: Seed Roles
            modelBuilder.Entity<IdentityRole>().HasData(
                    new IdentityRole
                    {
                        Name = "User",
                        NormalizedName = "USER",
                        Id = "e4225d87-3b1c-41bf-bfb6-34a2c238c6cb"
                    },
                    new IdentityRole
                    {
                        Name = "Administrator",
                        NormalizedName = "ADMINISTRATOR",
                        Id = "c868aebb-0d51-4c91-8f47-9533d6ceecdd"
                    }
                );
            #endregion

            #region Seed Users
            // Comment: Seed Users
            var passwordHasher = new PasswordHasher<ApiUser>(); // Comment: Api User Password Hasher
            modelBuilder.Entity<ApiUser>().HasData(
                    new ApiUser { 
                        Id = "31b9ecdc-05e1-42b9-ae4a-965bc7d0aebf",
                        Email = "admin@alexandria.com",
                        NormalizedEmail = "ADMIN@ALEXANDRIA.COM",
                        UserName = "admin@alexandria.com",
                        NormalizedUserName = "ADMIN@ALEXANDRIA.COM",
                        FirstName = "Kibonga",
                        LastName = "Kimur",
                        PasswordHash = passwordHasher.HashPassword(null, "!Kibonga01") 
                    },
                    new ApiUser { 
                        Id = "3d7f652f-c44d-4d48-9231-0b4f8ea1e89a",
                        Email = "user@alexandria.com",
                        NormalizedEmail = "USER@ALEXANDRIA.COM",
                        UserName = "user@alexandria.com",
                        NormalizedUserName = "USER@ALEXANDRIA.COM",
                        FirstName = "Suga",
                        LastName = "Sean",
                        PasswordHash = passwordHasher.HashPassword(null, "!Kibonga01")
                    }
                );
            #endregion

            #region Assign Users to Roles
            // Comment: Assign users to roles
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "c868aebb-0d51-4c91-8f47-9533d6ceecdd",
                    UserId = "31b9ecdc-05e1-42b9-ae4a-965bc7d0aebf"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "e4225d87-3b1c-41bf-bfb6-34a2c238c6cb",
                    UserId = "3d7f652f-c44d-4d48-9231-0b4f8ea1e89a"
                }
            );
            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
