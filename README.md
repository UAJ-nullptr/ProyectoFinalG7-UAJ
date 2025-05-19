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
- Ventana dentro del editor donde generar, preconfigurar y ajustar las transcripción
- Carga de un audio/vídeo para el procesado
- Distinción de interlocutores (diarización)
- Componentes propios para la puesta en escena de los subtítulos customizables
- Uso de Python y Wisper AI

# **Instrucciones**

## **1. Creación del entorno virtual:** 
Desde la raíz del proyecto de Unity se debe abrir una terminal con Python y ejecutar *setUp.py* (python setUp.py), es importante encontrarse en esa ubicación porque el entorno virtual debe crearse en ese directorio -> *ProyectoFinalUAJG7*. Al terminar, se debe haber generado en dicho directorio una carpeta llamada *myENV*.
## **2. Transcripción:** 
Para realizar la transcripción del audio/video se debe abrir la ventana *Transcript Audio Tool*. Para abrirla debemos desde el editor de Unity en la barra superior ir a la pestaña *Tools -> Transcript Audio Tool*. Se abrirá una ventana en la que debemos arrastrar el archivo de audio o video al campo llamado *Audio File*. Comenzará el proceso de transcripción, al finalizar el mismo, se habrá generado en la carpeta *Assets/SubtitleGenerator/Python/audio_segments* los audios de los segmentos y en la carpeta *Assets/SubtitleGenerator/Transcriptions* donde se almacenan los archivos .srt con la transcripción. Desde la ventana, se pueden modificar distintos parámetros y posteriormente guardar el resultado de dichas modificaciones quedándonos con el resultado final.
## **3. Visualización en escenas de los subtítulos:**
Las escenas están configuradas para que asignando el archivo .srt generado al GameObject llamada *SubtitleManager* con el componente *SubtitleManager* en el campo path la ruta de dicho archivo, es la indicada en el punto anterior -> *Assets/SubtitleGenerator/Transcriptions/<nombre_archivo>.srt*. Y asignar la pista de audio al *AudioSource* de dicho GameObject.
