﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Nummus.Data {
    public class Category {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public CategoryType CategoryType { get; set; }
    }

    public enum CategoryType {
        INCOME,
        SPENDING,
        CARRY
    }
}