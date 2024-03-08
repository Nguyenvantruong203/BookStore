using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ASP_GCH1107_NguyenVanTruong.Data
{
	public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        public DbSet<ASP_GCH1107_NguyenVanTruong.Models.Category> Category { get; set; } = default!;
        public DbSet<ASP_GCH1107_NguyenVanTruong.Models.Product> Product { get; set; } = default!;
        public DbSet<ASP_GCH1107_NguyenVanTruong.Models.Order> Order { get; set; } = default!;
    }
}

