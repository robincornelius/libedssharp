# Requires mono-devel and msbuild:
# https://www.mono-project.com/download/stable/

APP := EDSEditorGUI

all:
	msbuild $(APP)

clean:
	$(RM) -r $(APP)/bin

install:
	cp -Tr $(APP)/bin/Debug /opt/EDSEditor
