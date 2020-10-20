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

        static Dictionary<string, int> productionAsignation(int estado)
        {
            var diccionario = new Dictionary<string, int>();
            switch (estado)
            {
                case 1:
                    diccionario.Add("S", 1);
                    break;
                case 2:
                    diccionario.Add("S", 3);
                    break;
                case 3:
                    diccionario.Add("V", 1);
                    break;
                case 4:
                    diccionario.Add("E", 1);
                    break;
                case 5:
                    diccionario.Add("E", 1);
                    break;
            }
            return diccionario;
        }
        static Dictionary<string, string> valueAsignation(int estado)
        {
            var diccionario = new Dictionary<string, string>();
            switch (estado)
            {
                case 0:
                    diccionario.Add("id", "d2");
                    diccionario.Add("S", "1");
                    diccionario.Add("V", "3");
                    break;
                case 1:
                    diccionario.Add("$", "Accept");
                    break;
                case 2:
                    diccionario.Add(":=", "r3");
                    diccionario.Add("$", "r1,r3");
                    break;
                case 3:
                    diccionario.Add(":=", "d4");
                    break;
                case 4:
                    diccionario.Add("id", "d8");
                    diccionario.Add("num", "d7");
                    diccionario.Add("V", "6");
                    diccionario.Add("E", "5");
                    break;
                case 5:
                    diccionario.Add("$", "r2");
                    break;
                case 6:
                    diccionario.Add("$", "r4");
                    break;
                case 7:
                    diccionario.Add("$", "r5");
                    break;
                case 8:
                    diccionario.Add("$", "r3");
                    diccionario.Add(":=", "r3");
                    break;
            }
            return diccionario;
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
                    Console.ReadLine();
                    break;
                }
            }
        }
        private static int Action(ref Stack<int> Pila, ref Stack<Token> Simbolo, ref Queue<Token> Tokens, Dictionary<string, string> Estado)
        {
            int pos = 0;
            if (reducction == 0)
            {
                if (Estado.ContainsKey(Tokens.First().name))
                {
                    var acciones = Estado[Tokens.First().name];
                    var conflictos = acciones.Split(',');
                    if (conflictos.Length == 1)
                    {
                        var valor = string.Empty;
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

                }
            }
            return pos;
        }
    }
}
