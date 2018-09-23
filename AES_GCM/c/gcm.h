/*
 *
 * Chinese Academy of Sciences
 * State Key Laboratory of Information Security, 
 * Institute of Information Engineering
 *
 * Copyright (C) 2016 Chinese Academy of Sciences
 *
 * LuoPeng, luopeng@iie.ac.cn
 * Updated in May 2016
 *
 */

#ifndef GCM_H
#define GCM_H

#include <stdint.h>
#include "aes.h"

#define GCM_BLOCK_SIZE  AES_BLOCK_SIZE       /* block size in bytes, AES 128-128 */
#define GCM_DEFAULT_IV_LEN (12)              /* default iv length in bytes */
#define GCM_FIELD_CONST (0xe100000000000000) /* the const value in filed */

typedef enum {
    OPERATION_FAIL = -1,
    OPERATION_SUC = 0,
} operation_result;

/*
 * basic functions of a block cipher
 */
typedef int (*block_key_schedule_p)(const uint8_t *key, uint8_t *roundkeys);
typedef int (*block_encrypt_p)(const uint8_t *roundkeys, const uint8_t *input, uint8_t *output);
typedef int (*block_decrypt_p)(const uint8_t *roundkeys, const uint8_t *input, uint8_t *output);

/*
 * block cipher context structure
 */
typedef struct {
    // rounds keys of block cipher
    uint8_t *rk;
    // block cipher encryption
    block_encrypt_p block_encrypt;
    uint8_t H[GCM_BLOCK_SIZE];
    uint8_t buff[GCM_BLOCK_SIZE];
    uint8_t T[GCM_BLOCK_SIZE][256][GCM_BLOCK_SIZE];
} gcm_context;

/**
 * @par purpose
 *    Initialize GCM context (just makes references valid)
 *    Makes the context ready for gcm_setkey() or
 *    gcm_free().
 */
void *gcm_init();

/**
 * @par purpose
 *    GCM initialization
 *
 * @par ctx      GCM context to be initialized
 * @par cipher   cipher to use
 * @par key      master keys
 * @par keybits  useless now
 *
 * @par return values
 *    0 if successful, or a cipher specific error code
 */
operation_result gcm_setkey( void *ctx,
        const unsigned char *key,
        unsigned int keybits );

/**
 * @par purpose
 *    Free a GCM context and underlying cipher sub-context
 *
 * @par ctx    GCM context to free
 */
void gcm_free( void *ctx );

/**
 * @par purpose
 * GCM authenticated encryption
 * The Galois/Counter Mode of Operation(GCM)
 * http://csrc.nist.gov/groups/ST/toolkit/BCM/documents/proposedmodes/gcm/gcm-spec.pdf
 *
 * @note     On encryption, the output buffer can be the same as the input buffer.
 *
 * @par ctx      GCM context
 * @par iv       an initialization vector IV, that can have any number of bits between 1 and 2^64.
 *               For a fixed value of the key, each IV value must be distinct, but need not have equal lengths.
 *               the value is (0^32||IV) if length of IV is 96.
 *     @note     iv can NOT be NULL.
 * @par iv_len   length of IV, the recommended length is 96-bit.
 *     @note     iv_len can NOT be 0.
 * @par add      additional authenticated data. It includes addresses, ports, sequence numbers, protocal version
 *               numbers, and other fields that indicate how the plaintext should be handled, forwarded and processed.
 *     @note     add can be NULL with add_len set to 0
 * @par add_len  add' length
 * @par input    buffer holding the input data
 *     @note     input can be NULL, therefore length is 0 and output is NULL
 * @par length   length of the input data
 * @par output   buffer for holding the output data
 * @par tag      an authentication tag, whose length can be any value between 0 and 64(not included)
 *     @note     tag can NOT be NULL
 * @par tag_len  length of the tag to generate
 *     @note     tag_len can NOT be 0
 *
 * @par return values
 *     OPERATION_SUC if successful
 *     else OPERATION_FAIL
 */
operation_result gcm_crypt_and_tag( void *ctx,
        const unsigned char *iv,
        size_t iv_len,
        const unsigned char *add,
        size_t add_len,
        const unsigned char *input,
        size_t length,
        unsigned char *output,
        unsigned char *tag,
        size_t tag_len);

/**
 * @par purpose
 * GCM authenticated decryption
 * The Galois/Counter Mode of Operation(GCM)
 * http://csrc.nist.gov/groups/ST/toolkit/BCM/documents/proposedmodes/gcm/gcm-spec.pdf
 *
 * @note      On decryption, the output buffer cannot be the same as input buffer.
 *            If buffers overlap, the output buffer must trail at least 8 bytes
 *            behind the input buffer
 *
 * @par ctx      GCM context
 * @par iv       an initialization vector IV, that can have any number of bits between 1 and 2^64.
 *               For a fixed value of the key, each IV value must be distinct, but need not have equal lengths.
 * @par iv_len   length of IV, the recommended length is 96-bit.
 * @par add      additional authenticated data. It includes addresses, ports, sequence numbers, protocal version
 *               numbers, and other fields that indicate how the plaintext should be handled, forwarded and processed.
 * @par add_len  ength of additional data
 * @par tag_len  length of the tag to generate
 * @par tag      an authentication tag, whose length can be any value between 0 and 64(not included)
 * @par input    buffer holding the input data
 * @par length   length of the input data
 * @par output   buffer for holding the output data
 *
 * @par return values
 *      OPERATION_SUC if successful and authenticated,
 *      OPERATION_FAIL if tag does not match or decryption fails.
 */
operation_result gcm_auth_decrypt( void *ctx,
        const unsigned char *iv,
        size_t iv_len,
        const unsigned char *add,
        size_t add_len,
        const unsigned char *tag,
        size_t tag_len,
        const unsigned char *input,
        size_t length,
        unsigned char *output );

#endif
