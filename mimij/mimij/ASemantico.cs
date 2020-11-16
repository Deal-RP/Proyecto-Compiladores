using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mimij
{
    class ASemantico
    {
        public static List<Token> TablaSimbolos = new List<Token>();
        public static List<Token> TokensList = new List<Token>();
        public static int i = 0;
        public static int cantError = 0;
        public static string tipo = string.Empty;
        public static Token Token;
        public static Token Token2;
        public static string ClassName = string.Empty;
        public static string path;
        public static string txtName;
        public static string EncontrarTipo(Token token)
        {
            switch (token.name)
            {
                case "int":
                    return "int";
                case "boolean":
                    return "boolean";
                case "double":
                    return "double";
                case "string":
                    return "string";
            }
            if (token.tipo == 6)
            {
                return token.name;
            }
            return string.Empty;
        }
        public static void Recorrido(string Path, string TXT)
        {
            path = Path;
            txtName = TXT;
            for (i = 0; i < TokensList.Count(); i++)
            {
                //Variable Declaration
                if (EncontrarTipo(TokensList[i]) != string.Empty)
                {
                    //validar si es una variable
                    tipo = EncontrarTipo(TokensList[i]);
                    i++;
                    //validar si contiene []
                    while (TokensList[i].tipo != 6)
                    {
                        i++;
                    }
                    //almacenar el token en una variable y verificar si ese token no existe ya
                    var encontrarIgual = TablaSimbolos.Find(X => X.name == TokensList[i].name);
                    if (encontrarIgual == null)
                    {
                        //almacenar el token en la tabla de simbolos
                        Token = TokensList[i];
                        Token.tipoFP = tipo;
                        Token.ubicación = "G";
                        TablaSimbolos.Add(Token);
                        i++;
                        //verificar si es una función o una variable
                        if (TokensList[i].name != ";")
                        {
                            //es una función
                        }
                    }
                    else
                    {
                        cantError++;
                        escribirError(1, TokensList[i]);
                    }
                }
                //FunctionDeclaration
                else if (TokensList[i].name == "void")
                {
                    //procedimientos
                    i++;
                    processDecl();
                }
                //Constdeclaration
                else if (TokensList[i].name == "static")
                {
                    i++;
                    constDecl();
                }
                //Classdeclaration
                else if (TokensList[i].name == "class")
                {

                }
                //Interfacedeclaration
                else if (TokensList[i].name == "interface")
                {
                    i++;
                    interfaceDecl();
                }                
                tipo = string.Empty;
            }
            if (cantError == 0)
            {
                Console.WriteLine("EL ARCHIVO NO POSEE NINGÚN ERROR SEMÁNTICO");
            }
            else
            {
                Console.WriteLine("EL ARCHIVO POSEE ERRORES SEMÁNTICOS");
            }
        }
        public static void processDecl()
        {
            //verificar que el token sea unico o no
            var encontrarIgual = TablaSimbolos.Find(X => X.name == TokensList[i].name&& X.tipoFP =="G");
            if (encontrarIgual == null)
            {
                Token = TokensList[i];
                Token.ubicación = "G";
                TablaSimbolos.Add(Token);
            }
            else
            {
                cantError++;
                escribirError(1, TokensList[i]);
            }
            i+=2 ;
            //validar las variables del procedimiento
            while(TokensList[i].name!= ")")
            {
                if(TokensList[i].name != ",")
                {
                    //verificar el tipo
                    tipo = EncontrarTipo(TokensList[i]);
                    //verificar la variable si existe en ese ámbito
                    encontrarIgual = TablaSimbolos.Find(X=>X.name == TokensList[i].name && X.ubicación == Token.name);
                    if(encontrarIgual == null)
                    {
                        //almacenar el token en la tabla con los datos
                        Token2 = TokensList[i];
                        Token2.ubicación = Token.name;
                        Token2.tipoFP = tipo;
                        TablaSimbolos.Add(Token2);
                    }
                    else
                    {
                        cantError++;
                        escribirError(1, TokensList[i]);
                    }
                }
                i++;
            }
            //validar los STMT
        }

        public static void constDecl()
        {
            tipo = EncontrarTipo(TokensList[i]);
            i++;
            var encontrarIgual = TablaSimbolos.Find(X => X.name == TokensList[i].name);
            if (encontrarIgual == null)
            {
                Token = TokensList[i];
                Token.tipoFP = tipo;
                Token.ubicación = "G";
                TablaSimbolos.Add(Token);
            }
            else
            {
                cantError++;
                escribirError(1, TokensList[i]);
            }
            i++;
        }
        public static void interfaceDecl()
        {
            var encontrarIgual = TablaSimbolos.Find(X => X.name == TokensList[i].name && X.tipoFP =="G");
            if (encontrarIgual == null)
            {
                Token = TokensList[i];
                Token.ubicación = "G";
                TablaSimbolos.Add(Token); 
            }
            else
            {
                cantError++;
                escribirError(1, TokensList[i]);
            }
            while(TokensList[i].name !="}")
            {
                if (EncontrarTipo(TokensList[i]) != string.Empty)
                {
                    tipo = EncontrarTipo(TokensList[i]);
                    i++;
                    encontrarIgual = TablaSimbolos.Find(X => (X.name == TokensList[i].name && X.ubicación == Token.name)||(X.ubicación == "G"&& X.name == TokensList[i].name));
                    if (encontrarIgual == null)
                    {
                        Token2 = TokensList[i];
                        Token2.ubicación = Token.name;
                        Token2.tipoFP = tipo;
                        TablaSimbolos.Add(Token2);
                    }
                    else
                    {
                        cantError++;
                        escribirError(1, TokensList[i]);
                    }
                    i += 2;
                    while (TokensList[i].name != ")")
                    {
                        if (TokensList[i].name != ",")
                        {
                            tipo = EncontrarTipo(TokensList[i]);
                            i++;
                            encontrarIgual = TablaSimbolos.Find(X => X.name == TokensList[i].name && X.ubicación == Token.name + Token2.name);
                            if (encontrarIgual == null)
                            {
                                TokensList[i].tipoFP = tipo;
                                TokensList[i].ubicación = Token.name + Token2.name;
                                TablaSimbolos.Add(TokensList[i]);
                            }
                            else
                            {
                                cantError++;
                                escribirError(1, TokensList[i]);
                            }
                        }
                        i++;
                    }
                }    
                else if(TokensList[i].name == "void")
                {
                    i++;
                    encontrarIgual = TablaSimbolos.Find(X => (X.name == TokensList[i].name && X.ubicación == Token.name) || (X.ubicación == "G" && X.name == TokensList[i].name));
                    if (encontrarIgual == null)
                    {
                        Token2 = TokensList[i];
                        Token2.ubicación = Token.name;
                        TablaSimbolos.Add(Token2);
                    }
                    else
                    {
                        cantError++;
                        escribirError(1, TokensList[i]);
                    }
                    i +=2;
                    while (TokensList[i].name != ")")
                    {
                        if(TokensList[i].name!=",")
                        {
                            tipo = EncontrarTipo(TokensList[i]);
                            i++;
                            encontrarIgual = TablaSimbolos.Find(X => X.name == TokensList[i].name && X.ubicación == Token.name+Token2.name);
                            if (encontrarIgual == null)
                            {
                                TokensList[i].tipoFP = tipo;
                                TokensList[i].ubicación = Token.name + Token2.name;
                                TablaSimbolos.Add(TokensList[i]);
                            }
                            else
                            {
                                cantError++;
                                escribirError(1, TokensList[i]);
                            }
                        }
                        i++;
                    }
                }
                i++;
            }
        }
        public static void escribirError(int numError, Token token)
        {
            switch(numError)
            {
                case 1:
                    Console.WriteLine($"TOKEN EN LINEA: {token.line} Y COLUMNA {token.columnFirst} - {token.columnEnd} : **ERROR** IDENTIFICADOR YA DECLARADO ");
                    break;
                case 2:
                     break;
            }
        }
        public static void EscrituraArchivo()
        {
            var escritura = string.Empty;


            var separacion = "----------------------------------------------------------------------------------------------------------------------------------";
            using (var writer = new StreamWriter(Path.Combine(path, $"{txtName}_Semantico.out"), append: true))
            {
                writer.WriteLine(escritura);
                writer.WriteLine(separacion);
            }
        }
    }
}
