#!/usr/bin/bash
# https://stackoverflow.com/questions/8264323/how-to-compile-a-visual-studio-c-sharp-project-with-mono
# apt-get install mono-complete
# sudo apt install gnome-themes-standard
mkdir build; cd build; cmake ..; make; cd ..

# build dependencies
xbuild /p:Configuration=Release GPFileTools/GPFileTools.csproj; mv GPFileTools/bin/Release/GPFileTools.dll Fps
xbuild /p:Configuration=Release NumericControls/NumericControls.csproj; mv NumericControls/bin/Release/NumericControls.dll Fps
xbuild /p:Configuration=Release mapack/Source/Mapack.csproj; mv mapack/Build/Release/Mapack.dll Fps
xbuild /p:Configuration=Release Fps.ScatterPlot/springtheory_scatterplot.csproj

# build FPS GUI
xbuild /p:Configuration=Release FPS.sln
