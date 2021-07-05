# Build instructions tested on Ubuntu 20.10
#
# 1. Install mono-devel and msbuild:
#
#     https://www.mono-project.com/download/stable/
#
# 2. Install nuget packages
#
#    sudo apt install nuget
#    nuget install SourceGrid -OutputDirectory packages
#

APP := EDSEditorGUI

all:
	msbuild $(APP)

clean:
	$(RM) -r $(APP)/bin

install:
	cp -Tr $(APP)/bin/Debug /opt/EDSEditor
