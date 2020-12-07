using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nummus.Data {
    public partial class BookingLine {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BookingText { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime BookingTime { get; set; }

        [Required]
        public virtual Account Account {get; set;}

        public virtual Category Category { get; set; }

        public virtual AccountStatement AccountStatement { get; set; }

        public virtual BookingLine RelatedBookingLine { get; set;}
    }
}
