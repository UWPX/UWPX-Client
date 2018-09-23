#pragma once

extern "C" {
#include "c\gcm.h"
}

#include <collection.h>

namespace AES_GCM
{
	public ref class AesGcmWrapper sealed
	{
	public:
		AesGcmWrapper();
		int encrypt(Platform::WriteOnlyArray<uint8>^ output, Platform::WriteOnlyArray<uint8>^ tag, const Platform::Array<uint8>^ input, const Platform::Array<uint8>^ key, const Platform::Array<uint8>^ iv);
		int decrypt(Platform::WriteOnlyArray<uint8>^ output, const Platform::Array<uint8>^ tag, const Platform::Array<uint8>^ input, const Platform::Array<uint8>^ key, const Platform::Array<uint8>^ iv);

		UINT calcEncryptSize(UINT inputSize);
		UINT calcDecryptSize(UINT inputSize);

		void test();

	private:
		void winArrToCharArr(const Platform::Array<uint8>^ input, UCHAR* output);
		void charArrToWinArr(UCHAR* input, Platform::WriteOnlyArray<uint8>^ output);
	};
}

