using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MP133.Pages.Users
{
    public class FilterModel : PageModel
    {
        private readonly string _connectionString= ("AzureConnection");
        public FilterModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureConnection");
        }

        public List<Fahrzeug> Fahrzeuge { get; set; } = new List<Fahrzeug>();

        [BindProperty(SupportsGet = true)]
        public string FilterMarke { get; set; }
        [BindProperty(SupportsGet = true)]
        public string FilterModell { get; set; }
        [BindProperty(SupportsGet = true)]
        public string FilterFahrzeugtyp { get; set; }
        [BindProperty(SupportsGet = true)]
        public decimal? FilterMinPreis { get; set; }
        [BindProperty(SupportsGet = true)]
        public decimal? FilterMaxPreis { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? FilterBaujahr { get; set; }

        public async Task OnGetAsync()
        {
            var query = @"
    SELECT * FROM Fahrzeuge
    WHERE (@Marke IS NULL OR Marke LIKE '%' + @Marke + '%')
      AND (@Modell IS NULL OR Modell = @Modell)
      AND (@Fahrzeugtyp IS NULL OR Fahrzeugtyp = @Fahrzeugtyp)
      AND (@MinPreis IS NULL OR Preis >= @MinPreis)
      AND ( @MaxPreis IS NULL OR Preis <= @MaxPreis)
      AND (@Baujahr IS NULL OR Baujahr >= @Baujahr)
    "; 

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Marke", (object)FilterMarke ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Modell", (object)FilterModell ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Fahrzeugtyp", (object)FilterFahrzeugtyp ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MinPreis", (object)FilterMinPreis ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MaxPreis", (object)FilterMaxPreis ?? DBNull.Value);

                    if (FilterBaujahr.HasValue)
                    {
                        command.Parameters.AddWithValue("@Baujahr", FilterBaujahr.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Baujahr", DBNull.Value);  
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Fahrzeuge.Add(new Fahrzeug
                            {
                                FahrzeugId = reader.GetInt32(0),
                                Marke = reader.GetString(1),
                                Modell = reader.GetString(2),
                                Fahrzeugtyp = reader.GetString(3),
                                Baujahr = reader.GetInt32(4),
                                Preis = reader.GetDecimal(5),
                                Verfügbarkeit = reader.GetString(6),
                                BildUrl = reader.GetString(7)
                            });
                        }
                    }
                }
            }
        }

        public class Fahrzeug
        {
            public int FahrzeugId { get; set; }
            public string Marke { get; set; }
            public string Modell { get; set; }
            public string Fahrzeugtyp { get; set; }
            public int Baujahr { get; set; }
            public decimal Preis { get; set; }
            public string Verfügbarkeit { get; set; }
            public string BildUrl { get; set; }
        }
    }
}