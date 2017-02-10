﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftN_Trainings.Models.BO
{
    [Table("Trainings")]
    public class Training
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        //naviation properties
        public virtual ICollection<Session> Sessions { get; set; }
    }
}