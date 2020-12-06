using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nummus.Data {
    public partial class Account {
        public Account() {
            this.AccountStatements = new HashSet<AccountStatement>();
        }

        public Account(string name) : this() {
            this.Name = name;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public virtual NummusUser User { get; set; }

        public virtual ICollection<AccountStatement> AccountStatements { get; set; }
    }
}