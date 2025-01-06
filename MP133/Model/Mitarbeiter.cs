using System.ComponentModel.DataAnnotations;

namespace MP133.Model
{
    public class Mitarbeiter
    {
        public int Id { get; set; }
        public string Vorname { get; set; }
        public string Name { get; set; }

        public string Strasse { get; set; }

        public string Nr { get; set; }

        public int PLZ { get; set; }
        public string Ort { get; set; }

        public string? Land { get; set; }

        public string Email { get; set; }

        public string Telefon { get; set; }

        public string? Funktion { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\W)(?=.*\d).{8,}$", ErrorMessage = "Das Passwort muss mindestens 8 Zeichen lang sein, einen Großbuchstaben, einen Kleinbuchstaben, eine Zahl und ein Sonderzeichen enthalten.")]
        public string  Passwort{ get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen")]
        [Compare("Passwort", ErrorMessage = "Das Passwort und die Passwortbestätigung stimmen nicht überein.")]
        public string PasswortBestätigen { get; set; }

    }
}
