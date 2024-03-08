using System;
namespace ASP_GCH1107_NguyenVanTruong.Models
{
	public class Order
	{
        public int Id { get; set; }
        public int TotalPrice { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public DateTime Submitted { get; set; }
    }
}

