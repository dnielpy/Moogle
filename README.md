#Moogle!

"Moogle!" es una aplicación web cuyo objetivo es realizar búsquedas en una base de 
datos de archivos .txt y devolver los resultados en el menor tiempo posible. Para esto, se 
utilizan algoritmos de TF-IDF y otros conocimientos de álgebra lineal que hacen más 
eficiente el buscador

¿Cómo ejecutar el proyecto?
En la carpeta raíz del proyecto, ejecute el comando ´dotnet watch run --project MoogleServer´. Este comando inicializará el servidor local de la aplicación. Luego (en caso de que de forma automática no se haya lanzado) abra un navegador y ingrese la dirección 'http://localhost:5285/' para ver el contenido de la web

¿Cómo funciona?
El objetivo de Moogle! es realizar búsquedas en el interior de varios archivos .txt y en función del contenido de los mismos, mostrar los resultados más relevantes de acuerdo a la búsqueda que usted haya realizado. Para esto, usted debe copiar los archivos .txt a los cuales quiera realizarle la búsqueda en la carpeta Content que aparece en la raiz del proyecto. La cantidad mínima de archivos .txt que el proyecto debe tener en la carpeta Content para funcionar de manera correcta es de 2 archivos. Los cuales ya se encuentran en dicha carpeta. (Siéntase libre de borrarlos y copiar sus propios archivos .txt siempre y cuando la cantidad mínima de estos sea 2).

Desde el terminal, una vez ejecutado e proyecto, podrá ver el proceso de carga de la base de datos. El cual no debería demorar mas de unos pocos segundos. Claro, todo dependerá de la cantidad de archivos .txt que usted tenga en la carpeta Content. Pero para una base de datos de 50 mb de archivos .txt (alrededor de 30) el algoritmo solo demora entre 4 a 6 segundos en cargar estos. La carga de la base de datos solo se realiza en la primera búsqueda que usted realice, luego se almacena en memoria, por lo que esta primera búsqueda demorará unos pocos segundos más que las siguientes que usted realice. 

El resto es bastante intuitivo, solo escriba su término de búsqueda y el programa se encargará de hacer el resto.
