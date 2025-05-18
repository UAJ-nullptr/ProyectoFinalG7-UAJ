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

create_virtualenv()
install_model()