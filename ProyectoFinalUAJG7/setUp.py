import os
import whisper
import venv
import sys
import subprocess

# Ruta de nuestro entorno que queremos crear
env_dir = os.path.abspath("myENV")

def create_virtualenv():
    if not os.path.exists(env_dir):
        print("Creando entorno virtual...")
        venv.create(env_dir, with_pip=True)
        subprocess.run(os.path.join(env_dir, "Scripts", "activate"),shell=True,check=True)
        print("Entorno virtual creado con exito: ", env_dir)
    else:
        print("Entorno virtual ya existe.")

def install_model():
    print("Descargando modelo medium...")
    model = whisper.load_model("medium")
    print("Modelo descargado con exito")
    # Función para instalar un paquete
def install_package(package):
    try:
        subprocess.check_call([sys.executable, "-m", "pip", "install", package, "-t", os.path.join(env_dir, "Lib", "site-packages")])
        print(f"{package} instalado correctamente.")
    except subprocess.CalledProcessError:
        print(f"Error al instalar {package}.")

def configure_ffmpeg():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    string_to_remove = "ProyectoFinalUAJG7\\Assets\\SubtitleGenerator\\Python"
    ffmpeg_path = script_dir.replace(string_to_remove, "")
    ffmpeg_path = ffmpeg_path + "ffmpeg 7.1.1\\bin"
    return ffmpeg_path

create_virtualenv()
# Paquetes necesarios
packages = [
    "openai-whisper>=20230314"
]

### CONFIGURACION DEL ENTORNO ###
# Instalar todos los paquetes
for package in packages:
    install_package(package)
print("Instalación completa de todos los paquetes.")

install_model()