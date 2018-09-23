#include "AesGcmWrapper.h"

using namespace AES_GCM;
using namespace Platform;

AesGcmWrapper::AesGcmWrapper() { }

int AesGcmWrapper::encrypt(WriteOnlyArray<uint8>^ output, WriteOnlyArray<uint8>^ tag, const Array<uint8>^ input, const Array<uint8>^ key, const Array<uint8>^ iv) {
	// Create temp arrays:
	UCHAR* _output = new UCHAR[output->Length];
	UCHAR* _input = new UCHAR[input->Length];
	UCHAR* _key = new UCHAR[key->Length];
	UCHAR* _iv = new UCHAR[iv->Length];
	UCHAR* _tag = new UCHAR[tag->Length];

	// Copy data in temp arrays:
	winArrToCharArr(input, _input);
	winArrToCharArr(key, _key);
	winArrToCharArr(iv, _iv);

	// Call lib:
	int ret = 1;
	void* context = gcm_init();
	if (!context) {
		printf("Failed to create encrypt context for AES 128 GCM!");
		ret = -1;
	}
	else
	{
		operation_result flag = gcm_setkey(context, const_cast<const UCHAR*>(_key), key->Length * 8);
		if (flag == OPERATION_FAIL) {
			printf("Failed to set key for encrypt AES 128 GCM!");
			ret = -2;
		}
		else
		{
			flag = gcm_crypt_and_tag(context, const_cast<const UCHAR*>(_iv), iv->Length, NULL, 0, const_cast<UCHAR*>(_input), input->Length, _output, _tag, tag->Length);
			if (flag == OPERATION_FAIL) {
				printf("Failed to encrypt AES 128 GCM!");
				ret = -3;
			}
		}
		gcm_free(context);
	}

	// Copy output, cleanup and return:
	charArrToWinArr(_output, output);
	charArrToWinArr(_tag, tag);
	delete[] _output;
	delete[] _input;
	delete[] _key;
	delete[] _iv;
	delete[] _tag;
	return ret;
}

int AesGcmWrapper::decrypt(WriteOnlyArray<uint8>^ output, const Array<uint8>^ tag, const Array<uint8>^ input, const Array<uint8>^ key, const Array<uint8>^ iv) {
	// Create temp arrays:
	UCHAR* _output = new UCHAR[output->Length];
	UCHAR* _input = new UCHAR[input->Length];
	UCHAR* _key = new UCHAR[key->Length];
	UCHAR* _iv = new UCHAR[iv->Length];
	UCHAR* _tag = new UCHAR[tag->Length];

	// Copy data in temp arrays:
	winArrToCharArr(tag, _tag);
	winArrToCharArr(input, _input);
	winArrToCharArr(key, _key);
	winArrToCharArr(iv, _iv);

	// Call function:
	int ret = 1;
	void* context = gcm_init();
	if (!context) {
		printf("Failed to create decrypt context for AES 128 GCM!");
		ret = -1;
	}
	else
	{
		operation_result flag = gcm_setkey(context, const_cast<const UCHAR*>(_key), key->Length * 8);
		if (flag == OPERATION_FAIL) {
			printf("Failed to set key for decrypt AES 128 GCM!");
			ret = -2;
		}
		flag = gcm_auth_decrypt(context, const_cast<const UCHAR*>(_iv), iv->Length, NULL, 0, const_cast<UCHAR*>(_tag), tag->Length, const_cast<UCHAR*>(_input), input->Length, _output);
		if (flag == OPERATION_FAIL) {
			printf("Failed to decrypt AES 128 GCM!");
			ret = -3;
		}
		// gcm_free(context);
	}

	// Copy output, cleanup and return:
	charArrToWinArr(_output, output);
	delete[] _output;
	delete[] _input;
	delete[] _key;
	delete[] _iv;
	return ret;
}

void AesGcmWrapper::winArrToCharArr(const Platform::Array<uint8>^ input, UCHAR* output) {
	for (UINT i = 0; i < input->Length; i++) {
		output[i] = input[i];
	}
}

void AesGcmWrapper::charArrToWinArr(UCHAR* input, Platform::WriteOnlyArray<uint8>^ output) {
	for (UINT i = 0; i < output->Length; i++) {
		output[i] = input[i];
	}
}

UINT AesGcmWrapper::calcEncryptSize(UINT inputSize) {
	return inputSize;
}

UINT AesGcmWrapper::calcDecryptSize(UINT inputSize) {
	return inputSize;
}

void AesGcmWrapper::test() {

}