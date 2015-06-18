using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perigometro.Dominio1
{
    public class Lista 
    {
        public string Ano { get; set; }
        public Clima? Clima { get; set; }
        public Regiao? Regiao { get; set; }
        public Semana? Semana { get; set; }
        public Tipo? Tipo { get; set; }
        public Turno? Turno { get; set; }
        public Veiculo? Veiculo { get; set; }

    }
   
}
