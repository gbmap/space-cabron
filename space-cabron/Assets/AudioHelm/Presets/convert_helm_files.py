import os
import shutil


# Copies and changes the extension of files in directory to .json
def convert_helm_files(dir):
    files = os.listdir(os.getcwd() + "\\" + dir)
    for file in files:
        if ".meta" in file:
            continue
        if ".helm" not in file:
            continue
        original_name = os.getcwd() + "\\" + dir + "\\" + file
        json_name = (os.getcwd() + "\\" + dir + "\\" + file).replace(".helm", ".json")

        if json_name in files:
            continue

        shutil.copy(
            original_name,
            json_name,
        )


dirs = os.listdir(os.getcwd())
for dir in dirs:
    if ".meta" in dir:
        continue
    if "." in dir:
        continue
    if not os.path.isdir(os.getcwd() + "\\" + dir):
        continue 
    convert_helm_files(dir)
