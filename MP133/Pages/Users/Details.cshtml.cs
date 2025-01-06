﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MP133.Data;
using MP133.Model;

namespace MP133.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly MP133.Data.MP133Context _context;

        public DetailsModel(MP133.Data.MP133Context context)
        {
            _context = context;
        }

        public Mitarbeiter Mitarbeiter { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Mitarbeiter == null)
            {
                return NotFound();
            }

            var mitarbeiter = await _context.Mitarbeiter.FirstOrDefaultAsync(m => m.Id == id);
            if (mitarbeiter == null)
            {
                return NotFound();
            }
            else
            {
                Mitarbeiter = mitarbeiter;
            }
            return Page();
        }
    }
}
