#pragma once

extern "C" {
// #include "c/aes.h"
#include "c/aes-gcm.h"
}

#include <collection.h>

namespace AES_GCM
{
	public ref class AesGcmWrapper sealed
	{
	public:
		AesGcmWrapper();
		int encrypt(Platform::WriteOnlyArray<uint8>^ output, const Platform::Array<uint8>^ input, const Platform::Array<uint8>^ key, const Platform::Array<uint8>^ iv);
		int decrypt(Platform::WriteOnlyArray<uint8>^ output, const Platform::Array<uint8>^ input, const Platform::Array<uint8>^ key, const Platform::Array<uint8>^ iv);
		void test();

	private:
		void winArrToCharArr(const Platform::Array<uint8>^ input, unsigned char* output);
		void charArrToWinArr(unsigned char* input, Platform::WriteOnlyArray<uint8>^ output);
	};
}

