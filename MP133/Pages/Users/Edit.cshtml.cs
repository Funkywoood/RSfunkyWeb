using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MP133.Data;
using MP133.Model;
using Microsoft.AspNetCore.Identity; // PasswordHasher
using Microsoft.Data.SqlClient;

namespace MP133.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly MP133.Data.MP133Context _context;
        private readonly IPasswordHasher<Mitarbeiter> _passwordHasher;

        public EditModel(MP133.Data.MP133Context context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Mitarbeiter>(); // PasswordHasher instanziieren
        }

        [BindProperty]
        public Mitarbeiter Mitarbeiter { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Mitarbeiter = await _context.Mitarbeiter.FindAsync(id);

            if (Mitarbeiter == null)
            {
                return NotFound();
            }

            Mitarbeiter.Passwort = string.Empty;
            Mitarbeiter.PasswortBestätigen = string.Empty;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Überprüfe, ob die ID korrekt ist
            if (Mitarbeiter.Id == 0)
            {
                ModelState.AddModelError(string.Empty, "Die ID ist ungültig oder fehlt.");
                return Page();
            }

            var mitarbeiterToUpdate = await _context.Mitarbeiter.FindAsync(Mitarbeiter.Id);
            if (mitarbeiterToUpdate == null)
            {
                return NotFound();
            }

            // Überprüfen, ob Passwort und PasswortBestätigen übereinstimmen
            if (!string.IsNullOrEmpty(Mitarbeiter.Passwort) || !string.IsNullOrEmpty(Mitarbeiter.PasswortBestätigen))
            {
                if (Mitarbeiter.Passwort != Mitarbeiter.PasswortBestätigen)
                {
                    ModelState.AddModelError("Mitarbeiter.PasswortBestätigen", "Das Passwort und die Bestätigung stimmen nicht überein.");
                    return Page();
                }

                // Passwort hashen und speichern, wenn es geändert wurde
                mitarbeiterToUpdate.Passwort = _passwordHasher.HashPassword(mitarbeiterToUpdate, Mitarbeiter.Passwort);
                mitarbeiterToUpdate.PasswortBestätigen = string.Empty; // Bestätigung muss nicht gespeichert werden
            }

            mitarbeiterToUpdate.Vorname = Mitarbeiter.Vorname;
            mitarbeiterToUpdate.Name = Mitarbeiter.Name;
            mitarbeiterToUpdate.Strasse = Mitarbeiter.Strasse;
            mitarbeiterToUpdate.Nr = Mitarbeiter.Nr;
            mitarbeiterToUpdate.PLZ = Mitarbeiter.PLZ;
            mitarbeiterToUpdate.Ort = Mitarbeiter.Ort;
            mitarbeiterToUpdate.Email = Mitarbeiter.Email;
            mitarbeiterToUpdate.Telefon = Mitarbeiter.Telefon;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MitarbeiterExists(Mitarbeiter.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool MitarbeiterExists(int id)
        {
            return _context.Mitarbeiter.Any(e => e.Id == id);
        }
    }
}
