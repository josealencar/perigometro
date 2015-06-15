using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perigometro.Dominio
{
    public class Acidente
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Tipo_Acid { get; set; }
        public int Fatais { get; set; }
        public int Auto { get; set; }
        public int Taxi { get; set; }
        public int Lotacao { get; set; }
        public int Onibus_Urb { get; set; }
        public int Onibus_Met { get; set; }
        public int Onibus_Int { get; set; }
        public int Caminhao { get; set; }
        public int Moto { get; set; }
        public int Carroca { get; set; }
        public int Bicicleta { get; set; }
        public int Outro { get; set; }
        public string Tempo { get; set; }
        public string Noite_dia { get; set; }
        public string Regiao { get; set; }
        public string Dia_Sem { get; set; }
        public int Ano { get; set; }
    }
}
