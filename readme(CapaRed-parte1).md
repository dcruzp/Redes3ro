## Proyecto de Redes (Capa de Red) *parte 1*

## Autor 
  - Daniel de la Cruz Prieto  C311

## Que fue lo nuevo que se implemento 

En esta parte del proyecto se implemeto el analisis de la informacion 
que llega desde una computadora en especifico. Una vez que una computadora 
detecto que un frame le fue enviado-capturado. Esta Primero chequea que los 
datos que le llegaron estaban bien. Es decir verifica que los datos sean 
correctos. 

Una vez que los datos que llegaron en el frame hayan sido chequeados entonces
esta decide verificar si las data que se recive es una query y es analizada 
si esta query y en dependecia de si es un AQRQ o un ARPR. 

Si la data es una query entonces esta contruye una frame de respuesta ente
la pregunta y este es enviado a esta computadora que le hizo la pregunta (
para saber si direccion mac). 

Si es una respuesta entonces la computadora busca en un diccionario para saber 
que datos estan listos para ser enviados a esa computadora (de los cuales no 
se sabia si direccion mac , pero si su ip). entonces elabora los frames correspondientes 
y los encola para que salgan por el puerto de la computadora hacia su destino. 

#### Configuración 

En el fichero *config.txt* se pueden ajustar varios parámetros para corres el programa

1. **signal_time** : este es el tiempo que un bit tiene que estar transmitiéndose para que sea recibido 

2. **numero_puertos_hub**: Este parámetro son 2 enteros de la forma ***a-b*** separados por el caracter **'-'**  el primer entero ***a*** corresponde con la cantidad mínima de puertos que puede tener un *hub* . Y el segundo ***b*** es la cantidad maxima de puertos que puede tener un hub .

3. **max_cantidad_milisegundos**: Este parámetro es la cantidad máxima de mili-segundos que correrá el programa , después de este tiempo el programa finaliza sin ejecutar ninguna otra instrucción. 


## Aspectos a tener en cuenta 

 EL proyecto esta hecho en C# , por lo tanto si se corre en linux 
se puede usar net.Core 3.1 que fue el que utilice para hacer el 
proyecto 

  No se usan bibliotecas especiales. Aunque no se si pueda haber problema 
porque en algunos casos tuve que usar System.Linq que debe venir pro 
default con .net Core.  