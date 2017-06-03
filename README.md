A C# CanOpen EDS (Electronic Data Sheet) library and GUI editor

This application is designed to load/save/edit and create EDS file for CanOpen 
and also to generate the object dictionary for CanOpenNode (CO_OD.c and CO_OD.h)
to aid development of CanOpenNode devices. 

The EDS editor on its own is useful without the CanOpenNode specific export and 
as of the 0.6-XDD-alpha version the editor can also load XDD files (save of XDD
is not yet supported). The GUI also shows PDO mappings and can generate reports
of multiple devices that are loaded into the software.

The core library can be used without the GUI to implement eds/xdd loading/saving
and parsing etc in other projects.

Please consider this code experimental and beta quality. 
It is a work in progress and is rapidly changing.

With many thanks to the following contributors for spotting my mistakes and 
improving the code
	* s-fuchs 
	* martinwag 
	* trojanobelix

Releases
--------

If you would like to try a pre compiled version, then head over to the [releases page!](https://github.com/robincornelius/libedssharp/releases)

Current beta: 0.5.2-beta
Current alpha: 0.6-XDD-alpha


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
* Set RX/TX PDO mappings easily from dropdown lists of avaiable objects
* Add and remove new PDO entries (communication paramaters and mapping) in a single button push
* Save groups of EDS/XML files as a network objects with abality to set concrete node IDs
* View report of all configured PDO across the network

In the alpha version we also have
* Support for loading XDD files (CanOpen offical XML) - This is still a work in 
progress although all my test files are loading with out issue and populating 
the various Device Info, File info and Object Dictionary correctly.

TODO
----

* Ensure and validate all XDD is loading 
* XDD saving
* Add extra Gui fields for accessing extra XDD paramaters not in EDS
* Look at XDC files and see if we can save config changes and allow editing and
  network setup here in the app

Pictures
--------

![alt tag](pic1.jpg)
![alt tag](pic2.jpg)
![alt tag](pic3.jpg)
![alt tag](pic4.jpg)