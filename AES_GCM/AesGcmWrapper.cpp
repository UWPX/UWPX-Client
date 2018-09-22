#include "AesGcmWrapper.h"

using namespace AES_GCM;
using namespace Platform;

AesGcmWrapper::AesGcmWrapper() {
	gcm_initialize();
}

int AesGcmWrapper::encrypt(WriteOnlyArray<uint8>^ output, const Array<uint8>^ input, const Array<uint8>^ key, const Array<uint8>^ iv) {
	// Create temp arrays:
	unsigned char* _output = new unsigned char[output->Length];
	unsigned char* _input = new unsigned char[input->Length];
	unsigned char* _key = new unsigned char[key->Length];
	unsigned char* _iv = new unsigned char[iv->Length];

	// Copy data in temp arrays:
	winArrToCharArr(input, _input);
	winArrToCharArr(key, _key);
	winArrToCharArr(iv, _iv);

	// Call function:
	int ret = aes_gcm_encrypt(_output, const_cast<const unsigned char*>(_input), input->Length, const_cast<const unsigned char*>(_key), key->Length, const_cast<const unsigned char*>(_iv), iv->Length);

	// Copy output, cleanup and return:
	charArrToWinArr(_output, output);
	delete[] _output;
	delete[] _input;
	delete[] _key;
	delete[] _iv;
	return ret;
}

int AesGcmWrapper::decrypt(WriteOnlyArray<uint8>^ output, const Array<uint8>^ input, const Array<uint8>^ key, const Array<uint8>^ iv) {
	// Create temp arrays:
	unsigned char* _output = new unsigned char[output->Length];
	unsigned char* _input = new unsigned char[input->Length];
	unsigned char* _key = new unsigned char[key->Length];
	unsigned char* _iv = new unsigned char[iv->Length];

	// Copy data in temp arrays:
	winArrToCharArr(input, _input);
	winArrToCharArr(key, _key);
	winArrToCharArr(iv, _iv);

	// Call function:
	int ret = aes_gcm_decrypt(_output, const_cast<const unsigned char*>(_input), input->Length, const_cast<const unsigned char*>(_key), key->Length, const_cast<const unsigned char*>(_iv), iv->Length);

	// Copy output, cleanup and return:
	charArrToWinArr(_output, output);
	delete[] _output;
	delete[] _input;
	delete[] _key;
	delete[] _iv;
	return ret;
}

void AesGcmWrapper::winArrToCharArr(const Platform::Array<uint8>^ input, unsigned char* output) {
	for (int i = 0; i < input->Length; i++) {
		output[i] = input[i];
	}
}

void AesGcmWrapper::charArrToWinArr(unsigned char* input, Platform::WriteOnlyArray<uint8>^ output) {
	for (int i = 0; i < output->Length; i++) {
		output[i] = input[i];
	}
}

void AesGcmWrapper::test() {

}