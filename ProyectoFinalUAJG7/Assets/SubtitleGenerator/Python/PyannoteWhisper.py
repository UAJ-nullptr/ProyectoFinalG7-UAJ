import os
import shutil
import sys
from Utils import install_package, configure_ffmpeg, is_video, video_to_audio

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
    "soundfile>=0.11.0",
    "ffmpeg-python"
]

### CONFIGURACION DEL ENTORNO ###
# Instalar todos los paquetes
for package in packages:
    install_package(package)
print("Instalación completa de todos los paquetes.")

# Añadir ffmpeg al PATH de manera temporal
ffmpeg_path = configure_ffmpeg()
os.environ["PATH"] = ffmpeg_path + os.pathsep + os.environ["PATH"]
print("ffmpeg encontrado en:", shutil.which("ffmpeg"))

import whisper
import librosa
import soundfile as sf
from pyannote.audio import Pipeline
from whisper.utils import get_writer

### PROCESO DE TRANSCRIPCION ###
# Paso 1: Diarización -  Cargar el pipeline de pyannote
print("Carpeta actual:", os.getcwd())
PATH_TO_CONFIG = os.path.abspath("..\pyannote\config.yaml")
pipeline = Pipeline.from_pretrained(PATH_TO_CONFIG)

# Paso 2: Procesar el archivo de input (si es video se transforma a audio)
input_file = sys.argv[1]
audio_file = {}
if (is_video(input_file)):
    video_name = os.path.splitext(os.path.basename(input_file))[0]
    video_directory = os.path.dirname(input_file)
    audio_file = os.path.join(video_directory, video_name + ".wav")
    video_to_audio(input_file, audio_file)
else:
    audio_file = input_file
diarization = pipeline({'uri': 'audio', 'audio': audio_file})

# Por si se quiere mostrar los resultados de la diarización (marcas de tiempo y hablantes)
#for speech_turn, _, speaker in diarization.itertracks(yield_label=True):
    #print(f"Speaker {speaker} speaks from {speech_turn.start} to {speech_turn.end}")

# Paso 3: Separación del audio en fragmentos
# Definir la carpeta de salida
output_folder = "audio_segments"
# Si existe de un audio anterior, se borra y se crea de nuevo
if os.path.exists(output_folder) and os.path.isdir(output_folder):
        shutil.rmtree(output_folder)
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
    segment_path = os.path.join(output_folder, f"segment_{speaker}_{speech_turn.start:.2f}_{speech_turn.end:.2f}.wav")
    sf.write(segment_path, segment_audio, sr)

    segments.append((segment_path, speaker,speech_turn.start,speech_turn.end))

# Paso 4: Transcripción
# Cargar el modelo Whisper
model = whisper.load_model("medium")

# Lista para almacenar las transcripciones
transcriptions = []
srt_writer = get_writer("srt", ".")
srt_segments = []

# Procesar cada segmento y transcribir
for segment, speaker, start, end in segments:
    # Transcribir el segmento de audio
    result = whisper.transcribe(model,os.path.abspath(segment))
    #result = model.transcribe(os.path.abspath("segment"))
    srt_segments.append({
        'start': start,
        'end': end,
        'text': f"Speaker {speaker}: {result['text']}"
    })

srt_writer({'segments': srt_segments}, audio_file)