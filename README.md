# Proyecto-Compiladores

#José Alejandro Montenegro Monzón - 1229918
#Derly Alejandro Rodas Perez - 1177318

#Funcionamiento

1. La consola le pedirá ingresar el archivo con los comandos correspondientes al lenguaje java.
2. A continuación leerá linea por línea del archivo.
3. Durante la lectura del archivo, el programa leerá caracter por caracter de cada una de las lineas del código, de esa forma junto a las expresiones regulares establecidas, se validará que cada uno de los lexemas que se está introduciendo sea valido y se pueda identificar. Una vez identifica el lexema, éste escribirá en un archivo de texto ubicado en la carpeta debug de este programa una linea que indica que tipo de lexema es y en que linea del archivo de lectura se encuentra.
4. En el caso de que un caracter no cumpla con una expresión regular, se identificará el lexema anterior y se escribira. Ahora la lectura continuará desde el último caracter que se introdujo. En el caso de que el caracter no sea válido el programa escribirá en el archivo que se encuentra en la carpeta Debug, una linea indicando que el caracter no es válido.
5. El programa es robusto porque logra identificar errores como cadenas comentadas sin terminar, cadenas strings sin cerrar, doubles válidos he invalidos, etc. En resumen, el programa no caerá cuando se introduzca el archivo de lectura.


# Fase 2

Analizador Sintáctico.

--Funcionamiento

	Tras haber analizado el archivo que se ingreso de forma léxica, se procede a analizar los tokens de forma Sintáctica. Para realizarlo se utilizó el analizador SLR debido a que creaba menos estados a que si se utilizaba el LR(1). Por fortuna se logró quitar toda la ambigüedad de la gramática y se removieron tokens innecesarios y se el resultado fue que se obtuvo una gramática sin conflictos. Por lo que las únicas acciones que realizará nuestro analizador sintáctico serán desplazamiento en caso la acción a realizar empieza con "s", una reducción en caso la accion a realizar empieza con "r", un salto en caso se realizó una reducción y toca analizar un simbolo No-Terminal de la Producción, Descubrir un error en caso un token haga falta o venga un token incorrecto y por último aceptar la cadena.

	El método que se realizó fue el mismo al de las hojas de trabajo, se trabajó con una tabla de análisis SLR y una "tabla" de parseo, donde dicha tabla se tiene una Pila la cual llevará los registros de los estados que ya pasamos y estamos por ejecutar, una Pila de Simbolos, el cual llevará control de los tokens que ya se analizaron y los No-Terminales a los cuales se les hizo una reducción, una Cola de Entradas la cual posee los tokens a analizár y una variable la cual almacenará la acción a realizar, ya sea un desplazamiento, reducción, etc.

--Manejo de errores

	En este caso se la forma en la que se realizó el manejo de errores fue el omitir la cadena actual y continuar el análisis con la siguiente linea.
	Para este tipo de manejo de errores existen tres casos:

	1) El primer caso es si el primer token de la linea actual es incorrecto o hace falta, se le indica al usuario el token que no es correcto y de que linea procede. Se omite toda la linea actual y continua con la siguiente. Además para poder verificar que toda la estructura del archivo viene de forma correcta, se restablece todos los datos de la Pila y de los Símbolos para volver a iniciar de 0.

	2) El segundo caso es si en alguna parte de la cadena viene un token incorrecto o hace falta, de la misma forma que el anterior, se omite cada token que proviene de la misma linea y se prosigue con el análisis de la linea siguiente.

	3) El ultimo caso sería de si viene un token incorrecto o de caso omiso al final, se procede a limpiar tanto la Pila como los Simbolos indicando que todo lo anterior no se le podrá realizar las reducciones correctas e incorrectas y continuar con el análisis en caso hacen falta tokens por analizar.


# Fase 3
	En esta fase se le agregó al compilador el análisis Semántico. Para dicho análisis se le agregaron atributos tanto heredados como sintetizados a la gramática. En esta fase se analisa el sentido de la gramática.

Atributos a Utilizar	Nombre				Función
Heredado		.TipoAS				Almacena el tipo del identificador, ya sea un tipo entero, booleano, cadena, etc.
Sintetizado		.Valor				Almacena el valor de las operaciones que se realizan dentro de las expresiones
Sintetizado		.SubNivel			Almacena el valor del Ámbito en el que se encuentra el token

 	En este caso los Errores se manejaron de la siguiente manera. Si se realiza una operación o una asignación que no sean del mismo tipo, entonces se le notificará al usuario que en dicha linea los tokens son incorrectos y se omite dicha linea, por lo que si es una asignación no se le ingresará el resultado al token correspondiente. En caso contrario si la expresión se encuentra correcta, el resutlado se almacenaría en dicho identificador.