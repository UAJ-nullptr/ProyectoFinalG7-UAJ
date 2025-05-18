import subprocess
import os
import sys

# Función para instalar un paquete
def install_package(package):
    try:
        subprocess.check_call([sys.executable, "-m", "pip", "install", package])
        print(f"{package} instalado correctamente.")
    except subprocess.CalledProcessError:
        print(f"Error al instalar {package}.")

def configure_ffmpeg():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    string_to_remove = "ProyectoFinalUAJG7\\Assets\\SubtitleGenerator"
    ffmpeg_path = script_dir.replace(string_to_remove, "")
    ffmpeg_path = ffmpeg_path + "ffmpeg 7.1.1\\bin"
    return ffmpeg_path

def is_video(file_path):
    try:
        result = subprocess.run(
            ["ffprobe", "-v", "error", "-select_streams", "v:0",
             "-show_entries", "stream=codec_type", "-of", "default=noprint_wrappers=1:nokey=1", file_path],
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True
        )
        return "video" in result.stdout.lower()
    except Exception as e:
        print(f"Error detectando tipo de archivo: {e}")
        return False
    
def video_to_audio(video_path, new_audio_path):
    try:
        subprocess.run(
            ["ffmpeg", "-y", "-i", video_path, "-vn", "-acodec", "pcm_s16le", "-ar", "16000", "-ac", "1", new_audio_path],
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True
        )
        print(f"Audio extraído a {new_audio_path}")
    except subprocess.CalledProcessError as e:
        print(f"Error al convertir a wav: {e}")