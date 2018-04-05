![Alt text](https://gitlab.lanl.gov/camerontauxe/cinema-unity-viewer/raw/master/doc/images/cinema_logo.png)

# Unity Viewer for Cinema multicamera databases

**Current Version: 1.4.3**

This project is a prototype viewer for Cinema multicamera databases, authored by Cameron Tauxe and Daniel Ben-Naim in the summer of 2016 using the Unity Game Engine.
It was intended to demonstrate the initial capability and vision for exploring multicamera spaces in Cinema. Currently, it only works with SpecA databases

The plan is to open source this viewer. Once that is done, this repository will remain as a record of the initial work on the viewer.

For information on running, using and working on the project, please see the documentation.

The CinemaUnityViewer directory is a Unity Project directory, open it in Unity to work on the viewer.

## Changelog

#### v1.4.3
	- Fixed being able to press Camera Options button through file browsers
#### v1.4.2
	- Fixed databases not loading on Windows machines.
#### v1.4.12
	- Fixed Camera position 3 not saving
#### v1.4.11
	- Removed "[placeholder]" text under camera save file path field.
	- The grid lines surrounding the play area now fade in more smoothly (as originally intended).

#### v1.4.1
	- File browsers now open to the directory of the file specified in the text field
	- Camera options now hidden behind show/hide camera options button on main menu
	- Added notifications when recorded camera angles or writing to disk in the upper-right corner of the screen
	- Added a text read-out when changing FOV
	- Updated documentation to describe camera saving/loading
	- Database Parameters can now be set under "metadata" in the meta database file. These will act as the default unless overriden in a specific run
### v1.4
	- Image processing for transparent backgrounds now done on gpu. No more delay in removing backgrounds
	- Removed Alpha Tool (now obsolete)
	- Removed vignettes. Please clip data in paraview instead.
	- Added documentation in the form of html files to the 'doc' folder
### v1.3
    - Added camera angles (saving, loading, animation) and alpha tool for removing backgrounds before runtime
#### v1.2.1
    - Added "scale" property to metadata in meta database file. See meta database spec for details.
#### v1.2.01
    - Fixed bug where images that had neither an alpha color or vignette would be stuck on the 'loading' sprite.
### v.1.2
    -Initial Commit

