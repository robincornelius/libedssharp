I have plans for a C# CanOpen EDS editor

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

* Open EDS file
* Open CanOpenNode XML Project file
* View OD Entries and explore the Object Dictionary
* BROKEN) Export CanOpenNode c/h files (this is work in progress)

![alt tag](pic1.jpg)

