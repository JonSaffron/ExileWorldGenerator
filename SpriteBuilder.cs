﻿using System.Drawing;
using System.Linq;

namespace ExileMappedBackground
    {
    class SpriteBuilder
        {
        public Bitmap BuildSprite(byte sprite, GamePalette palette, bool flipHorizontally, bool flipVertically)
            {

            }



        private static byte[] BuildSpriteSheet()
            {
            var result = new byte[]
                {
                // sprite sheet is 128 x 81 pixels
                // There are 2 bits of colour per pixel, therefore 4 pixels to the byte.
                // Each byte has the four pixels interlaced in an abcdabcd format like mode 5.
                0xC0,0x00,0x00,0x00,0x32,0x11,0x80,0x10,0x00,0x00,0x00,0x20,0x06,0x08,0x00,0x00,0x01,0x8C,0x00,0x66,0x80,0x00,0x00,0x00,0x01,0x02,0x08,0x64,0x90,0x80,0x00,0x66,    // 000 
                0x8C,0x00,0x00,0x00,0x56,0xA3,0xC0,0xCA,0x00,0x00,0x00,0x07,0x2D,0x66,0x00,0x00,0x00,0x00,0x00,0x60,0xC0,0x00,0x00,0x00,0x01,0x0B,0x88,0x42,0xF0,0x3E,0x64,0x4C,    // 010    
                0x3C,0x00,0x00,0x00,0x03,0x01,0x19,0x68,0x00,0x00,0x13,0x21,0x2D,0x00,0x00,0x00,0x0B,0x8D,0x00,0x40,0x68,0x00,0x00,0x00,0x02,0xAB,0x00,0xCB,0xF8,0x18,0x90,0x4C,    // 020
                0xDF,0x00,0x00,0x00,0x46,0x23,0x00,0x1C,0x00,0x00,0x37,0x07,0x06,0x00,0x00,0x00,0xCA,0x35,0x04,0x42,0xFC,0x80,0x00,0x00,0x13,0x19,0x0A,0xE8,0x74,0x99,0x83,0x1E,    // 030
                0x3C,0xC0,0x00,0x00,0x46,0x23,0x00,0x0A,0x00,0x00,0x1F,0x21,0x00,0x00,0x00,0x00,0xCB,0xBD,0xC5,0x63,0x4F,0xC0,0x00,0x00,0x15,0x9F,0x0B,0xE2,0x33,0x00,0x87,0x78,    // 040
                0xFF,0x0C,0x00,0x00,0x07,0x83,0x59,0x0E,0x00,0x01,0x3F,0x07,0x00,0x00,0x03,0x0F,0x4B,0xAD,0xF5,0x7B,0x6F,0xE0,0x00,0x00,0x47,0xFF,0x26,0xC0,0x65,0x01,0x96,0xC0,    // 050
                0xC7,0xF4,0x00,0x00,0x04,0x02,0x11,0x08,0x00,0x03,0x1F,0x07,0x00,0x00,0x0F,0x0F,0x4A,0x25,0xB5,0xDB,0xFF,0xAC,0x00,0x00,0x4F,0xFF,0x4E,0x80,0x61,0x0B,0x1E,0x00,    // 060
                0xFF,0xEF,0x00,0x00,0x06,0x13,0x11,0x5C,0x00,0x21,0x3F,0x21,0x00,0x01,0x0F,0x0F,0x4B,0xAD,0xA5,0x4B,0x7A,0x9E,0x80,0x00,0x37,0xFB,0xCE,0x80,0xF8,0x01,0x96,0xE0,    // 070
                0x79,0x79,0xC0,0x00,0x06,0x13,0x11,0x1E,0x00,0x07,0x1F,0x21,0x00,0x03,0x1F,0x2F,0x4B,0xAD,0x85,0x43,0x6F,0x7C,0xC0,0x00,0x13,0xF9,0x8C,0x40,0xE0,0x00,0x87,0x3C,    // 080
                0x2F,0x3D,0xCC,0x00,0x26,0x03,0x00,0x8C,0x00,0x21,0x3F,0x07,0x00,0x03,0x0E,0x0D,0x4A,0x25,0x04,0x42,0xDF,0x4F,0xCA,0x00,0x13,0xF0,0x8C,0x10,0x80,0x11,0x87,0x1E,    // 090
                0xFF,0xFF,0x3C,0x00,0x04,0x03,0x28,0x24,0x00,0x07,0x3F,0x07,0x00,0x07,0x0F,0x0F,0x0B,0x8D,0x00,0x40,0xBD,0xEF,0x9E,0x00,0x0D,0xF0,0x8F,0x00,0x0C,0x10,0x80,0x84,    // 0a0
                0x96,0xC7,0xF7,0x00,0x04,0x06,0x68,0x30,0x00,0x21,0x1F,0x21,0x00,0x06,0x0B,0x0D,0x00,0x00,0x00,0x40,0x9F,0x3E,0x5E,0x80,0x15,0xF1,0xAE,0x00,0xCC,0x32,0x32,0x00,    // 0b0
                0xFF,0xFF,0xDE,0x80,0x40,0x40,0x40,0x10,0x00,0x07,0x3F,0x21,0x00,0x11,0x44,0x22,0x03,0x0E,0x00,0x42,0xFF,0xA7,0xFC,0x84,0x37,0xEB,0xCC,0x01,0x0E,0x00,0x12,0x08,    // 0c0
                0xB6,0x1E,0x8F,0x68,0x60,0x60,0x00,0x03,0x00,0x21,0x3F,0x07,0x01,0x0F,0x0F,0x0F,0x0E,0x0B,0x08,0x41,0xA7,0xBF,0x9F,0x68,0x6F,0xEF,0x0F,0x11,0xCA,0x00,0x25,0x0C,    // 0d0
                0xFF,0xFF,0xFF,0x8E,0x00,0x00,0x00,0xCA,0x00,0x07,0x1F,0x07,0x01,0x0F,0x0F,0x0F,0x88,0x88,0x88,0x62,0x6F,0x9F,0x3F,0xF8,0x07,0xBF,0x0D,0x03,0x2D,0x00,0x48,0x44,    // 0e0
                0x69,0xC7,0x78,0xBE,0x00,0x11,0x91,0x06,0x00,0x21,0x3F,0x07,0x00,0x80,0x00,0x90,0x00,0x12,0xC0,0x71,0xCF,0xFF,0xEF,0xFE,0x11,0x9B,0x88,0x33,0xED,0x00,0x4B,0x0E,    // 0f0
                0x6D,0x6F,0x3D,0x8F,0x00,0x23,0xC0,0x0C,0x00,0x21,0x3F,0x21,0x30,0xE8,0x30,0xB9,0x0C,0x25,0xE0,0x50,0x6F,0x8F,0xD6,0xAF,0x01,0x8A,0x08,0x07,0x0F,0x08,0x00,0x00,    // 100
                0xFF,0xFF,0xFF,0xFF,0x00,0x01,0x19,0x1A,0x11,0x07,0x1F,0x21,0x73,0xFC,0x73,0xBB,0xCE,0x25,0xC0,0x66,0xED,0xAF,0xCF,0xCB,0x00,0x0B,0x8C,0xFF,0x8F,0x1D,0xFF,0xFF,    // 110
                0x3C,0x9E,0xC7,0x3C,0x00,0x23,0x11,0x2A,0x13,0x07,0x3F,0x07,0x74,0xF2,0x74,0xB0,0xCE,0x00,0x31,0x06,0xCF,0xFF,0x9F,0xDF,0x01,0x09,0x00,0x07,0x0F,0x08,0xDC,0xE0,    // 120
                0xFF,0xFF,0xFF,0xFF,0x00,0x03,0x59,0x08,0x04,0x21,0x3F,0x07,0x64,0xB2,0x64,0x90,0xED,0xFF,0xE2,0xFF,0x7F,0xBF,0xDF,0x7F,0x22,0x00,0x88,0x33,0x6F,0x11,0xDD,0xFF,    // 130
                0xE3,0x3D,0x0F,0xE3,0x00,0x01,0x11,0x4C,0x17,0x21,0x1F,0x07,0x64,0x32,0x32,0x07,0xED,0xFF,0x80,0x06,0x7B,0x3E,0x7F,0x1F,0x74,0x11,0xC0,0x03,0x07,0x00,0xDC,0xE0,    // 140
                0xFF,0xFF,0xFF,0xFF,0x00,0x02,0x11,0x7E,0x0C,0x07,0x3F,0x07,0x64,0x77,0xB0,0x27,0xEB,0xFF,0xEE,0xFF,0x2F,0xE7,0x3F,0xBF,0x40,0x10,0x00,0x11,0x46,0x11,0xDD,0xFF,    // 150
                0x3C,0x8F,0x79,0x2D,0x00,0x07,0x00,0x3C,0x97,0x07,0x3F,0x21,0x64,0x72,0x80,0x05,0xEB,0x00,0x33,0x06,0x7F,0x3F,0x7E,0xEF,0x60,0x10,0x80,0x01,0x0E,0x00,0xCC,0x00,    // 160
                0xFF,0xFF,0xFF,0xFF,0x00,0x27,0x00,0x0C,0x0C,0x21,0x1F,0x21,0xFE,0x00,0x20,0x07,0xE7,0x25,0xC0,0x66,0xDF,0xFF,0xCF,0x4F,0x70,0x10,0xC0,0x00,0xCC,0x00,0x11,0xCC,    // 170
                0xCB,0x6B,0xD6,0xC7,0x08,0x0D,0x00,0x40,0x97,0x21,0x1F,0x07,0xF4,0x20,0x20,0x27,0x6F,0x25,0xE0,0x05,0x9E,0xCF,0xC7,0x6D,0xDC,0x33,0x40,0x11,0x00,0x00,0xD1,0x88,    // 180
                0x87,0xE7,0x1E,0xDE,0x18,0x81,0x00,0x60,0x0C,0x07,0x3F,0x07,0x00,0x32,0x64,0x05,0x69,0x00,0x00,0x05,0x8F,0x4F,0x5F,0xEF,0x8E,0x23,0x08,0xC1,0x60,0x00,0x00,0x00,    // 190
                0xFF,0xFF,0xFF,0xFF,0x98,0x10,0x00,0x00,0x0C,0x07,0x3F,0x07,0x64,0x32,0x32,0x06,0x6F,0x88,0x40,0x20,0xDF,0xFE,0xFF,0xB7,0x0D,0x03,0x04,0x61,0xC0,0x8E,0x30,0x80,    // 1a0
                0xA7,0x1E,0xC7,0x4B,0x90,0x10,0x80,0x00,0x97,0x07,0x1F,0x07,0x64,0x77,0xB0,0x00,0x6F,0x08,0x90,0x22,0xF7,0x3F,0xEF,0x3F,0x8C,0x23,0x04,0x01,0x11,0x4A,0x07,0x0C,    // 1b0
                0xFF,0xFF,0xFF,0xFF,0x88,0x22,0x00,0x00,0x1F,0x21,0x1F,0x21,0x64,0x72,0x80,0x20,0x6F,0x98,0xB0,0x64,0x5F,0xB7,0x2F,0xEF,0x8F,0xAB,0x08,0x11,0x11,0xED,0xF0,0xF7,    // 1c0
                0x9E,0xC7,0x79,0x1E,0x08,0x74,0x00,0x88,0x0C,0x21,0x3F,0x21,0xE0,0x00,0x30,0xE2,0x6F,0x78,0x70,0xBC,0x5F,0xAF,0x7B,0x8F,0x8B,0xAB,0x2E,0x9F,0x01,0xBD,0x00,0x00,    // 1d0
                0xFF,0xFF,0xFF,0xFF,0x88,0x40,0x11,0xC0,0x84,0x07,0x3F,0x07,0xE8,0x20,0x73,0xEE,0x6F,0x7C,0xA0,0x06,0xCF,0xFF,0x6F,0xED,0xCC,0x22,0x3F,0x99,0x99,0x2F,0x73,0xEE,    // 1e0
                0x7B,0x5A,0x8F,0xC7,0x80,0x60,0x10,0x00,0x1F,0x07,0x1F,0x07,0xC0,0x3A,0x30,0xE0,0x6F,0x3C,0x04,0x9C,0x8F,0x7D,0xCF,0xF7,0x37,0x11,0x11,0xFF,0x99,0x0F,0x30,0xC4,    // 1f0
                0xE3,0x0F,0x1E,0xE7,0x08,0x70,0x10,0x80,0x00,0x11,0xEE,0x0F,0x01,0x3A,0x04,0x20,0x6F,0xD8,0xA0,0x0E,0xFF,0xC7,0xEF,0x5F,0x07,0x09,0xDD,0x99,0x88,0x9F,0x00,0x00,    // 200
                0xFF,0xFF,0xFF,0xFF,0x88,0xDC,0x10,0xC0,0x00,0x11,0xEE,0x0F,0x1B,0x3A,0x06,0x0E,0x6F,0x1C,0xC0,0x04,0xD7,0x6F,0x7F,0x4F,0x07,0x0D,0x0C,0x9F,0x00,0xEE,0x73,0xEE,    // 210
                0x3C,0x9E,0xE3,0x1E,0x0C,0x8E,0x33,0x40,0x00,0x01,0x0E,0x44,0x0A,0x3A,0x04,0x0E,0x6F,0xBC,0x80,0x2A,0x1F,0xFF,0xFD,0xCF,0x07,0x0D,0x08,0x00,0x00,0x5C,0x30,0xCC,    // 220
                0xFF,0xFF,0xFF,0xFF,0xCC,0x0D,0x23,0x08,0x00,0x11,0xEE,0x0B,0x0B,0x3A,0x02,0x4E,0x6F,0xE8,0x80,0x9F,0xBF,0xBD,0x9F,0xFF,0x0E,0x81,0x08,0x00,0x00,0x2C,0x30,0xC4,    // 230
                0x4F,0x79,0x0F,0xE3,0xC0,0x8D,0x03,0x04,0x00,0x05,0x0E,0x0F,0x1B,0x3A,0x06,0x0A,0x6F,0x5F,0x48,0x03,0xEF,0x0F,0x9F,0x7B,0x0C,0xC5,0x08,0x00,0x00,0x6E,0x30,0xCC,    // 240
                0xFF,0xFF,0xFF,0xFF,0x8C,0x8E,0x23,0x00,0x00,0x04,0x00,0x0F,0x0A,0x2A,0x0E,0x0E,0x6F,0x1F,0x0C,0x06,0x7B,0x2F,0xFF,0x2F,0x80,0xC1,0x00,0x00,0x00,0x2C,0x00,0x00,    // 250
                0x3D,0xBC,0x79,0x0F,0x88,0x8F,0xAB,0x2E,0x00,0x1D,0xEE,0x78,0x09,0x09,0x4E,0x4E,0x6F,0xEF,0x4E,0x40,0x1F,0xFF,0xEB,0x2F,0xC4,0x10,0x00,0x00,0x00,0x5C,0x33,0xCC,    // 260
                0x3C,0x8F,0x7D,0x3C,0xC8,0x8B,0xAA,0x2E,0x00,0x0D,0x0E,0x08,0x0C,0x03,0x0A,0x0A,0x6F,0x8F,0xDF,0x60,0xFF,0xBD,0x3F,0x6F,0xC0,0x10,0x88,0x00,0x00,0x00,0x30,0xCC,    // 270
                0xFF,0xFF,0xFF,0xFF,0xCC,0x44,0x33,0x00,0x01,0x08,0x00,0x0F,0x0E,0x07,0x0E,0x0E,0xF0,0xF0,0xF0,0xC0,0xBD,0x1F,0x1F,0xFF,0x00,0x10,0x80,0x00,0x00,0x00,0x30,0xCC,    // 280
                0xE3,0x1E,0x8F,0xE3,0x48,0x37,0x01,0xCC,0x01,0x5D,0xEE,0x78,0x2F,0x27,0x4E,0x4E,0x6F,0xFF,0xEF,0xEC,0x9F,0x7F,0xDF,0x3D,0x00,0x88,0x00,0x00,0x00,0x00,0x30,0xCC,    // 290
                0xFF,0xFF,0xFF,0xFF,0x88,0x07,0x01,0x0E,0x03,0x01,0x0E,0x08,0x0D,0x05,0x0A,0x0A,0x6F,0xFF,0xEF,0xEC,0xDF,0xDE,0xFF,0x8F,0x22,0x91,0x40,0x40,0x00,0x00,0x30,0xC4,    // 2a0
                0x0F,0xC7,0x79,0x2D,0xC4,0x03,0x01,0x0E,0x03,0xC4,0x01,0x0F,0x0F,0x07,0x0E,0x0E,0x6F,0xFF,0xEF,0xDE,0x7F,0x4F,0xCB,0xEF,0x20,0x54,0x40,0x41,0x00,0x00,0x30,0xCC,    // 2b0
                0xFF,0xFF,0xFF,0xFF,0xCE,0xCB,0x63,0x0E,0x06,0x00,0x03,0xF0,0x0A,0x02,0x0A,0x0A,0x6F,0xFF,0xEF,0xDE,0x6F,0x6F,0x8F,0xBF,0x45,0x42,0x61,0xC3,0xAC,0x85,0x30,0xCC,    // 2c0
                0x6B,0x79,0x0F,0xC7,0x4A,0xC3,0x61,0x20,0x16,0xD5,0x8F,0x00,0x55,0x55,0x55,0x55,0x6F,0xFF,0xEF,0xBE,0xFF,0xA7,0xFF,0xBD,0x61,0x00,0xA9,0x81,0x7F,0xCE,0x30,0xCC,    // 2d0
                0x7F,0xFF,0xFF,0xFF,0xCC,0x90,0x40,0x31,0x0E,0x01,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x6F,0xFF,0xEF,0xBE,0x3D,0xBF,0xDF,0x9F,0x20,0xB8,0x01,0x03,0x88,0x07,0x30,0xCC,    // 2e0
                0xCF,0xC7,0xB5,0x3C,0xC6,0x10,0x88,0x30,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0x0F,0xF0,0xF0,0xF0,0x7E,0x8F,0xEF,0x4F,0xFF,0x89,0xA4,0x99,0x00,0xCC,0xCE,0x30,0xC4,    // 2f0
                0xF7,0xFF,0xFF,0xCE,0x00,0x10,0x80,0x00,0x0F,0x0F,0x0F,0x0F,0x0F,0x07,0x0E,0x0F,0x00,0x30,0x00,0x02,0xF0,0xF2,0xF3,0xF4,0xA0,0x90,0x80,0x00,0x77,0xCD,0x30,0xCC,    // 300
                0xB5,0x0F,0xE3,0x68,0x00,0x00,0x00,0x10,0x0F,0x0F,0x0F,0x0F,0x0F,0x07,0x19,0x1E,0x00,0x31,0x00,0x55,0x60,0xF0,0x70,0xE0,0x51,0x40,0x11,0x22,0x0F,0x0F,0x30,0xCC,    // 310
                0x79,0xAD,0x7B,0x0C,0x00,0x00,0x00,0x10,0x0C,0x03,0x00,0x00,0x0E,0x02,0x47,0x69,0x00,0x20,0x11,0x20,0x40,0xA0,0x20,0x40,0x12,0x39,0x32,0x31,0xF0,0x6F,0x30,0xC4,    // 320
                0xFF,0xFF,0x8F,0xC0,0x00,0x00,0x00,0x30,0x3C,0xC3,0xF0,0xF0,0x19,0x55,0x1E,0x83,0x00,0x02,0x00,0xE0,0x00,0x2B,0x04,0x00,0x22,0xC6,0x31,0x3E,0x00,0xF6,0x30,0xCC,    // 330
                0x2F,0x6B,0xDF,0x88,0x00,0x00,0x00,0x21,0x0F,0x0F,0x0F,0x0F,0x47,0x0F,0x69,0x4F,0x00,0x06,0x01,0x51,0x00,0x22,0x09,0x44,0x10,0x50,0x33,0x3F,0x06,0x6F,0x30,0xCC,    // 340
                0xEF,0x6B,0x5A,0x80,0x00,0x00,0x00,0x61,0x00,0x06,0x00,0x00,0x0F,0x1E,0x87,0x0F,0x00,0x0E,0xC0,0xC0,0x11,0x7F,0x26,0x44,0x00,0x28,0x33,0x33,0x60,0x0F,0x30,0xCC,    // 350
                0x7F,0xFF,0xCF,0x00,0x00,0x00,0x00,0x43,0xF0,0x96,0xF0,0xF0,0x0F,0x69,0x0F,0x0E,0x01,0x0D,0xEA,0x82,0x15,0x55,0x67,0x4C,0x10,0x0C,0x11,0x22,0x00,0x44,0x30,0xC4,    // 360
                0x96,0xC7,0x78,0x00,0x00,0x00,0x00,0x43,0x0F,0x0F,0x0F,0x0F,0x1E,0x83,0x0F,0x1B,0x67,0x0B,0x50,0xC0,0x33,0xDF,0xEF,0xEE,0x00,0x08,0x00,0x00,0x0F,0xCD,0x30,0xCC,    // 370
                0xFF,0xFF,0xCA,0x00,0x00,0x00,0x00,0xA5,0x00,0x0C,0x00,0x03,0x69,0x4F,0x0E,0x4D,0x46,0x00,0x10,0x22,0xAA,0x9D,0xAA,0xAA,0x00,0x08,0x55,0x00,0x09,0xF0,0x30,0xCC,    // 380
                0x2D,0x3C,0xE8,0x00,0x00,0x00,0x10,0x87,0xF0,0x3C,0xF0,0xC3,0x87,0x0F,0x1B,0x07,0x4D,0x2E,0x10,0x80,0xEF,0x17,0x23,0xBF,0x65,0x11,0x60,0x00,0x69,0x49,0x30,0xCC,    // 390
                0xEF,0x1E,0x0C,0x00,0x00,0x00,0x10,0xD7,0x0F,0x0F,0x0F,0x0F,0x0F,0x0E,0x4D,0x0C,0x8F,0x2E,0x30,0x44,0x46,0x0E,0x37,0x99,0x33,0x8A,0xB0,0x88,0x69,0x6E,0x00,0x00,    // 3a0
                0x7B,0xCF,0xC0,0x00,0x00,0x00,0x30,0x7F,0x0F,0x0F,0x0F,0x0F,0x0F,0x1B,0x07,0x00,0x8F,0x00,0x20,0x00,0x0F,0x07,0x1B,0x06,0x55,0x9D,0x55,0x02,0x0F,0x08,0x70,0xEE,    // 3b0
                0xEF,0x6F,0x88,0x00,0x00,0x00,0x30,0x7B,0x0A,0x0D,0x0A,0x0D,0x0E,0x4D,0x0C,0x00,0xCE,0x00,0x00,0x44,0x0B,0x06,0x1F,0x0B,0x66,0x0A,0xFA,0x27,0x09,0x01,0x00,0xAC,    // 3c0
                0x2D,0x5E,0x80,0x00,0x00,0x00,0x21,0x2F,0x55,0x22,0x55,0x22,0x1B,0x07,0x00,0x00,0x20,0x00,0x00,0x22,0x30,0x01,0x08,0x11,0x33,0x8C,0xA0,0xFD,0x09,0x21,0x91,0x3D,    // 3d0
                0xF3,0xEF,0x00,0x00,0x00,0x00,0x53,0xA7,0x0F,0x0F,0x0F,0x0F,0x4D,0x0C,0x00,0x00,0xE0,0x00,0x00,0x22,0x43,0x0B,0x0C,0x32,0x80,0x19,0x50,0x27,0x69,0x25,0x91,0x0F,    // 3e0
                0xEF,0x3C,0x00,0x00,0x00,0x00,0x53,0xAF,0x0F,0x0F,0x0F,0x0F,0x07,0x00,0x00,0x00,0xC0,0x00,0x00,0x01,0x04,0x82,0x44,0x20,0x00,0x04,0x55,0x02,0x6F,0x3D,0x80,0x0E,    // 3f0
                0xB4,0x2E,0x00,0x00,0x00,0x00,0xA7,0xEF,0x05,0x0F,0x0F,0x0A,0x0C,0x00,0x00,0x00,0x80,0x00,0x00,0x88,0x06,0x03,0x00,0x30,0x00,0x24,0x24,0x24,0x6F,0x2C,0x91,0x8C,    // 400
                0xFF,0x68,0x00,0x00,0x00,0x10,0xA5,0x7F,0x84,0xAA,0x55,0x12,0x08,0x10,0x4C,0x00,0xE8,0x00,0x11,0xC0,0x07,0x0B,0x0C,0x30,0x80,0x24,0xFF,0x24,0x69,0x37,0x3A,0x0C,    // 410
                0x6B,0x4C,0x00,0x00,0x00,0x01,0x0F,0xFF,0x84,0x00,0x00,0x12,0x08,0x32,0x2E,0x00,0x44,0x00,0x10,0x00,0x00,0x00,0x00,0x66,0x80,0x00,0xF6,0x24,0x09,0x33,0x47,0x07,    // 420
                0xFF,0xC0,0x00,0x00,0x00,0x10,0x6F,0x8F,0x85,0x0F,0x0F,0x1A,0x2E,0x03,0x2E,0x00,0x00,0x00,0x10,0x80,0x71,0x9A,0x0C,0x47,0x00,0xFB,0x64,0x99,0x09,0x22,0x47,0x09,    // 430
                0xA5,0x08,0x00,0x00,0x00,0x30,0x7F,0xDF,0x87,0x0B,0x0D,0x1E,0x17,0x01,0x0C,0x00,0x00,0x00,0x10,0xC0,0xA7,0x12,0x28,0x06,0x19,0xFD,0x80,0xF6,0x0F,0x02,0x47,0x2E,    // 440
                0x5E,0x80,0x00,0x00,0x00,0x30,0x7B,0xF5,0x83,0x49,0x29,0x1C,0x21,0x88,0x6E,0xC1,0x00,0x00,0x33,0x40,0xAA,0x52,0x70,0x46,0x18,0xFF,0x88,0x64,0x0F,0x22,0x23,0xA6,    // 450
                0x9F,0x00,0x00,0x00,0x00,0x53,0x3F,0x7F,0xC9,0x6C,0x63,0x39,0x47,0x88,0x6A,0xAC,0x08,0x00,0x27,0x00,0xFF,0x12,0xC0,0x47,0x00,0xFA,0x00,0x00,0x09,0x22,0x11,0x0C,    // 460
                0x78,0x00,0x00,0x00,0x00,0xC3,0x2F,0x4F,0x64,0x3F,0xCF,0x62,0xDF,0x03,0x1F,0xBA,0x04,0x00,0xCF,0x09,0xAF,0x9A,0xD0,0x47,0x4C,0x66,0x00,0x00,0x69,0x22,0x00,0x0A,    // 470
                0xCE,0x00,0x00,0x00,0x00,0xD3,0x6F,0xE7,0x33,0x04,0x02,0xCC,0x8E,0x02,0x1F,0x9F,0x82,0x00,0x8F,0x09,0xAA,0xCE,0x60,0x45,0x4C,0x00,0x00,0x44,0x69,0x33,0x00,0x0E,    // 480
                0x68,0x00,0x00,0x00,0x00,0x97,0x7F,0x3F,0xF0,0x0F,0x0F,0x0F,0x8F,0x04,0x17,0x99,0xC8,0x00,0x4F,0x19,0xFF,0xCF,0x08,0x22,0x00,0x00,0x00,0xE8,0x0F,0x08,0x01,0x0F,    // 490
                0x8C,0x00,0x00,0x00,0x10,0x0F,0xFF,0xFF,0xC3,0x87,0x0F,0x0F,0x8F,0x1D,0x2E,0x8F,0xEC,0x01,0x4E,0x1D,0xAF,0x8D,0x0C,0x13,0x88,0x00,0xDF,0x80,0xF0,0x0F,0x0F,0xF0,    // 4a0
                0x48,0x00,0x00,0x00,0x10,0x7A,0x6D,0xBF,0xF0,0x0F,0x0F,0x0F,0x65,0x23,0xE6,0x88,0xEE,0xCB,0x2E,0x2E,0xAA,0xEE,0x00,0x03,0x08,0x02,0xFF,0xC0,0x00,0x1C,0x83,0x00,    // 4b0
                0x88,0x00,0x00,0x00,0x30,0x3F,0x4F,0xAF,0x77,0xFF,0xFF,0xEE,0x23,0x23,0x08,0xF0,0xF0,0xC3,0x0C,0x26,0x77,0xBB,0xCC,0xC5,0x1D,0x85,0x4F,0xE0,0x06,0x1D,0x8B,0x06,    // 4c0
                0x80,0x00,0x00,0x00,0x61,0xFF,0xDF,0xEF,0x70,0xA5,0x0F,0x0E,0x03,0x19,0x0C,0xFF,0xFF,0x83,0x00,0x01,0x00,0x00,0x00,0x80,0x0C,0x03,0x4F,0x28,0x60,0x1D,0x8B,0x60,    // 4d0
                0xFF,0xFF,0x00,0x04,0x52,0xCF,0xFF,0x3F,0x70,0xC3,0x0F,0x0E,0x47,0x0C,0x04,0x00,0x00,0x44,0x08,0x01,0x70,0x91,0x08,0xE3,0x1D,0x87,0x4F,0x0C,0x00,0x1C,0x83,0x00,    // 4e0
                0xFF,0xFF,0x2D,0x2A,0xD3,0xEF,0xCF,0xF7,0x70,0x2D,0x0F,0x0E,0x47,0xC4,0x0C,0x31,0x00,0x61,0x08,0x00,0x87,0x59,0x0C,0x61,0x18,0x87,0x46,0x1F,0x0F,0x0F,0x0F,0x0F,    // 4f0
                0x00,0x00,0x22,0x11,0x97,0x2F,0x6F,0xCF,0x70,0x87,0x0F,0x0E,0x23,0x80,0x0C,0x31,0x00,0x41,0x00,0x00,0x0F,0x1D,0xCC,0x40,0x10,0x06,0x00,0x17,0xF0,0x80,0x10,0xF0     // 500
                };
            return result;
            }

        private static byte[] BuildSpriteOffsetA()
            {
            var result = new byte[]
                {
                // format is: b1 b0 p1 p0 0 b4 b3 b2
                // where b is the byte offset from the start of the row (32 bytes per row), and p is the pixel offset within the byte (4 pixels per byte)
                0x36,0x44,0xC5,0x04,0x66,0x06,0x91,0x41,0x60,0x43,0xD0,0xD4,0xE4,0xE4,0xA4,0x03,    // 00
                0x97,0x63,0x03,0x05,0x77,0x97,0x65,0x06,0x06,0x06,0x06,0xE6,0xC6,0xD6,0xE6,0xF6,    // 10
                0x37,0xD6,0x65,0x02,0x03,0x12,0xC2,0x03,0x03,0x03,0x03,0x02,0x02,0x04,0x96,0xC4,    // 20
                0x04,0x04,0x43,0x43,0x00,0x00,0x05,0x05,0x01,0x00,0x00,0x00,0x00,0x05,0x05,0x05,    // 30
                0x05,0x00,0xC0,0x00,0x00,0x44,0xC0,0x03,0x03,0x03,0x07,0x07,0x53,0x17,0x02,0xD4,    // 40
                0x80,0xD4,0x07,0xF6,0xC6,0x87,0x04,0x84,0x47,0x67,0xC6,0xC6,0x96,0x02,0x44,0xD4,    // 50
                0xC3,0x67,0x36,0x86,0x01,0x51,0x31,0xB1,0xB1,0xC4,0x02,0x67,0x47,0x15,0x55,0x35,    // 60
                0x05,0x00,0x16,0x37,0xE6,0x76,0x04,0xB6,0xC4,0x47,0x84,0xB6,0xA4                    // 70
                };
            return result;
            }

        private static byte[] BuildSpriteOffsetB()
            {
            var result = new byte[]
                {
                // format is: r4 r3 r2 r1 r0 0 r6 r5
                // where r is the row within the sprite sheet
                0x42,0x02,0xE9,0x81,0x98,0x98,0xE0,0xE0,0x7A,0x00,0x72,0xE1,0x5A,0x3A,0x81,0x00,    // 00
                0xE1,0x0A,0x02,0xE9,0x60,0x68,0xE9,0x00,0x00,0x49,0x49,0x50,0x68,0x68,0x58,0x50,    // 10
                0xD0,0xD9,0xA0,0x00,0x28,0x01,0x01,0x09,0x49,0x89,0xC9,0x01,0x81,0x41,0x81,0x41,    // 20
                0x80,0x41,0x80,0x20,0x00,0x00,0x00,0x00,0x89,0x80,0x80,0x80,0x31,0x80,0x09,0x89,    // 30
                0x49,0x80,0x80,0x71,0x01,0xD0,0x00,0x80,0x01,0x41,0x4A,0x89,0xA8,0xE9,0xF9,0xC0,    // 40
                0x72,0xC0,0x00,0x20,0x00,0xE0,0x00,0x18,0x11,0xE1,0xC0,0xC8,0x51,0xB1,0x78,0x00,    // 50
                0x2A,0x00,0x02,0x02,0x00,0x00,0x70,0x00,0x60,0xE0,0x4A,0x0A,0xB1,0x99,0x99,0x99,    // 60
                0x99,0x72,0xC9,0x61,0x59,0xC1,0xD0,0x11,0x88,0x89,0x90,0xE8,0x90                    // 70
                };
            return result;
            }

        private static bool[] BuildFlipSpriteHorizontally()
            {
            return BuildSpriteWidthLookup().Select(w => (w & 0x1) != 0).ToArray();
            }

        private static int[] BuildSpriteWidthTable()
            {
            return BuildSpriteWidthLookup().Select(w => w >> 4).ToArray();
            }

        private static byte[] BuildSpriteWidthLookup()
            {
            var result = new byte[]
                {
                // Format: wwww000h
                // wwww + 1 is the width of the sprite in pixels
                // h indicates a horizontal flip is required
                // 0    1    2    3    4    5    6    7    8    9    a    b    c    d    e    f
                0xC0,0xA0,0x50,0x90,0x40,0x50,0x60,0x40,0x21,0x20,0x20,0x21,0x11,0x11,0x50,0x30, // 00
                0x60,0x51,0x50,0x50,0x80,0x50,0x50,0xB0,0xB0,0x80,0x80,0x50,0x90,0x70,0x50,0x30, // 10
                0x40,0x20,0x10,0xF1,0xF1,0x71,0x30,0xF0,0xF0,0xF0,0xF0,0xF1,0xF0,0x30,0x60,0x30, // 20
                0x30,0xF0,0xF0,0xF1,0xF0,0xF0,0xF0,0xF0,0xF1,0xF0,0xF0,0xF0,0xF0,0xF0,0xF0,0xF0, // 30
                0xF0,0x30,0x60,0xF0,0xF0,0x70,0x00,0xF0,0xF0,0xF0,0xF0,0x30,0x31,0x41,0xF0,0x20, // 40
                0x40,0x20,0x50,0x50,0x30,0x70,0xC0,0x40,0x40,0x20,0x60,0x60,0x40,0xF0,0x70,0x20, // 50
                0x70,0x90,0xC0,0x30,0x40,0x50,0x60,0x40,0x40,0x30,0xF0,0x20,0x30,0x31,0x70,0xB1, // 60
                0xF0,0x70,0x51,0x41,0x50,0x51,0x30,0x80,0x30,0x30,0x50,0x50,0x40                 // 70
                };
            return result;
            }

        private static bool[] BuildFlipSpriteVertically()
            {
            return BuildSpriteHeightLookup().Select(h => (h & 0x1) != 0).ToArray();
            }

        private static int[] BuildSpriteHeightTable()
            {
            return BuildSpriteHeightLookup().Select(h => h >> 3).ToArray();
            }

        private static byte[] BuildSpriteHeightLookup()
            {
            var result = new byte[]
                {
                // hhhhh00v
                // hhhhh + 1 is the sprite height in pixels
                // if v=1, the vertical flip flag is inverted when plotting the sprite
                //        0   1   2   3   4   5   6   7   8   9   a   b   c   d   e   f
                0x40,0x80,0x98,0x91,0xA8,0xA0,0xA0,0xA0,0x09,0x08,0x10,0x19,0x19,0x18,0x58,0x18, // 00
                0x60,0x78,0x81,0x98,0x78,0x70,0x98,0x90,0x28,0x48,0x78,0x69,0x20,0x28,0x38,0x48, // 10
                0x38,0x20,0x08,0xF8,0xF8,0x68,0xF9,0xF9,0xB9,0x79,0x39,0xF8,0x78,0x38,0x38,0x38, // 20
                0xF8,0x38,0x78,0xF8,0xF8,0x78,0xF8,0x78,0xF8,0xF8,0xC9,0x78,0x38,0xF8,0x89,0x09, // 30
                0x49,0xF8,0xF8,0xF9,0xF9,0x68,0x00,0xF8,0x78,0x38,0x39,0xF9,0xF9,0x28,0x48,0x18, // 40
                0x11,0x19,0x20,0x29,0x41,0xF8,0x70,0x30,0x20,0x20,0x19,0x18,0x28,0x60,0x48,0x80, // 50
                0x58,0x58,0x39,0x19,0x68,0x68,0x68,0x58,0x68,0x58,0x38,0x38,0x28,0x48,0x48,0x48, // 60
                0x48,0x08,0x30,0x20,0x28,0x39,0x70,0x38,0x30,0x20,0x20,0x20,0x20                 // 70
                };
            return result;
            }
        }
    }
