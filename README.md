
THIS BRANCH IS OBSOLETE PLEASE USE xdd branch



A C# CanOpen EDS (Electronic Data Sheet) library and GUI editor

Please consider this code highly experimental and alpha quality and buggy
It is a work in progress and is rapidly changing.

With many thanks to the following contributors for spotting my mistakes and 
improving the code
	* s-fuchs 
	* martinwag 

Releases
--------

If you would like to try a pre compiled version, then head over to the [releases page!](https://github.com/robincornelius/libedssharp/releases)

Current version 0.4-beta - 

* Known issues #51, #36 both fixed in git

Assuming no one shouts too loudly after last set of commits 0.5 will be released some time early Feb 2017


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


![alt tag](pic1.jpg)
![alt tag](pic2.jpg)
![alt tag](pic3.jpg)
![alt tag](pic4.jpg)