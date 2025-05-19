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
    # Función para instalar un 
    
def install_package(package):
    try:
        subprocess.check_call([sys.executable, "-m", "pip", "install", package, "-t", os.path.join(env_dir, "Lib", "site-packages")])
        print(f"{package} instalado correctamente.")
    except subprocess.CalledProcessError:
        print(f"Error al instalar {package}.")

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