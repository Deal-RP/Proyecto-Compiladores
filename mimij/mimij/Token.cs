using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mimij
{
    class Token
    {
        public string name { get; set; }
        public int line { get; set; }
        public int columnFirst { get; set; }
        public int columnEnd { get; set; }
        public int tipo { get; set; }
        public string valor { get; set; }
        public string tipoAS { get; set; }
        public string subnivel { get; set; }
        public bool parametro { get; set; }
        public int idNombre { get; set; }
        public Token(string Name, int Line, int Column, int Tipo, string Valor, string TipoAS, string subNivel, bool Parametro, int id)
        {
            name = Name;
            line = Line;
            columnFirst = Column;
            columnEnd = columnFirst + Name.Length;
            tipo = Tipo;
            valor = Valor;
            tipoAS = TipoAS;
            subnivel = subNivel;
            parametro = Parametro;
            idNombre = id;
        }

        public Token()
        {
        }
    }
}
