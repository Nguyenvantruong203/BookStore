using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_GCH1107_NguyenVanTruong.Models
{
	public class Product
	{
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Author { get; set; }

        public decimal Price { get; set; }

        public string? ProfilePicture { get; set; }
        [NotMapped]
        public IFormFile ProfileImage { get; set; }

        public int CategoryID { get; set; }
        public virtual Category? Category { get; set; }
    }
}

