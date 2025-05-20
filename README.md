# **Proyecto Final UAJ - Herramienta y componentes de transcripción por IA para subtitulado**

## **Participantes y enlace al drive**

- Pablo González Arroyo
- Daniel Alonso Herranz
- Rafael Vilches Hernández
- Rafael Argandoña Blácido
- Fran Molla Astar

Enlace al Drive : [Drive](https://drive.google.com/drive/folders/1a08olHUXmRyknzD2CMK9ixYf7XTkIO1D?usp=sharing)

# **Propósito**

Para el proyecto final de la asignatura de Usabilidad y Análisis de Juegos hemos decidido crear una herramienta que permita facilitar la tarea de subtitulado de escenas en un videojuego a través de IA. Para ello, la herramienta cuenta con las siguientes características:
- Ventana dentro del editor donde generar, preconfigurar y ajustar las transcripción.
- Carga de un audio/vídeo para el procesado.
- Distinción de interlocutores (diarización).
- Componentes propios para la puesta en escena de los subtítulos customizables.
- Uso de Python en C#, junto con Pyannote y Wisper AI.

# **Instrucciones de uso**

## **1. Creación del entorno virtual:** 
Desde la raíz del proyecto de Unity se debe abrir una terminal con Python y ejecutar *setUp.py* (python setUp.py), es importante encontrarse en esa ubicación porque el entorno virtual debe crearse en ese directorio -> (*ruta_local_proyecto_unity/ProyectoFinalUAJG7*). Al terminar, se debe haber generado en dicho directorio una carpeta llamada *myENV*.

## **2. Transcripción:** 
Para realizar la transcripción del audio/video se debe abrir la ventana `Transcript Audio Tool`. Para abrirla debemos desde el editor de Unity en la barra superior ir a la pestaña *Tools -> Transcript Audio Tool*. Se abrirá una ventana en la que debemos arrastrar el archivo de audio o video al campo llamado `Audio File`. Comenzará el proceso de transcripción al pulsar en el botón `Process`, al finalizar el mismo, se habrá generado en la carpeta *Assets/SubtitleGenerator/Python/audio_segments* los audios de los segmentos y en la carpeta *Assets/SubtitleGenerator/Transcriptions* donde se almacenan los archivos .srt con la transcripción. 

Desde la ventana, se pueden modificar distintos parámetros y posteriormente guardar el resultado de dichas modificaciones quedándonos con el resultado final, para esto debemos pulsar `Save` después de las modificaciones. Alternativamente, también se puede guardar el archivo .srt en cualquier otra ruta del proyecto pulsando el botón `Export`.

## **3. Visualización en escenas de los subtítulos:**
Las escenas están configuradas para que el GameObject llamado *SubtitleManager* con el componente `SubtitleManager` muestre el texto correctamente sin necesidad de muchas configuraciones. Existen dos formas:

- Asignando el archivo .srt generado en el campo `Path` escribiendo la ruta de dicho archivo. Este estará en *Assets/SubtitleGenerator/Transcriptions/<nombre_archivo>.srt* u otra dentro del proyecto en caso de haber realizado la acción *Export*, en cuyo caso estará en la ruta que se haya indicado en ese momento.

- Asigando el `ScriptableObject` generado junto con el .srt al campo `Data`. 

Por último, solo habría que asignar la pista de audio al `AudioSource` de dicho GameObject y, de ser necesario, un vídeo en el `Video PLayer` del GameObject *Screen*.
