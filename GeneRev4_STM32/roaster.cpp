// 
// 
// 

#include "roaster.h"
#include <MAX31855.h>
#include "helper.h"


Roaster::Roaster(const uint8_t fanPin, const uint8_t drumPin, const uint8_t ssrPin,
  const uint8_t endSwitchPin, const float maxRoasterTemp, const DebugMode debugMode = DebugMode::Off)
    : fanPin_{fanPin}
    , drumPin_{drumPin}
    , ssrPin_{ssrPin}
    , endSwitchPin_{endSwitchPin}
		, maxRoasterTemp_{maxRoasterTemp}
		, setpoint_{0}
		, thermocouplePtr_(new MAX31855(3))
		, debugMode_{debugMode}
{
  // Set fan and drum first!
  pinMode(fanPin_, OUTPUT);
  analogWrite(fanPin_, 0);
  pinMode(drumPin_, OUTPUT);
  analogWrite(drumPin_, 0);
  pinMode(ssrPin_, OUTPUT);
  digitalWrite(ssrPin_, LOW);
  pinMode(endSwitchPin_, INPUT_PULLUP);
	
  // Start MAX31855
	if (debugMode_ == DebugMode::Off)
	{
		thermocouplePtr_->begin();
		{
			while (thermocouplePtr_->getChipID() != MAX31855_ID)
			{
				Serial.println(F("MAX6675 error")); //(F()) saves string to flash & keeps dynamic memory free
				delay(5000);
			}
			while (thermocouplePtr_->detectThermocouple() != MAX31855_THERMOCOUPLE_OK)
			{
				switch (thermocouplePtr_->detectThermocouple())
				{
					case MAX31855_THERMOCOUPLE_SHORT_TO_VCC:
						Serial.println(F("Thermocouple short to VCC"));
						break;

					case MAX31855_THERMOCOUPLE_SHORT_TO_GND:
						Serial.println(F("Thermocouple short to GND"));
						break;

					case MAX31855_THERMOCOUPLE_NOT_CONNECTED:
						Serial.println(F("Thermocouple not connected"));
						break;

					case MAX31855_THERMOCOUPLE_UNKNOWN:
						Serial.println(F("Thermocouple unknown error, check spi cable"));
						break;
				}
			}
		}
	}
}

const uint8_t Roaster::ManageRoaster()
{
	// Update temperature while ignoring possible reading errors
		auto tmp = thermocouplePtr_->getTemperature(thermocouplePtr_->readRawData());
		// Newly measured temperature should also be in the arbitrarily taken 20 degree interval
		// or else the new temperature probably has a measurement error, too. If this does not fix
		// some occasional measurement skips, check the TCP connection!
		if (tmp != 0 && tmp != 2000 && tmp >= temperature_ - 20)
			temperature_ = tmp;

	// Mock temperature and temp change delay
	if (debugMode_ == DebugMode::NoHardware)
	{
		//Serial.println("Roaster state: " + String((int)state_));
		temperature_ = static_cast<uint8_t>(millis() / 1000 % 255);
		delay(700);
	}	

		if (debugMode_ == DebugMode::Debug)
		{
		Serial.print("temperature_ = ");
		Serial.println(temperature_);
		}

	switch (state_)
	{
	case Roaster::RoasterState::Idle:
		break;
	case Roaster::RoasterState::Started:
		RegulateHeater();
		break;
	case Roaster::RoasterState::Stopped:
		if (temperature_ <= tempCool_)
			analogWrite(fanPin_, 0);
		break;
	case Roaster::RoasterState::Cooling:
		if (temperature_ <= tempCool_)
		{
			ReturnDrum();
			analogWrite(fanPin_, 0);
			state_ = RoasterState::Idle;
		}
		break;
	default:
		break;
	}
	return temperature_;
}

bool Roaster::UseArtisanPID() const
{
  return useArtisanPID_;
}
    
void Roaster::SetUseArtisanPID(bool UseArisanPID) noexcept
{
  useArtisanPID_ = UseArisanPID;
}

void Roaster::Start()
{
  analogWrite(fanPin_, fanSpeed_);
  analogWrite(drumPin_, drumSpeed_);
  state_ = RoasterState::Started;
  windowStartTime_ = millis();
}

void Roaster::Stop()
{
	ReturnDrum();
	state_ = RoasterState::Stopped;
}

void Roaster::Cool()
{
	digitalWrite(ssrPin_, LOW);
	analogWrite(fanPin_, fanSpeed_);
	analogWrite(drumPin_, drumSpeed_);
	state_ = RoasterState::Cooling;

	if (debugMode_ == DebugMode::NoHardware)
		Serial.println("Successfully cooled down roaster.");
}

const uint8_t Roaster::Temperature() const
{		
	return temperature_;
}

void Roaster::SetTemperature(const uint8_t newTemp)
{
	if(debugMode_ == DebugMode::Debug)
		Serial.println("Set temperature to " + String(newTemp));
	setpoint_ = newTemp;
}

void Roaster::RegulateHeater()
{
	if (debugMode_ == DebugMode::Debug)
	{
		Serial.print("Using Artisan: ");
		Serial.println(UseArtisanPID());
		Serial.print("setpoint_ = ");
		Serial.println(setpoint_);
		Serial.print("ssrPin_ HIGH? = ");
		Serial.println(ssrPin_ == HIGH);
	}

	if (UseArtisanPID())
	{
		auto now = millis();
		auto windowProgress = now - windowStartTime_;
		// Serial.print("Window Progress = ");
		// Serial.println(windowProgress);
		// Serial.print("Window StartTime = ");
		// Serial.println(windowStartTime_);
		
		// Update PID heater interval window
		if(windowProgress > windowSize_)
		{
			// Serial.println("Adding to windwoStartTime");
			windowStartTime_ += windowSize_;
		}
		
		auto pidResult = (setpoint_ / maxRoasterTemp_) * windowSize_;
		// Serial.print("SP = ");
		// Serial.print(setpoint_);
		// Serial.print(" - PID result = ");
		// Serial.println(pidResult);
		if(pidResult > windowProgress)
		{
			Serial.println("HeaterON");
			digitalWrite(ssrPin_, HIGH);
		}
		else
		{
			digitalWrite(ssrPin_, LOW);
			Serial.println("HeaterOFF");
		}
	}
	else
	{
		if (temperature_ > setpoint_)
			digitalWrite(ssrPin_, LOW);
		else if (temperature_ <= setpoint_)
			digitalWrite(ssrPin_, HIGH);
	}
}


void Roaster::ReturnDrum() const
{
	digitalWrite(ssrPin_, LOW);
  analogWrite(drumPin_, drumSpeedSlow_);
	while (digitalRead(endSwitchPin_) == 1)
	{
		delay(10);
		if (debugMode_ == DebugMode::NoHardware)
		{
			Serial.println("Successfully reset drum.");
			break;
		}
	}
  analogWrite(drumPin_, 0);
}
