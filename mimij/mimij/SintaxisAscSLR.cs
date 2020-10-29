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
        static bool error = false;
        static bool aceptar = false;
        static int lActual = 0;
        static int cantError = 0;
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
                if (pos == 1 && aceptar)
                {
                    break;
                }
                if (pos == 0 && error)
                {
                    while (lActual == Tokens.First().line)
                    {
                        Tokens.Dequeue();
                    }
                    if (Tokens.First().name == "$" && error)
                    {
                        break;
                    }
                    //resetear la pila
                    Pila = new Stack<int>();
                    Simbolo = new Stack<Token>();
                }
                error = false;
            }
            if (cantError > 0)
            {
                Console.WriteLine("Error: La cadena no es aceptada");
            }
            else
            {
                Console.WriteLine("Cadena aceptada");
            }
        }
        private static int Action(ref Stack<int> Pila, ref Stack<Token> Simbolo, ref Queue<Token> Tokens, Dictionary<string, string> Estado)
        {
            var actions = string.Empty;
            if (reducction == 0)
            {
                var valor = (Estado.ContainsKey(Tokens.First().name)) ? Tokens.First().name : esUnTerminalDiferente();
                actions = Estado[valor];
                if (actions != string.Empty)
                {
                    int estadoA;
                    lActual = Tokens.First().line;
                    if (actions[0] == 's')
                    {
                        estadoA = Convert.ToInt32(actions.Substring(1));
                        Pila.Push(estadoA);
                        Simbolo.Push(Tokens.First());
                        Tokens.Dequeue();
                        return estadoA;
                    }
                    else if (actions[0] == 'r')
                    {
                        estadoA = Convert.ToInt32(actions.Substring(1));
                        var producction = producciones[estadoA];
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
                        return Pila.First();
                    }
                    else if (actions == "acc")
                    {
                        aceptar = true;
                        return 1;
                    }
                }
                else
                {
                    Console.WriteLine("El token {0} localizado en la linea {1} es incorrecto", Tokens.First().name, lActual);
                    error = true;
                    cantError++;
                    return 0;
                }
            }
            else
            {
                actions = Estado[Simbolo.First().name];
                if (actions != string.Empty)
                {
                    var value = Convert.ToInt32(actions);
                    Pila.Push(value);
                    reducction = 0;
                    return value;
                }
                else
                {
                    Console.WriteLine($"La linea {lActual} no posee la estructura correcta");
                    error = true;
                    return 0;
                }
            }
            return 0;
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
                default: return "e";
            }
        }
    }
}
