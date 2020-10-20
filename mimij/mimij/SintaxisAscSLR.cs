using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Threading;

namespace mimij
{
    class SintaxisAscSLR
    {
        public static Queue<Token> Tokens = new Queue<Token>();
        public static Dictionary<int, Dictionary<string, string>> Tabla = new Dictionary<int, Dictionary<string, string>>();
        public static Dictionary<int, Dictionary<string, int>> producciones = new Dictionary<int,Dictionary<string, int>>();
        static int reducction = 0;
        static bool Error = false;
        static _Application excel = new _Excel.Application();
        static string newPath = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf("\\bin"));
        static Workbook wb = excel.Workbooks.Open(Path.Combine(newPath, "AnalisiSintactico", "Tabla.xlsx"));
        static Worksheet ws = wb.Worksheets[1];
        static Range range;
        static Thread t;
        static string getValueCell(int x, int y)
        {
            var celda = (range.Cells[y, x] as Range).Value2;
            return celda == null ? "" : Convert.ToString(celda);
        }
        static void loadTabla()
        {
            range = ws.UsedRange;
            var rw = range.Rows.Count;
            var cl = range.Columns.Count;
            var columnBase = new List<string>();
            for (int i = 2; i <= cl; i++)
            {
                columnBase.Add(getValueCell(i, 1));
            }
            for (int i = 2; i <= rw; i++)
            {
                var dicFila = new Dictionary<string, string>();
                for (int j = 2; j < cl; j++)
                {
                    dicFila.Add(columnBase[j - 2], getValueCell(j, i));
                }
                Tabla.Add(i - 2, dicFila);
            }
            wb.Close(true, null, null);
            excel.Quit();
        }
        static void loadGramatica()
        {
            using (var file = new StreamReader(Path.Combine(newPath, "AnalisiSintactico", "Gramatica.txt")))
            {
                var cont = 0;
                var line = string.Empty;
                while ((line = file.ReadLine()) != null)
                {
                    var produc = line.Split(new[] { " -> " }, StringSplitOptions.None);
                    var cantProduc = produc[1].Split(new char[] { ' ' , '\t'});
                    producciones.Add(cont, new Dictionary<string, int> { { produc[0], cantProduc.Length } });
                    cont++;
                }
                file.Close();
            }
        }
        static public void load()
        {
            t = new Thread(new ThreadStart(loadTabla));
            t.Start();
            loadGramatica();
        }
        public static void Parse()
        {
            t.Join();
            //pila
            var Pila = new Stack<int>();
            Pila.Push(0);
            //simbolo
            var Simbolo = new Stack<Token>();
            //Entrada
            var tokenAux = new Token("$", 0, 0, 0);
            Tokens.Enqueue(tokenAux);
            var pos = 0;
            while (Tokens.Count() != 0)
            {
                pos = Action(ref Pila, ref Simbolo, ref Tokens, Tabla[pos]);
                if (pos == 1 && Tabla[pos][Tokens.First().name] == "Accept")
                {
                    Console.WriteLine("Cadena aceptada");
                    break;
                }
                if (pos == 0 && Error)
                {
                    var lineaActual = Tokens.First().line;
                    while (lineaActual == Tokens.First().line)
                    {
                        Tokens.Dequeue();
                    }
                    if (Tokens.First().name == "$")
                    {
                        Console.WriteLine("Error: La cadena no es aceptada");
                        break;
                    }
                    Error = false;
                }
            }
            Console.ReadLine();
        }
        private static int Action(ref Stack<int> Pila, ref Stack<Token> Simbolo, ref Queue<Token> Tokens, Dictionary<string, string> Estado)
        {
            int pos = 0;
            if (reducction == 0)
            {
                var valor = (Tokens.First().tipo == 6 || Tokens.First().tipo == 1 || Tokens.First().tipo == 2 || Tokens.First().tipo == 7 || Tokens.First().tipo == 4) ? esUnTerminalDiferente() : Tokens.First().name;
                if (Estado.ContainsKey(valor))
                {
                    var acciones = Estado[Tokens.First().name];
                    var conflictos = acciones.Split(',');
                    if (conflictos.Length == 1)
                    {
                        var accAux = conflictos[0];
                        if (accAux[0] == 'd' && reducction != 1)
                        {
                            valor = conflictos[0].Substring(1);
                            Simbolo.Push(Tokens.First());
                            Pila.Push(Convert.ToInt32(valor));
                            pos = Convert.ToInt32(valor);
                            Tokens.Dequeue();
                            return pos;
                        }
                        else
                        {
                            if (accAux[0] == 'r' && reducction != 1)
                            {
                                valor = conflictos[0].Substring(1);
                                var producction = producciones[Convert.ToInt32(valor)];
                                var simbol = producction.Keys;
                                var cantidadRemover = producction[simbol.First()];
                                for (int i = 0; i < cantidadRemover; i++)
                                {
                                    Pila.Pop();
                                    Simbolo.Pop();
                                }
                                var tokenAux = new Token(simbol.First(), 0, 0, 0);
                                Simbolo.Push(tokenAux);
                                reducction = 1;
                                pos = Pila.First();
                                return pos;
                            }
                            else
                            {
                                if (accAux == "Accept")
                                {
                                    return 1;
                                }
                            }
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    Console.WriteLine("El token {0} cituado en la linea {1} es incorrecto", Tokens.First().name, Tokens.First().line);
                    Error = true;
                    return 0;
                }
            }
            else
            {
                if (Estado.ContainsKey(Simbolo.First().name))
                {
                    var acciones = Estado[Simbolo.First().name];
                    var valor = Convert.ToInt32(acciones);
                    Pila.Push(valor);
                    reducction = 0;
                    return valor;
                }
                else
                {
                    Error = true;
                    return 0;
                }
            }
            return pos;
        }
        private static string esUnTerminalDiferente()
        {
            switch(Tokens.First().tipo)
            {
                case 1:
                    return "booleanConstant";
                case 2:
                    return "intConstant";
                case 4:
                    return "doubleConstant";
                case 7:
                    return "stringConstant";
                case 6:
                    return "ident";
                default: return string.Empty;
            }
        }
    }
}
