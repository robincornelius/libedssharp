A C# CanOpen EDS (Electronic Data Sheet) library and GUI editor

Please consider this code highly experimental and alpha quality and buggy
It is a work in progress and is rapidly changing.

If you would like to try a pre compiled version, then head over to the [releases page!](https://github.com/robincornelius/libedssharp/releases)


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
* Export C and H files in CanOpenNode format CO_OD.c and CO_OD.h

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
* Create new Devices
* Add default profiles
* Create profiles that can be added to any project (just save the device xml file to the profiles/ 
directory, only include the minimum number of objects that you want to auto insert) This will auto add to insert menu
* Edit Device and File Info sections
* Set TX PDO mappings
* Set RX PDO mappings

Not finished/broken
-------------

* Unit tests are totaly broken, due to massive rewrite/change of plan after
tests were written.

![alt tag](pic1.jpg)
![alt tag](pic2.jpg)
![alt tag](pic3.jpg)
