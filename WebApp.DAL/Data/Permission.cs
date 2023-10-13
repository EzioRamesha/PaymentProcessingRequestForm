﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public class Permission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }

        public Permission()
        {
            IsEnabled = true;
        }

        public Permission(string name) : this()
        {
            Name = name;
        }

        public Permission(string name, string description)
            : this(name)
        {
            Description = description;
        }
    }
}
