﻿using GBSharp.MemorySpace;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace GBSharp.VideoSpace
{
  internal static class DisplayFunctions
  {

    internal static BitmapData 
    LockBitmap(Bitmap bmp, ImageLockMode lockMode, PixelFormat pixelFormat)
    {
      BitmapData result = bmp.LockBits(
        new Rectangle(0, 0, bmp.Width, bmp.Height),
        lockMode, pixelFormat);
      return result;
    }

    /// <summary>
    /// Retreives a the contents on a tile depending on the coordinates and the accessing methods.
    /// </summary>
    /// <param name="tileX">The x coord for the tile</param>
    /// <param name="tileY">The y coord for the tile</param>
    /// <param name="LCDBit3">
    /// Whether the LCDC Register (0xFF40) Bit 3 is enabled.
    /// Determines what tilemap (where the tile indexes are) is used:
    /// 0: 0x9800 - 0x9BFF
    /// 1: 0x9C00 - 0x9FFF
    /// </param>
    /// <param name="LCDBit4">
    /// Whether the LCDC Register (0xFF40) Bit 3 is enabled.
    /// Determines the base address for the actual tiles and the
    /// accessing method (interpretation of the byte tile index retreived from the tilemap).
    /// 0: 0x8800 - 0x97FF | signed access
    /// 1: 0x8000 - 0x8FFF | unsigned access
    /// </param>
    /// <param name="wrap">Whether the x, y tile coordinates should wrap or be clipped</param>
    /// <returns>A byte[] with the 16 bytes that create a tile</returns>
    internal static byte[] 
    GetTileData(DisplayDefinition disDef, Memory memory, 
                int tileX, int tileY, 
                bool LCDCBit2, bool LCDBit3, bool LCDBit4, 
                bool wrap)
    {

      if(wrap)
      {
        tileX %= disDef.frameTileCountX;
        tileY %= disDef.screenTileCountY;
      }
      else
      {
        // TODO(Cristian): See if clipping is what we want
        if(tileX >= disDef.frameTileCountX) { tileX = disDef.frameTileCountX - 1; }
        if(tileY >= disDef.screenTileCountY) { tileY = disDef.screenTileCountY - 1; }
      }

      ushort tileMapBaseAddress = GetTileMapBaseAddress(LCDBit3);
      ushort tileBaseAddress = GetTileBaseAddress(LCDBit4);
      int tileOffset = GetTileOffset(disDef, memory, tileMapBaseAddress, LCDBit4, tileX, tileY);

      // We obtain the correct tile index
      byte[] result = GetTileData(disDef, memory, tileBaseAddress, tileOffset, LCDCBit2);
      return result;
    }

    static byte[] flipLookup = new byte[16]
    {
      0x0, 0x8, 0x4, 0xC, 0x2, 0xA, 0x6, 0xE,
      0x1, 0x9, 0x5, 0xD, 0x3, 0xB, 0x7, 0xF
    };

    internal static byte[]
    GetTileData(DisplayDefinition disDef, Memory memory,
                int tileBaseAddress, int tileOffset,
                bool LCDCBit2,
                bool flipX = false, bool flipY = false)
    {
      int tileLength = 16;
      int spriteLength = disDef.bytesPerTileShort;
      if (LCDCBit2) { spriteLength = disDef.bytesPerTileLong; }

      // We obtain the tile memory
      byte[] data = new byte[spriteLength];
      data = memory.LowLevelArrayRead(
        (ushort)(tileBaseAddress + (tileLength * tileOffset)),
        spriteLength);

      if (flipX)
      {
        for (int i = 0; i < spriteLength; ++i)
        {
          byte d = data[i];
          byte r = (byte)((flipLookup[d & 0x0F] << 4) | flipLookup[d >> 4]);
          data[i] = r;
        }
      }

      if (flipY)
      {
        // NOTE(Cristian): We have to flip them in pairs, because
        //                 otherwise the colors change!
        for (int i = 0; i < spriteLength / 4; ++i)
        {
          byte d0 = data[2 * i];
          byte d1 = data[2 * i + 1];

          int index = (spriteLength - 1) - 2 * i;
          data[2 * i] = data[index - 1];
          data[2 * i + 1] = data[index];

          data[index - 1] = d0;
          data[index] = d1;
        }
      }

      return data;
    }

    /// <summary>
    /// Gets a row of pixels from the tilemap
    /// </summary>
    /// <param name="row">The row number to retreive [0, frameHeight)</param>
    /// <param name="LCDBit3">
    /// Whether the LCDC Register (0xFF40) Bit 3 is enabled.
    /// Determines what tilemap (where the tile indexes are) is used:
    /// 0: 0x9800 - 0x9BFF
    /// 1: 0x9C00 - 0x9FFF
    /// </param>
    /// <param name="LCDBit4">
    /// Whether the LCDC Register (0xFF40) Bit 3 is enabled.
    /// Determines the base address for the actual tiles and the
    /// accessing method (interpretation of the byte tile index retreived from the tilemap).
    /// 0: 0x8800 - 0x97FF | signed access
    /// 1: 0x8000 - 0x8FFF | unsigned access
    /// </param>
    /// <returns>An array with the pixels to show for that row (color already calculated)</returns>
    internal static uint[] 
    GetRowPixels(DisplayDefinition disDef, Memory memory,
                 int row, bool LCDBit3, bool LCDBit4)
    {
      // We determine the y tile
      int tileY = row / disDef.pixelPerTileY;
      int tileRemainder = row % disDef.pixelPerTileY;

      ushort tileMapBaseAddress = GetTileMapBaseAddress(LCDBit3);
      ushort tileBaseAddress = GetTileBaseAddress(LCDBit4);

      uint[] pixels = new uint[disDef.framePixelCountX];
      for(int tileX = 0; tileX < disDef.frameTileCountX; tileX++)
      {
        // We obtain the correct tile index
        int tileOffset = GetTileOffset(disDef, memory, tileMapBaseAddress, LCDBit4, tileX, tileY);

        // We obtain both pixels
        int currentTileBaseAddress = tileBaseAddress + disDef.bytesPerTileShort * tileOffset;
        byte top = memory.LowLevelRead((ushort)(currentTileBaseAddress + 2 * tileRemainder));
        byte bottom = memory.LowLevelRead((ushort)(currentTileBaseAddress + 2 * tileRemainder + 1));

        uint[] tilePixels = GetPixelsFromTileBytes(disDef.tilePallete,
                                                   disDef.pixelPerTileX, 
                                                   top, bottom);
        int currentTileIndex = tileX * disDef.pixelPerTileX;
        for (int i = 0; i < disDef.pixelPerTileX; i++)
        {
          pixels[currentTileIndex + i] = tilePixels[i];
        }
      }

      return pixels;
    }

    internal static uint[]
    GetPixelRowFromBitmap(DisplayDefinition disDef, BitmapData bmp, int row)
    {
      uint[] pixels = new uint[bmp.Width];
      unsafe
      {
        int uintStride = bmp.Stride / disDef.bytesPerPixel;
        uint* rowPtr = (uint*)bmp.Scan0 + row * uintStride;


        for (int x = 0; x < bmp.Width; ++x)
        {
          pixels[x] = *rowPtr++;
        }
      }

      return pixels;
    }

    /// <summary>
    /// Gets the OAMs for the a certain row
    /// </summary>
    /// <param name="spriteOAMs"></param>
    /// <param name="row"></param>
    /// <param name="LCDCBit2"></param>
    /// <returns></returns>
    internal static OAM[]
    GetScanLineOAMs(DisplayDefinition disDef, OAM[] spriteOAMs, int row, bool LCDCBit2)
    {
      // First we sort the oams
      // TODO(Cristian): Find a more efficient way to keep this list sorted by priority
      OAM[] oams = (OAM[])spriteOAMs.Clone();
      Array.Sort<OAM>(oams, (a, b) => (a.x == b.x) ?
                                      (a.index - b.index) : (a.x - b.x));

      int spriteSize = disDef.bytesPerTileShort / 2;
      if(LCDCBit2) { spriteSize = disDef.bytesPerTileLong / 2; }

      // Then we select the 10 that correspond
      int scanLineSize = 0;
      int maxScanLineSize = 10;
      OAM[] scanLineOAMs = new OAM[maxScanLineSize];
      foreach(OAM oam in oams)
      {
          int y = oam.y - 16;
          if ((y <= row) && (row < (y + spriteSize)))
          {
            scanLineOAMs[scanLineSize++] = oam;
            if (scanLineSize == maxScanLineSize) { break; }
          }
      }

      // NOTE(Cristian): This array "resize" will actually allocate a new
      //                 array and copy the correspondent data, so it's not
      //                 that magical afterall. (The GB will have to collect the
      //                 past one anyway).
      Array.Resize<OAM>(ref scanLineOAMs, scanLineSize);

      return scanLineOAMs;
    }

    
    internal static void
    GetSpriteRowPixels(DisplayDefinition disDef, Memory memory, OAM[] spriteOAMs,
                       uint[] targetPixels, int row, bool LCDCBit2,
                       bool ignoreBackgroundPriority = false)
    {
      // TODO(Cristian): Separate this step from the call and pass it as an argument
      OAM[] scanLineOAMs = GetScanLineOAMs(disDef, spriteOAMs, row, LCDCBit2);

      // We obtain the pixels we want from it
      for (int oamIndex = scanLineOAMs.Length - 1; oamIndex >= 0; --oamIndex)
      {
        // TODO(Cristian): Obtain only the tile data we care about
        OAM oam = scanLineOAMs[oamIndex];

        bool flipX = Utils.UtilFuncs.TestBit(oam.flags, 5) != 0;
        bool flipY = Utils.UtilFuncs.TestBit(oam.flags, 6) != 0;
        byte[] tilePixels = GetTileData(disDef, memory, 0x8000, oam.spriteCode, 
                                        LCDCBit2, flipX, flipY);

        int x = oam.x - 8;
        int y = row - (oam.y - 16);

        uint[] spritePallete = (Utils.UtilFuncs.TestBit(oam.flags, 4) == 0) ?
                                  disDef.spritePallete0 : disDef.spritePallete1;

        uint[] spritePixels = GetPixelsFromTileBytes(spritePallete,
                                                     disDef.pixelPerTileX,
                                                     tilePixels[2 * y], tilePixels[2 * y + 1]);
        bool backgroundPriority = (Utils.UtilFuncs.TestBit(oam.flags, 7) != 0);
        if(ignoreBackgroundPriority)
        {
          backgroundPriority = false;
        }
        for (int i = 0; i < 8; ++i)
        {
          int pX = x + i;
          if (pX >= disDef.screenPixelCountX) { break; }
          uint color = spritePixels[i];
          if (color == 0) { continue; } // transparent pixel

          // NOTE(Cristian): If the BG priority bit is set, the sprite is hidden
          //                 on every color except tile color 0
          if (backgroundPriority)
          {
            if (targetPixels[pX] != disDef.tileColors[0]) { continue; }
          }
          targetPixels[pX] = color;
        }
      }
    }

    internal static int
    GetTileOffset(DisplayDefinition disDef, Memory memory, 
                  ushort tileMapBaseAddress, bool LCDBit4,
                  int tileX, int tileY)
    {
      int tileOffset;
      if(LCDBit4)
      {
        tileOffset = memory.LowLevelRead((ushort)(tileMapBaseAddress + 
                                                 (disDef.frameTileCountX * tileY) + 
                                                 tileX));
      }
      else
      {
        unchecked
        {
          byte t = memory.LowLevelRead((ushort)(tileMapBaseAddress + 
                                                (disDef.frameTileCountX * tileY) + 
                                                tileX));
          sbyte tR = (sbyte)t;
          tileOffset = tR;
        }
      }

      return tileOffset;
    }


    internal static ushort
    GetTileBaseAddress(bool LCDBit4)
    {
      ushort result = (ushort)(LCDBit4 ? 0x8000 : 0x9000);
      return result;
    }
    
    internal static ushort
    GetTileMapBaseAddress(bool LCDBit3)
    {
      ushort result = (ushort)(LCDBit3 ? 0x9C00 : 0x9800);
      return result;
    }

    internal static void
    SetupTilePallete(DisplayDefinition disDef, Memory memory)
    {
      // We extract the BGP (BG Pallete Data)
      byte bgp = memory.LowLevelRead((ushort)MemoryMappedRegisters.BGP);

      for(int color = 0; color < 4; ++color)
      {
        int down = (bgp >> (2 * color)) & 1;
        int up = (bgp >> (2 * color + 1)) & 1;
        int index = (up << 1) | down;
        disDef.tilePallete[color] = disDef.tileColors[index];
      }
    }

    internal static void
    SetupSpritePalletes(DisplayDefinition disDef, Memory memory)
    {
      byte obp0 = memory.LowLevelRead((ushort)MemoryMappedRegisters.OBP0);
      disDef.spritePallete0[0] = 0x00000000; // Sprite colors are trasparent
      for(int color = 1; color < 4; ++color)
      {
        int down = (obp0 >> (2 * color)) & 1;
        int up = (obp0 >> (2 * color + 1)) & 1;
        int index = (up << 1) | down;
        disDef.spritePallete0[color] = disDef.spriteColors[index];
      }

      byte obp1 = memory.LowLevelRead((ushort)MemoryMappedRegisters.OBP1);
      disDef.spritePallete1[1] = 0x00000000; // Sprite colors are trasparent
      for(int color = 1; color < 4; ++color)
      {
        int down = (obp1 >> (2 * color)) & 1;
        int up = (obp1 >> (2 * color + 1)) & 1;
        int index = (up << 1) | down;
        disDef.spritePallete1[color] = disDef.spriteColors[index];
      }
    }


    internal static uint[] 
    GetPixelsFromTileBytes(uint[] pallete, int pixelPerTileX, byte top, byte bottom)
    {
      uint[] pixels = new uint[pixelPerTileX];
      for(int i = 0; i < pixelPerTileX; i++)
      {
        int up = (bottom >> (7 - i)) & 1;
        int down = (top >> (7 - i)) & 1;
        int index = (up << 1) | down;
        uint color = pallete[index];
        pixels[i] = color;
      }

      return pixels;
    }

    /// <summary>
    /// Draw a tile into a bitmap. 
    /// IMPORTANT: presently it requires that the bitmap be the whole background (256x256 pixels)
    /// </summary>
    /// <param name="bmd">The bitmap data where to output the pixels</param>
    /// <param name="tileData">The 16 bytes that conform the 8x8 pixels</param>
    /// <param name="pX">x coord of the pixel where to start drawing the tile</param>
    /// <param name="pY">y coord of the pixel where to start drawing the tile</param>
    internal static void
    DrawTile(DisplayDefinition disDef, BitmapData bmd, byte[] tileData,
             int pX, int pY, int maxPx, int maxPy)
    {
      unsafe
      {
        int uintStride = bmd.Stride / disDef.bytesPerPixel;
        uint* start = (uint*)bmd.Scan0;

        // We iterate for the actual bytes
        for (int j = 0; j < tileData.Length; j += 2)
        {
          int pixelY = pY + (j / 2);
          if(pixelY < 0) { continue; }
          if(pixelY >= maxPy) { break; } // We can continue no further

          uint* row = start + pixelY * uintStride; // Only add every 2 bytes
          uint[] pixels = GetPixelsFromTileBytes(disDef.tilePallete, 
                                                 disDef.pixelPerTileX,
                                                 tileData[j], tileData[j + 1]);
          for (int i = 0; i < 8; i++)
          {
            int pixelX = pX + i;
            if(pixelX < 0) { continue; }
            if(pixelX >= maxPx) { break; }
            uint* cPtr = row + pixelX;
            cPtr[0] = pixels[i];
          }
        }
      }
    }

    internal static void 
    DrawTransparency(DisplayDefinition disDef, BitmapData bmd, int minX, int minY, int maxX, int maxY)
    {
      uint[] colors = { 0xF0F0F0F0, 0xF0CDCDCD };
      int squareSize = 5;
      unsafe
      {
        int uintStride = bmd.Stride / disDef.bytesPerPixel;
        uint* start = (uint*)bmd.Scan0;

        for(int y = minY; y < maxY; y++)
        {
          uint* rowStart = start + y * uintStride;
          for(int x = minX; x < maxX; x++)
          {
            int sX = x / squareSize;
            int sY = y / squareSize;
            int index = (sX + (sY % 2)) % 2;
            uint* pixel = rowStart + x;
            pixel[0] = colors[index];
          }
        }
      }
    }
    
    internal static void 
    DrawRectangle(DisplayDefinition disDef, BitmapData bmd, 
                  int rX, int rY, int rWidth, int rHeight, uint color, bool fill = false)
    {
      unsafe
      {
        int uintStride = bmd.Stride / disDef.bytesPerPixel;
        for(int y = 0; y < rHeight; y++)
        {
          for (int x = 0; x < rWidth; x++)
          {
            int pX = (rX + x) % disDef.framePixelCountX;
            int pY = (rY + y) % disDef.framePixelCountY;
            if(fill ||
               x == 0 || x == (rWidth - 1) ||
               y == 0 || y == (rHeight - 1))
            {
              uint* p = (uint*)bmd.Scan0 + pY * uintStride + pX;
              p[0] = color;
            }
          }
        }
      }
    }

    internal static void 
    DrawLine(DisplayDefinition disDef, BitmapData bmd, uint[] rowPixels,
             int targetX, int targetY, int rowStart, int rowSpan,
             bool CopyZeroPixels = false)
    {
      // We obtain the data 
      unsafe
      {
        uint* bmdPtr = (uint*)bmd.Scan0 + (targetY * bmd.Stride / disDef.bytesPerPixel) + targetX;
        // NOTE(Cristian): rowMax is included
        for (int i = 0; i < rowSpan; i++)
        {
          // NOTE(Cristian): Sometimes the window is 7 pixels to the left (in the case of the windows)
          //                 We must draw on those cases
          if(targetX < 0)
          {
            targetX++;
            continue;
          } 
          uint pixel = rowPixels[rowStart + i];
          if(!CopyZeroPixels && pixel == 0) { continue; }
          bmdPtr[i] = rowPixels[rowStart + i];
        }
      }
    }
  }
}
