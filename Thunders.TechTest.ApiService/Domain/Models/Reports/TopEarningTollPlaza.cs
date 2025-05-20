using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Thunders.TechTest.ApiService.Domain.Enums;

namespace Thunders.TechTest.ApiService.Domain.Models.Reports;

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
    public int TollPlazaId { get; set; }

    [ForeignKey("TollPlazaId")]
    public TollPlaza TollPlaza { get; set; } = null!;

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
    public int Rank { get; set; }

    [Required]
    public ReportStatus Status { get; set; }
}