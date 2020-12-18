# GeneCBR101Automation - An inexpensive home roasting alternative for all platforms

## This mod enables support for your roster for:
* __Artisan on Windows/Mac/Linux__
* __Standalone apps on Windows/iOS/Android which include:__
  * __Manual roasting__
  * __Following custom roast profiles__
  * __Control over the roaster (start cooling, return drum, ...)__

This is a mod which I am currently using with my Gene CBR 101 home coffee roaster. It consists of a new harware build, which connects to your home wifi network. The roaster can then be connected to using any app and platform you like for a better roasting experience. You can use and adapt this repo as you want for use with different roaster etc. Contibutions or donations are greatly appreciated.
If you are confident in your modding skills but do not have the time to get the code to build (there is some setup to be done) you can write me. I may have some pre-programmed PCBs left which I could sent to you.

### How the apps work ###
The microcontoller controls the roaster hardware using basic commands received on the serial port over tcp. Current commands are:
- _Start_: Start the roaster (drum and fan on), set temp to setpoint (heater on) and try to hold it there.
- _Stop_: Return the drum and turn the fan off if the temperature is below 80 °C.
- _Cool_: Run both drum and fan until the roaster is below 80 °C.
- _temp_: Set a new setpoint as new roasting temperature.

The app communicates with the microcontroller and sends the signal for above commands, thus controlling the whole roast process. In turn the roaster sends the current temperature value to the app for display.
A slider can be used to set a new temperature during roast while the time is stopped by the app.
Furthermore, previously created roast profiles can be loaded as *.csv or *.txt. These must hold the time temperature pairs at which time the roast temperature is to be changed automatically. This allows for a fully automatic roast process.

### How the Artisan setup works ###
If you don't know Artisan, it is basicialy above apps with development started 10 years ago. It is widely used in the coffee industy up to mid sized roasters (around 100 kg per batch).
With this mod applied you can connect your roaster wirelessly to your PC running Artisan. Use the scripts provided in _GeneRev4Py_ to set the button controls and temperature response accordingly. An examply of my initial settings is also attached (_GeneRev4Py/artisan-settings.aset_) which can be loaded directly with Artisan. Check your paths to the files tough!


XXXXXXXXXXXXXXXXXX Check if my login to appstore etc is in VSproject saved?!?!?!

#### New roast hardware ####


#### TCP ####

##### TCP setup #####


##### TCP communication #####


### Apps in this package ###

This project contains (more or less) working solutions for three platforms:
- Android
- iOS
- Windows

Since I don't have an iOS device, only the Windows and Android apps built and tested.



