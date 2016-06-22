A C# CanOpen EDS (Electronic Data Sheet) library and GUI editor

Please consider this code highly experimental and alpha quality and buggy
It is a work in progress and is rapidly changing.

Current Features

Library
-------

* Read EDS file and parse contents to approprate classes
* Dump EDS classes via ToString()
* Save EDS classes back to EDS file
* Read CanOpenNode xml project file
* Write CanOpenNode xmlproject file
* Switch formats between EDS and CanOpenNode XML (note to EDS will result in
  data loss as the format supports less information).

GUI
---
* Open multiple devices
* Open EDS file
* Save EDS file
* Open CanOpenNode XML Project file
* Save CanOpenNode XML File
* View OD Entries and explore the Object Dictionary
* Add new OD entries
* Delete exisiting OD entries

Not finished/broken
-------------
* Export CanOpenNode c/h files (this is work in progress) the code is mostly 
stubbed out and partially working but needs completing

* Cannot add/remove entries to ARRAY and REC objects

* Unit tests are totaly broken, due to massive rewrite/change of plan after
tests were written.

* PDO page is empty


![alt tag](pic1.jpg)
![alt tag](pic2.jpg)

