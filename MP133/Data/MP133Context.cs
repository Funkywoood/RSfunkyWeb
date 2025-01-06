using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MP133.Model;

namespace MP133.Data
{
    public class MP133Context : DbContext
    {
        public MP133Context(DbContextOptions<MP133Context> options)
            : base(options)
        {
        }

        public DbSet<MP133.Model.Mitarbeiter> Mitarbeiter { get; set; } = default!;
    }
}