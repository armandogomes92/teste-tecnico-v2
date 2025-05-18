using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunders.TechTest.ApiService.Models.Reports
{
    [Table("ReportTopEarningTollPlazas")]
    public class TopEarningTollPlaza
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        [MaxLength(100)]
        public string TollPlaza { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public int Rank { get; set; } // Rank of the toll plaza for that month
    }
}