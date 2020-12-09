using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nummus.Data {
    public partial class Account {
        private Account() {
            AccountStatements = new HashSet<AccountStatement>();
            BookingLines = new HashSet<BookingLine>();
        }

        public Account(string name, NummusUser nummusUser) : this() {
            Name = name;
            NummusUser = nummusUser;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public virtual NummusUser NummusUser { get; set; }

        public virtual ICollection<AccountStatement> AccountStatements { get; }
        
        public virtual ICollection<BookingLine> BookingLines { get; }
    }
}