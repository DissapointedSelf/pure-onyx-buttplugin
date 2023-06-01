import os
import shutil

os.system("dotnet build")
src = '"F:\\sys_logs\\g4\\!unity test\\old pure onyx\\OnyxPlugin\\bin\\Debug\\netstandard2.0\\OnyxPlugin.dll"'
dst = '"F:\\sys_logs\\g4\\!unity test\\PURE ONYX - 2023-04-30 Patched Release\\BepInEx\\plugins\\OnyxPlugin.dll"'
shutil.copyfile(src, dst)