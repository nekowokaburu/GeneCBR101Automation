// roaster.h

#ifndef _ROASTER_h
#define _ROASTER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif


class MAX31855;
enum class DebugMode : uint8_t;

class Roaster
{
   public:
    Roaster(const uint8_t fanPin, const uint8_t drumPin, const uint8_t ssrPin,
            const uint8_t endSwitchPin, const float maxRoasterTemp, const DebugMode debugMode);

    //! \brief Manage roast process and give current temperature.
    //! \return Current roaster temperature
    const uint8_t ManageRoaster();

    bool UseArtisanPID() const noexcept;
    
    void SetUseArtisanPID(bool UseArisanPID) noexcept;

    void Start() noexcept;

    void Stop() noexcept;

    void Cool() noexcept;

    void SetTemperature(const uint8_t newTemp) noexcept;
    
    const uint8_t Temperature() const noexcept;

protected:
  enum class RoasterState
  {
    Idle = 0,
    Started,
    Stopped,
    Cooling
  };

  void RegulateHeater() noexcept;
  void ReturnDrum() const noexcept;

private:
    const uint8_t drumSpeed_{127};
    const uint8_t drumSpeedSlow_{80};
    const uint8_t fanSpeed_{255};
    const uint8_t tempCool_{80};

    RoasterState state_{RoasterState::Idle};
    const uint8_t fanPin_;
    const uint8_t drumPin_;
    const uint8_t ssrPin_;
    const uint8_t endSwitchPin_;
    const float   maxRoasterTemp_;
    const DebugMode debugMode_;

    unsigned long windowStartTime_{0};
    unsigned long windowSize_{3000}; // Same as minimum Artisan update interval in ms

    uint8_t setpoint_;
    uint8_t temperature_{0};

    bool useArtisanPID_{false};

    MAX31855* thermocouplePtr_;
  };

#endif
