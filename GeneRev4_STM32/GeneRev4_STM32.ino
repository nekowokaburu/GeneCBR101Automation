/*
#######################################################################################################
########################################## Software setup #############################################
#######################################################################################################
vMicro:
                - Install Extension Arduino for Visual Studio (vMicro)
                - Restart VS (Visual Studio)
                - Set Arduino IDE location if necessary
                - Build/Activate trial to get build settings

Board:
                - Add url in Arduino IDE
http://dan.drown.org/stm32duino/package_STM32duino_index.json
                - Add board in boards manager: STM32F1xx / GD32F1xx boards by
stm32duino
                - Restart arduino IDE/rescan toolchain in vMicro or also restart
VS
                - Select "Generic STM32F103C series, 72 MHz" board
VS Code         - Arduino: Board Config, also set upload method to serial

Libraries:
                - In "C:\Program Files (x86)\Arduino\libraries":
                                        git clone
https://github.com/enjoyneering/MAX31855.git
                - Restart arduino IDE/rescan toolchain in vMicro or also restart
VS


*******************************************************************************************************
************************************ Notes for creating the PCP ***************************************
*******************************************************************************************************

--> LF33CV 3.3 V power supply, do not add to own 9 V power supply, other chip
can't handle the load! left pin: n V +, middle pin gnd, right pin out 3.3 V +
--> 2N3904 NPN transistor for SSR
                left pin: SSR -, middle pin 1k Ohm resistor and then PB0
(SSR_PIN), right pin gnd >= 5V (see SSR spec for max. V)

--> Gene
                - 12 V relais , mechanical originally used, I set it to always
on, now it can be removed as well!
                - My power supply goes from the 40 V AC in the Gene (converter
already installed, maybe check rating in old pictures) to 40 V DC with rectifier
bridge
                - 24 V (stepped down from 40 V, transistors (Tip 120, cooled),
used for
                                + new fan (24 V), original was 12 V I think, but
it broke quickly, fan now always 255 (100 %)
                                + original drum motor, 12 V, so the max. speed
is 127 (out of 255, ~50 %)
                - 9 V (stepped down from 24 V), used for Arduino Mega, now only
for the 12 V original relais
                                + 12 V original relais set to always open
                                + Not enough power here to step down to 3.3 V
again!
                                + Can be completely ignored in future or used by
others for 12 V Fan and drum
                - MAX31855 MUST BE ON 5 V of STM32! Else reads 0 all the time,
chinese crap!


--> General:
                - Kp = 720, Ki = 0, Kd = 0, PID is not needed?!
                - Max31855 with (non adafruit) library
                - SR relay 25 A
                - 2 x NPN-220 TIP120 ic darlington-transistor (Fan and Drum)
                - 75x75x30 mm 24 V radial fan to replace the existing 12 V
--> Power supply (buy one, don't build this anymore!):
                - KBP206 Single Phase 2.0 ampere bridge rectifier
                - lm350t adjustable voltage regulator (adjust to 24 V with
resistors of your choice, see internet for manual)
                - 25 V 2200 µF
                - 2 x 25 V 220 µF (on 9 V side --> Ignore in PCB?!)
--> Pin connections looking at the built in part in the roaster (TCP-Version):
                From upper left to upper right, then one line down, my naming
is: 1,  2,  3,  4,  5 6, 7, 8, 9, 10 11, 12, 13, 14, 15

                1) Drum PWM
                2) Fan PWM
                3) SSR (PWM/HIGH/LOW, npn transistor!)
                4) Gnd
                5) End position switch, non Gnd part/ other size of switch to
Gnd, thus use INPUT_PULLUP 
                6) MAX31855 CLK = A5
                7) MAX31855 CS = A7
                8) MAX31855 DO = A6
                9) MAX31855 Vin, use 5 V! 3.3 not working properly on chinese crap!
                --> 9 pins overall to connect to roaster + 1 more 3.3 V "+"

                On board only connections:
                ESP-01 s (Wireless, ONLY USE "s" version since it does not need the 
                EN and reset pins tied to Vcc!!!):
                  - VCC: 3.3V
                  - GND: Gnd
                  - 
power pin for PCB version


--> PCB SETUP AND COMPONENTS:
                - Remove all electronic crap from roaster
                - Buy 230 V to 24 V transformator! (12 V for people with 12 V
fan!)
                - Buy/Build 24 V or 12 V stepdown to 3.3 V
                - 2 x TIP120 for fan and drum
                - 1 x 2N3904 NPN transistor for SSR
*/

#include "helper.h"
#include "roaster.h"

#define FAN_PIN PA0  // @ 4k7 Ohm
#define DRUM_PIN PA1 // @ 2k Ohm
#define ROTENDSWITCH_PIN PB1
#define SSR_PIN PB0
#define MAX_ROASTER_TEMP                                                       \
  250 // No more then 250 degree celsius on the Gene CBR roaster, used for
      // Artisan PID heater load computation.
// AUto defined since hardware SPI pins! for harware wiring reference only
// thermoCLK PA5
// thermoCS  PA7
// thermoDO  PA6
// Rx of serial port on PA9
// Tx of serial port on PA10
// ESP8266 TCP board Tx to PA3
// ESP8266 TCP board Rx to PA2

const DebugMode DEBUG_MODE = DebugMode::Off;   //DebugMode::Debug | DebugMode::NoHardware;      //
                    // Communication to and from App
const int TCP_BUFFER_SIZE = 30;
uint8_t oldTemp = 0;
uint8_t newTemp = 1;

Roaster *roasterPtr = nullptr;

void setup() {
  delay(1000);
  Serial.begin(115200); // Give serial some time so Wifi module can't spam it (= fast blinking LED on stm32)
  Serial2.begin(115200);
  Serial2.setTimeout(50);

  if (DEBUG_MODE == DebugMode::Debug) {
    Serial.println("Started Serial!");
    // Serial2.println("TCP Started Serial2!");
  }

  roasterPtr = new Roaster(FAN_PIN, DRUM_PIN, SSR_PIN, ROTENDSWITCH_PIN,
                           MAX_ROASTER_TEMP, DEBUG_MODE);
}

void loop() {
  // React to app commands
  uint16_t strLen = 0;
  char receivedMessage[TCP_BUFFER_SIZE] = {'\0'};

  while (Serial2.available()) {
    receivedMessage[strLen] += (char)Serial2.read();
    ++strLen;
  }

  if (strLen > 0) {
    if (DEBUG_MODE == DebugMode::Debug)
      Serial1.println(receivedMessage);

    if (strstr(receivedMessage, "ArtisanOn"))
      roasterPtr->SetUseArtisanPID(true);

    if (strstr(receivedMessage, "ArtisanOff"))
      roasterPtr->SetUseArtisanPID(false);

    if (strstr(receivedMessage, "Start")) {
      roasterPtr->SetTemperature(0);
      roasterPtr->Start();
    }

    if (strstr(receivedMessage, "Stop"))
      roasterPtr->Stop();

    if (strstr(receivedMessage, "Cool"))
      roasterPtr->Cool();

    if (strstr(receivedMessage, "temp")) {
      if (DEBUG_MODE == DebugMode::Debug)
        Serial1.println("Getting temp from " + String(receivedMessage));
      auto msgData =
          GetBetweenAsInt(receivedMessage, (char *)"temp", (char *)";");
      if (DEBUG_MODE == DebugMode::Debug)
        Serial1.println("Calling SetTemp with " + String(msgData));
      roasterPtr->SetTemperature(msgData);
      Serial2.println(String(msgData));
    }
  }

  // Send current temperature to app
  newTemp = roasterPtr->ManageRoaster();
  Serial2.println(newTemp);
  // if (oldTemp != newTemp)
  // {
  // Serial2.println(newTemp);
  // if (DEBUG_MODE == DebugMode::Debug)
  // 	Serial.println(newTemp);
  // oldTemp = newTemp;
  // }
  delay(300); // Do not flood the serial port
}
