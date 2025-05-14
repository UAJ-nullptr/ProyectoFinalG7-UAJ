import os
import subprocess

# Función para instalar un paquete
def install_package(package):
    try:
        subprocess.check_call(["pip", "install", package])
        print(f"{package} instalado correctamente.")
    except subprocess.CalledProcessError:
        print(f"Error al instalar {package}.")

# Crear un entorno virtual
env_name = "Transcription_env"
print(f"Creando entorno virtual: {env_name}")
subprocess.run(["python", "-m", "venv", env_name])

# Activar el entorno virtual
if os.name == "nt":
    activate_script = os.path.join(env_name, "Scripts", "activate")
else:
    activate_script = os.path.join(env_name, "bin", "activate")

print(f"Activando entorno virtual: {activate_script}")
subprocess.run(["source", activate_script], shell=True)

# Paquetes necesarios
packages = [
    "torch>=1.10.0",
    "torchaudio>=0.10.0",
    "pyannote.audio>=2.1.1",
    "librosa>=0.10.0",
    "openai-whisper>=20230314",
    "numpy>=1.21.0",
    "scipy>=1.7.0",
    "einops>=0.6.1",
    "pytorch-lightning>=2.0.0",
    "pyannote.core>=5.0.0",
    "soundfile>=0.11.0"
]

# Instalar todos los paquetes
for package in packages:
    install_package(package)

print("Instalación completa de todos los paquetes.")

#Una vez instalados los paquetes, se hace la transcripción

from pyannote.audio.pipelines import SpeakerDiarization
import whisper
import librosa
import soundfile as sf
from pathlib import Path
from pyannote.audio import Pipeline
#Paso 1: Diarización
print("Carpeta actual:", os.getcwd())
PATH_TO_CONFIG = "pyannote\config.yaml"
pipeline = Pipeline.from_pretrained(PATH_TO_CONFIG)
# Cargar el pipeline de diarización de pyannote

# Procesar el archivo de audio (Habrá que editarlo para que reciba la path desde la ventana)
audio_file = 'audio.wav' # PathToAudio
diarization = pipeline({'uri': 'audio', 'audio': audio_file})

# Por si se quiere mostrar los resultados de la diarización (marcas de tiempo y hablantes)
#for speech_turn, _, speaker in diarization.itertracks(yield_label=True):
    #print(f"Speaker {speaker} speaks from {speech_turn.start} to {speech_turn.end}")

#Paso 2: Separación del audio en fragmentos
# Definir la carpeta de salida
output_folder = "audio_segments"
os.makedirs(output_folder, exist_ok=True)

# Cargar el archivo de audio completo
audio, sr = librosa.load(audio_file, sr=None)

# Lista para almacenar los segmentos de audio
segments = []

# Extraer segmentos de audio según las marcas de tiempo de la diarización
for speech_turn, _, speaker in diarization.itertracks(yield_label=True):
    start_sample = int(speech_turn.start * sr)  # Convertir tiempo a muestras
    end_sample = int(speech_turn.end * sr)      # Convertir tiempo a muestras

    # Extraer el segmento de audio
    segment_audio = audio[start_sample:end_sample]
    segment_path = f"{output_folder}\segment_{speaker}_{speech_turn.start:.2f}_{speech_turn.end:.2f}.wav"
    sf.write(segment_path, segment_audio, sr)

    segments.append((segment_path, speaker))
    
# Paso 3: Transcripción
# Cargar el modelo Whisper
# Cargar el modelo Whisper
model = whisper.load_model("base")  # Puedes elegir el modelo "base", "small", "medium", "large"

# Lista para almacenar las transcripciones
transcriptions = []

# Procesar cada segmento y transcribir
for segment, speaker in segments:
    print(os.path.abspath(segment))
    # Transcribir el segmento de audio
    result = model.transcribe(os.path.abspath(segment))

    # Guardar transcripción en lista
    transcriptions.append((speaker, result['text']))

# Guardar las transcripciones en un archivo de texto
with open("transcription.txt", "w") as txt_file:
    for speaker, text in transcriptions:
        txt_file.write(f"Speaker {speaker}: {text}\n")