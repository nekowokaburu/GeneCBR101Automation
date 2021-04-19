# GeneCBR101Automation - An inexpensive home roasting alternative for all platforms

## Disclaimer ##
___This site, I or the manufacturer of Gene Café CBR-101 shall not be responsible or liable, directly or
indirectly, for any damage or loss caused or alleged to be caused by or in connection with use of or
reliance of this information. No approval has been given by the manufacturer of Gene Café CBR-101
for any modifications to the Gene Café CBR-101. Any modifications done should be carried out by
qualified electrical personnel. 
Applying this mod to your Gene CBR 101 or similar roaster will not only void your warranty but be
irreversible where cuts to the casing are made. Also you should know what you are doing when
dealing with electricity and heat. This document makes no claim for correctness or completeness.___

## This mod enables support for your roster for:
* __Artisan on Windows/Mac/Linux__
* __Standalone apps on Windows(x64/x86)/iOS/Android/ARM which include:__
  * __Manual roasting__
  * __Following custom roast profiles__
  * __Control over the roaster (start cooling, return drum, ...)__

This is a mod which I am currently using with my Gene CBR 101 home coffee roaster. It consists of a new harware build, which connects to your home wifi network. The roaster can then be connected to using any app and platform you like for a better roasting experience. You can use and adapt this repo as you want for use with different roaster etc. Contibutions or donations are greatly appreciated.
If you are confident in your modding skills but do not have the time to get the code to build (there is some setup to be done) you can write me. I may have some pre-programmed PCBs left which I could sent to you.

<div align="center">
  <a href="https://www.youtube.com/watch?v=1ynHtYo9W6A"><img src="https://img.youtube.com/vi/1ynHtYo9W6A/0.jpg" alt="Rough function overview"></a>
</div>

### Downloads ###
[Android app on Google Play](https://play.google.com/store/apps/details?id=com.companyname.GeneRev4)
TODO: Add more binaries here.

### How the apps work ###
The microcontoller controls the roaster hardware using basic commands received on the serial port over tcp. Current commands are:
- _Start_: Start the roaster (drum and fan on), set temp to setpoint (heater on) and try to hold it there.
- _Stop_: Return the drum and turn the fan off if the temperature is below 80 °C.
- _Cool_: Run both drum and fan until the roaster is below 80 °C.
- _temp_: Set a new setpoint as new roasting temperature.

The app communicates with the microcontroller and sends the signal for above commands, thus controlling the whole roast process. In turn the roaster sends the current temperature value to the app for display.
A slider can be used to set a new temperature during roast while the time is stopped by the app.
Furthermore, previously created roast profiles can be loaded as _*.csv_ or _*.txt_. These must hold the time temperature pairs at which time the roast temperature is to be changed automatically. This allows for a fully automatic roast process.

### How the Artisan setup works ###
If you don't know Artisan, it is basicialy above apps with development started 10 years ago. It is widely used in the coffee industy up to mid sized roasters (around 100 kg per batch).
With this mod applied you can connect your roaster wirelessly to your PC running Artisan. Use the scripts provided in _GeneRev4Py_ to set the button controls and temperature response accordingly. An examply of my initial settings is also attached (_GeneRev4Py/artisan-settings.aset_) which can be loaded directly with Artisan. Check your paths to the files tough!

### New roast hardware ###
You will need to modify your roaster a bit. To help with this step, a new PCB is available which will allow you to keep the original connectors. Only a new K-Type thermocouple will be needed for measuring the air temperature using a MAX31855. Hints on the hardware setup are in the [old version](https://github.com/nekowokaburu/GeneCBR101Automation/tree/master/doc)) of this mod. Also check the [STM32 source file](https://github.com/nekowokaburu/GeneCBR101Automation/blob/master/GeneRev4_STM32/GeneRev4_STM32.ino) for used hardware and the connection setup/routing if you want to build the PCB yourself or make adaptions.

![PCB](https://github.com/nekowokaburu/GeneCBR101Automation/blob/master/GeneRev4_PCB/pcb_2d_rendering.png?raw=true)

### TCP setup ###
For the TCP communication to work, I used an ESP8266 as ESP-01S module. This setup is currently not included in this repo. You can choose your prefered communication. My current version just connects to your local wifi, allows for a connection of multiple clients and forwards all serial commanication from the microprocessor to all connected clients and vice versa. [Here is a good general explanation and code example (See TCP Server)](http://stefanfrings.de/esp8266/)

### Remarks ###
- Since I don't have an iOS device, only the Windows and Android apps were built and tested.
- To install the Windows UWP app, install it from the project directly. Currently there is no prebuilt and signed installer. available
