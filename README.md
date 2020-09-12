## Proyecto-Compiladores

#José Alejandro Montenegro Monzón - 1229918
#Derly Alejandro Rodas Perez - 1177318

#Funcionamiento

1. La consola le pedirá ingresar el archivo con los comandos correspondientes al lenguaje java.
2. A continuación leerá linea por línea del archivo.
3. Durante la lectura del archivo, el programa leerá caracter por caracter de cada una de las lineas del código, de esa forma junto a las expresiones regulares establecidas, se validará que cada uno de los lexemas que se está introduciendo sea valido y se pueda identificar. Una vez identifica el lexema, éste escribirá en un archivo de texto ubicado en la carpeta debug de este programa una linea que indica que tipo de lexema es y en que linea del archivo de lectura se encuentra.
4. En el caso de que un caracter no cumpla con una expresión regular, se identificará el lexema anterior y se escribira. Ahora la lectura continuará desde el último caracter que se introdujo. En el caso de que el caracter no sea válido el programa escribirá en el archivo que se encuentra en la carpeta Debug, una linea indicando que el caracter no es válido.
5. El programa es robusto porque logra identificar errores como cadenas comentadas sin terminar, cadenas strings sin cerrar, doubles válidos he invalidos, etc. En resumen, el programa no caerá cuando se introduzca el archivo de lectura.

#LABORATORIO A
1. Tras validar los Tokens en el análisis léxico se procedio a validar el orden en el que venian los Tokens por medio del analizador sintáctico.
2. De primero se procedió a enlistar los Tokens que fueron leidos correctamente en una lista de nodos Tokens, el cual tenía los siguientes atributos:
	* Nombre = lexema del Token.
	* Numero de linea = en que linea se encuentra en el archivo de entrada.
	* Tipo = entero que representa (int, hexadecimales, double, string, palabra reservada, etc.)
3. A continuación se procedió a corregir la gramática a una gramática LL(1). Por medio de la eliminación de ambigüedad, eliminación de recursividad a la izquierda y factorización por la izquierda.
4. Se recorre la lista de Tokens con el fin de validar la estructura y orden del código.
5. Durante el transcurso del recorrido, en el caso de que un Token no este colocado en el lugar indicado se le notificará al usuario de que dicho Token se encuentra incorrecto en su linea establecida en el archivo de entrada. El programa por ende, al encontrar un error en el análisis sintáctico omitirá el resto de la linea en la que se encuentra e irá evaluar la linea próxima.
6. En el caso de no encontrar errores se desplegará un mensaje de éxito al usuario.