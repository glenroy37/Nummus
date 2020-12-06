using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nummus.Data {
    public partial class Account {
        public Account() {
            this.AccountStatements = new HashSet<AccountStatement>();
        }

        public Account(string name, NummusUser nummusUser) : this() {
            this.Name = name;
            this.NummusUser = NummusUser;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public virtual NummusUser NummusUser { get; set; }

        public virtual ICollection<AccountStatement> AccountStatements { get; set; }
    }
}