using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA3
{ 
        public class Partido
{
    public string Nombre { get; set; }
    public string Acronimo { get; set; }
    public string Presidente { get; set; }
    public int Escanos { get; set; }
    public int VotosValidos { get; set; } // Cambiado a int

    public Partido(string nombre, string acronimo, string presidente, int posicion, int population, int abstentionVotes)
    {
        Nombre = nombre;
        Acronimo = acronimo;
        Presidente = presidente;

        int nullVotes = (population - abstentionVotes) / 20;
        int totalValidVotes = population - abstentionVotes - nullVotes; // Cambiado a int

        // Ajustar los porcentajes en función de la posición del partido en la lista
        Dictionary<string, double> porcentajes = new Dictionary<string, double>
        {
            { "Partido1", 35.25 },
            { "Partido2", 24.75 },
            { "Partido3", 15.75 },
            { "Partido4", 14.25 },
            { "Partido5", 3.75 },
            { "Partido6", 3.25 },
            { "Partido7", 1.5 },
            { "Partido8", 0.5 },
            { "Partido9", 0.25 },
            { "Partido10", 0.25 },
            { "Votos en Blanco", 0.5 }
        };

        // Asegurarse de que la posición esté dentro de los límites del diccionario
        int posicionPartido = Math.Max(1, Math.Min(posicion, porcentajes.Count));
        string key = "Partido" + posicionPartido;

        if (porcentajes.ContainsKey(key))
        {
            double porcentaje = porcentajes[key] / 100;
            VotosValidos = (int)(totalValidVotes * porcentaje); // Cambiado a int
        }
        else
        {
            VotosValidos = 0; // Establecer votos válidos en 0 si no se encuentra la posición en el diccionario
        }
    }
}
}