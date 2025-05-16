import os
import shutil

script_dir = os.path.dirname(os.path.abspath(__file__))
string_to_remove = "ProyectoFinalUAJG7\\Assets\\SubtitleGenerator"

ffmpeg_path = script_dir.replace(string_to_remove, "")

ffmpeg_path = ffmpeg_path + "ffmpeg 7.1.1\\bin"

print(ffmpeg_path)


#os.environ["PATH"] = r"C:\Users\pavip\Desktop\Universidad\ProyectoFinalG7-UAJ\ffmpeg 7.1.1\bin" + os.pathsep + os.environ["PATH"]
os.environ["PATH"] = ffmpeg_path + os.pathsep + os.environ["PATH"]
print("ffmpeg encontrado en:", shutil.which("ffmpeg"))