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
clase Computadora , que esta en un fichero aparte de la solución , los Hubs por la clase Hub.

Entonces en el ciclo principal , se recorren primero todos los dispositivos que son computadoras que hay en el momento actual creados , 
para actualiza el bit que ellos van a emitir a otras computadoras 

Después se hace otro ciclo para verificar cual es el bit que entra en cada computadora y determinar si 
hay una colisión.

Si hay una colisión, el Host va a esperar de 5 a 50 mili-segundos (una cantidad random de mili-segundos entre 5 y 50) 
para volver a enviar información. Si no se pudo enviar un bit completo (es decir que el bit no se estuvo transmitiendo por el canal la cantidad de mili-segundos 
que se especifican en el Signal_time) entonces el bit no se da como enviado y cuando la computadora vuelva a tratar de enviar información va a empezar por ese bit 
tratando de mantenerlo transmitiéndose el tiempo que indica el signal_time.   


#### Configuración 

En el fichero *config.txt* se pueden ajustar varios parámetros para corres el programa

1. **signal_time** : este es el tiempo que un bit tiene que estar transmitiéndose para que sea recibido 

2. **numero_puertos_hub**: Este parámetro son 2 enteros de la forma ***a-b*** separados por el caracter **'-'**  el primer entero ***a*** corresponde con la cantidad mínima de puertos que puede tener un *hub* . Y el segundo ***b*** es la cantidad maxima de puertos que puede tener un hub .

3. **max_cantidad_milisegundos**: Este parámetro es la cantidad máxima de mili-segundos que correrá el programa , después de este tiempo el programa finaliza sin ejecutar ninguna otra instrucción. 