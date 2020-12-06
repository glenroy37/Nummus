using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nummus.Data {
    public partial class AccountStatement {
        public AccountStatement() {
            this.BookingLines = new HashSet<BookingLine>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,2)")]
        public decimal ClosingSum { get; set; }

        [Required]
        public virtual Account Account { get; set; }

        public virtual AccountStatement lastStatement { get; set; }

        public virtual ICollection<BookingLine> BookingLines { get; set; }
        
    }
}