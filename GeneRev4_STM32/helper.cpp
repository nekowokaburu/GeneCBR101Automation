// 
// 
// 

#include "helper.h"

uint8_t GetBetweenAsInt(const char* s, const char* PATTERN1, const char* PATTERN2)
{
    char* target = NULL;
    if (const char* start = strstr(s, PATTERN1))
    {
        start += strlen(PATTERN1);
        if (const char* end = strstr(start, PATTERN2))
        {
            target = (char*)malloc(end - start + 1);
            memcpy(target, start, end - start);
            target[end - start] = '\0';
        }
    }
    auto val = atoi(target);
    free(target);
    return static_cast<uint8_t>(val);
}
