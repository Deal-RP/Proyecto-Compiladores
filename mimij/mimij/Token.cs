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

        public Token(string Name, int Line, int Column, int Tipo)
        {
            name = Name;
            line = Line;
            columnFirst = Column;
            columnEnd = columnFirst + Name.Length;
            tipo = Tipo;
        }
    }
}
