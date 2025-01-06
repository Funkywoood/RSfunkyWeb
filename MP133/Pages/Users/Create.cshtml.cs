using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity; // PasswordHasher
using Microsoft.EntityFrameworkCore;
using MP133.Data;
using MP133.Model;

namespace MP133.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly MP133.Data.MP133Context _context;
        private readonly IPasswordHasher<Mitarbeiter> _passwordHasher;

        public CreateModel(MP133.Data.MP133Context context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Mitarbeiter>();
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Mitarbeiter Mitarbeiter { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Überprüfen, ob das Passwort mit der Bestätigung übereinstimmt
            if (Mitarbeiter.Passwort != Mitarbeiter.PasswortBestätigen)
            {
                ModelState.AddModelError("Mitarbeiter.PasswortBestätigen", "Das Passwort und die Bestätigung stimmen nicht überein.");
                return Page();
            }

            var neuerMitarbeiter = new Mitarbeiter
            {
                Vorname = Mitarbeiter.Vorname,
                Name = Mitarbeiter.Name,
                Strasse = Mitarbeiter.Strasse,
                Nr = Mitarbeiter.Nr,
                PLZ = Mitarbeiter.PLZ,
                Ort = Mitarbeiter.Ort,
                Email = Mitarbeiter.Email,
                Telefon = Mitarbeiter.Telefon,
                Passwort = string.Empty, // Initialisieren, wird gleich gehasht
                PasswortBestätigen = string.Empty
            };

            // Passwort hashen
            neuerMitarbeiter.Passwort = _passwordHasher.HashPassword(neuerMitarbeiter, Mitarbeiter.Passwort);

            _context.Mitarbeiter.Add(neuerMitarbeiter);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
