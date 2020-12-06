using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nummus.Data {
    public partial class NummusUser {
        public NummusUser(string userEmail) {
            this.UserEmail = userEmail;
            this.Accounts = new HashSet<Account>();
        }

        public int Id { get; set; }

        [Required]
        public string UserEmail { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}