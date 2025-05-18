using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunders.TechTest.ApiService.Models.Reports
{
    [Table("ReportHourlyCityRevenue")]
    public class HourlyCityRevenue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public DateOnly ReportDate { get; set; } // YYYY-MM-DD

        [Required]
        public int Hour { get; set; } // 0-23

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(2)]
        public string State { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }
    }
}