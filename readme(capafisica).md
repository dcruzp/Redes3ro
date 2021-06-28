# Proyecto de Redes  (Capa Física)

#### Integrante : 
 - Daniel de la Cruz Prieto C311

 Nota: 

 El proyecto lo hice en ***C#*** (como lenguaje de programación) en ***Visual Studio 2019*** 
 de modo que para correrlo en Linux se puede usar mono ***mono***. 


#### Como funciona el programa

El Programa tiene varios parámetros fundamentales que esta en la clase *Program* como son el 
*signal_time* y el tiempo máximo que va a correr el programa (para el programa la interpretación del tiempo es cada vez que pasa un 
ciclo del programa *ciclo principal del programa* es como si pasara 1 mili-segundo) Entonces los Host están representados por la 
clase Computadora , que esta en un fichero aparte de la solución , los Hubs por la clase Hub y los Switch por la clase Switch.cs. 

Entonces basicamente hay dos momentos para el manejo de la inormacion que se 
transmite de dsipositivo a dispositivo. El primero se determina que bit es 
el que va a la salida de cada puerto. Esto es que cada dispositivo determian que 
bit se va a transmitir por cada puerto. 

Y el segundo momento se recogen los bit que cada dispositivo recibe y cada uno de 
los dispositivos de la red determian que tratamiento le da a esos dispositivos. 


#### Configuración 

En el fichero *config.txt* se pueden ajustar varios parámetros para corres el programa

1. **signal_time** : este es el tiempo que un bit tiene que estar transmitiéndose para que sea recibido 

2. **numero_puertos_hub**: Este parámetro son 2 enteros de la forma ***a-b*** separados por el caracter **'-'**  el primer entero ***a*** corresponde con la cantidad mínima de puertos que puede tener un *hub* . Y el segundo ***b*** es la cantidad maxima de puertos que puede tener un hub .

3. **max_cantidad_milisegundos**: Este parámetro es la cantidad máxima de mili-segundos que correrá el programa , después de este tiempo el programa finaliza sin ejecutar ninguna otra instrucción. 


