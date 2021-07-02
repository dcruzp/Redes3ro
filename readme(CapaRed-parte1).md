## Proyecto de Redes (Capa de Red) *parte 1*

### Autor 
  - Daniel de la Cruz Prieto  C311

### Que fue lo nuevo que se implemento 

En esta parte del proyecto se implemento el an�lisis de la informaci�n 
que llega desde una computadora en especifico. Una vez que una computadora 
detecto que un frame le fue enviado-capturado. Esta Primero chequea que los 
datos que le llegaron estaban bien. Es decir verifica que los datos sean 
correctos. 

Una vez que los datos que llegaron en el frame hayan sido chequeados entonces
esta decide verificar si las data que se revive es una query y es analizada 
si esta query y en dependencia de si es un AQRQ o un ARPR. 

Si la data es una query entonces esta construye una frame de respuesta ente
la pregunta y este es enviado a esta computadora que le hizo la pregunta (
para saber si direcci�n mac). 

Si es una respuesta entonces la computadora busca en un diccionario para saber 
que datos est�n listos para ser enviados a esa computadora (de los cuales no 
se sabia si direcci�n mac , pero si su ip). entonces elabora los frames correspondientes 
y los encola para que salgan por el puerto de la computadora hacia su destino. 

#### Configuraci�n 

En el fichero *config.txt* se pueden ajustar varios par�metros para corres el programa

1. **signal_time** : este es el tiempo que un bit tiene que estar transmiti�ndose para que sea recibido 

2. **numero_puertos_hub**: Este par�metro son 2 enteros de la forma ***a-b*** separados por el caracter **'-'**  el primer entero ***a*** corresponde con la cantidad m�nima de puertos que puede tener un *hub* . Y el segundo ***b*** es la cantidad maxima de puertos que puede tener un hub .

3. **max_cantidad_milisegundos**: Este par�metro es la cantidad m�xima de mili-segundos que correr� el programa , despu�s de este tiempo el programa finaliza sin ejecutar ninguna otra instrucci�n. 


### Aspectos a tener en cuenta 

 EL proyecto esta hecho en C# , por lo tanto si se corre en linux 
se puede usar net.Core 3.1 que fue el que utilice para hacer el 
proyecto 

  No se usan bibliotecas especiales. Aunque no se si pueda haber problema 
porque en algunos casos tuve que usar System.Linq que debe venir pro 
default con .net Core.  

### Entrada 

el fichero de entrada *script.txt* esta en una carpeta que se llama **input**. 

### Salida 

los ficheros de salida est�n en una carpeta que se llama **output**. 
Al principio de la ejecuci�n del programa este directorio se borra 
completamente, y despu�s son generados los ficheros a medida que se 
va ejecutando el programa

### Cosas que se cambiaron 

En los proyectos anteriores se escrib�a en los ficheros de salida 
cada vez que una computadora recib�a un bit , esto generaba un costo 
adicional a la hora de ejecuci�n. En este proyecto eso se cambios.
Ahora solo se escribe en la salida solo al final de la ejecuci�n del 
programa. 

Los frames si se escriben en la salida en cuanto son recibidos por la 
computadora, al igual que los ip_packet, ya que estos son recibidos 
mas espaciados en el tiempo. Esto hab�a que hacerlo porque el programa 
se pon�a muy lento. 

### Correcci�n de errores 

No hay mas de un algoritmo de detecci�n de errores. Por eso es que no 
se tiene en cuenta en las configuraciones. El algoritmo que se usa es 
un Hash. Se suman todos los bytes y despu�s cuando estos son recibidos 
entonces se comprueba que los datos fueron correctos mediante este 
mecanismo