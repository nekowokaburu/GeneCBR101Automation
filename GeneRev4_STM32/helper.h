// helper.h

#ifndef _HELPER_h
#define _HELPER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

// \brief Available debug modes.
enum class DebugMode : uint8_t
{
  Off = 0x0000,         // No debugging
  Debug = 0x0001,       // Enable debug output to Serial
  NoHardware = 0x0010   // No hardware connected --> Mock hardware
};

inline DebugMode operator|(DebugMode a, DebugMode b)
{
    return static_cast<DebugMode>(static_cast<uint8_t>(a) | static_cast<uint8_t>(b));
}

inline DebugMode operator&(DebugMode a, DebugMode b)
{
    return static_cast<DebugMode>(static_cast<uint8_t>(a) & static_cast<uint8_t>(b));
}

inline bool operator==(DebugMode a, DebugMode b)
{
	return static_cast<uint8_t>(a) & static_cast<uint8_t>(b);
}


// \brief Get value from string message as unsigned integer.
uint8_t GetBetweenAsInt(const char* s, const char* PATTERN1, const char* PATTERN2);

#endif

