# Proyecto de Redes  (Deteccion de Errores )

#### Integrante : 
 - Daniel de la Cruz Prieto C311

 Nota: 

 El proyecto lo hice en ***C#*** (como lenguaje de programaci�n) en ***Visual Studio 2019*** 
 de modo que para correrlo en Linux se puede usar mono ***mono***. 


#### Como funciona el programa

El Programa tiene varios par�metros fundamentales que esta en la clase *Program* como son el 
*signal_time* y el tiempo m�ximo que va a correr el programa (para el programa la interpretaci�n del tiempo es cada vez que pasa un 
ciclo del programa *ciclo principal del programa* es como si pasara 1 mili-segundo) Entonces los Host est�n representados por la 
clase Computadora , que esta en un fichero aparte de la soluci�n , los Hubs por la clase Hub y los Switch por la clase Switch.cs. 

Entonces basicamente hay dos momentos para el manejo de la inormacion que se 
transmite de dsipositivo a dispositivo. El primero se determina que bit es 
el que va a la salida de cada puerto. Esto es que cada dispositivo determian que 
bit se va a transmitir por cada puerto. 

Y el segundo momento se recogen los bit que cada dispositivo recibe y cada uno de 
los dispositivos de la red determian que tratamiento le da a esos dispositivos. 


#### Configuraci�n 

En el fichero *config.txt* se pueden ajustar varios par�metros para corres el programa

1. **signal_time** : este es el tiempo que un bit tiene que estar transmiti�ndose para que sea recibido 

2. **numero_puertos_hub**: Este par�metro son 2 enteros de la forma ***a-b*** separados por el caracter **'-'**  el primer entero ***a*** corresponde con la cantidad m�nima de puertos que puede tener un *hub* . Y el segundo ***b*** es la cantidad maxima de puertos que puede tener un hub .

3. **max_cantidad_milisegundos**: Este par�metro es la cantidad m�xima de mili-segundos que correr� el programa , despu�s de este tiempo el programa finaliza sin ejecutar ninguna otra instrucci�n. 

#### Como se corrigen los errores 

El algoritmo basico es coger todos los bytes de la 
data y sumarlos, y esa informacion enviarla en el frame
y asi verificar despues que en el host de salida 
esa suma coincide con los datos de la informacion que 
se recibe en los hots


