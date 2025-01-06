using System;
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
    public class IndexModel : PageModel
    {
        private readonly MP133.Data.MP133Context _context;

        public IndexModel(MP133.Data.MP133Context context)
        {
            _context = context;
        }

        public IList<Mitarbeiter> Mitarbeiter { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Mitarbeiter != null)
            {
                Mitarbeiter = await _context.Mitarbeiter.ToListAsync();
            }
        }
    }
}
