﻿using GBSharp.CPUSpace;
using GBSharp.MemorySpace;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace GBSharp.VideoSpace
{
  public struct OAM
  {
    internal int index;
    internal byte y;
    internal byte x;
    internal byte spriteCode;
    internal byte flags;

    public byte X { get { return x; } }
    public byte Y { get { return y; } }
    public byte SpriteCode { get { return spriteCode; } }
    public byte Flags { get { return flags; } }

  }

  internal struct DisplayDefinition
  {
    internal int framePixelCountX;
    internal int framePixelCountY;
    internal int screenPixelCountX;
    internal int screenPixelCountY;
    internal int frameTileCountX;
    internal int frameTileCountY;
    internal int screenTileCountX;
    internal int screenTileCountY;
    internal int bytesPerTile;
    internal int pixelPerTileX;
    internal int pixelPerTileY;
    internal int bytesPerPixel;
    internal PixelFormat pixelFormat;
    internal uint[] tileColors;
    internal uint[] spriteColors;
    internal uint[] tilePallete;
    internal uint[] spritePallete0;
    internal uint[] spritePallete1;
  }

  class Display : IDisplay
  {
    public event Action RefreshScreen;

    private Memory memory;

    private DisplayDefinition disDef;

    private int spriteCount = 40;
    private OAM[] spriteOAMs;
    public OAM GetOAM(int index)
    {
      return spriteOAMs[index];
    }
    internal void SetOAM(int index, byte x, byte y, byte spriteCode, byte flags)
    {
      spriteOAMs[index].index = index;
      spriteOAMs[index].x = x;
      spriteOAMs[index].y = y;
      spriteOAMs[index].spriteCode = spriteCode;
      spriteOAMs[index].flags = flags;
    }


    /// <summary>
    /// Load an OAM from direct byte data. NOTE THE ARRAY FORMAT
    /// </summary>
    /// <param name="index"></param>
    /// <param name="data">
    /// Data layout assumes
    /// data[0]: y 
    /// data[0]: x
    /// data[0]: spriteCode 
    /// data[0]: flags 
    /// This is because this is the way the OAM are in memory.
    /// </param>
    internal void SetOAM(int index, byte[] data)
    {
      SetOAM(index, data[1], data[0], data[2], data[3]);
    }

    /// <summary>
    /// Pixel numbers of the display.
    /// With this it should be THEORETICALLY have displays
    /// of different sizes without too much hazzle.
    /// </summary>
    private Bitmap background;
    public Bitmap Background { get { return background; } }

    private Bitmap window;
    public Bitmap Window { get { return window; } }

    /// <summary>
    /// The final composed frame from with the screen is calculated.
    /// It's the conbination of the background, window and sprites.
    /// </summary>
    private Bitmap frame;
    public Bitmap Frame { get { return frame; } }


    private Bitmap sprite;

    private Bitmap spriteLayer;
    public Bitmap SpriteLayer { get { return spriteLayer; } }


    /// <summary>
    /// The bitmap that represents the actual screen.
    /// It's a portial of the frame specified by the SCX and SCY registers.
    /// </summary>
    private Bitmap screen;
    public Bitmap Screen { get { return screen; } }


    /// <summary>
    /// Display constructor.
    /// </summary>
    /// <param name="interruptController">A reference to the interrupt controller.</param>
    /// <param name="Memory">A reference to the memory.</param>
    public Display(InterruptController interruptController, Memory memory)
    {
      disDef = new DisplayDefinition();
      disDef.framePixelCountX = 256;
      disDef.framePixelCountY = 256;
      disDef.screenPixelCountX = 160;
      disDef.screenPixelCountY = 144;
      disDef.frameTileCountX = 32;
      disDef.frameTileCountY = 32;
      disDef.screenTileCountX = 20;
      disDef.screenTileCountY = 18;
      disDef.bytesPerTile = 16;
      disDef.pixelPerTileX = 8;
      disDef.pixelPerTileY = 8;
      disDef.bytesPerPixel = 4;
      disDef.pixelFormat = PixelFormat.Format32bppArgb;
      // TODO(Cristian): Output the color to the view for custom setting
      disDef.tileColors = new uint[4]
      {
        0xFFFFFFFF,
        0xFFBBBBBB,
        0xFF666666,
        0xFF000000
      };
      disDef.tilePallete = new uint[4];

      // TODO(Cristian): Output the color to the view for custom setting
      disDef.spriteColors = new uint[4]
      {
        0xFFFFFFFF,
        0xFFBBBBBB,
        0xFF666666,
        0xFF000000
      };
      disDef.spritePallete0 = new uint[4];
      disDef.spritePallete1 = new uint[4];

      this.memory = memory;

      // We create the target bitmaps
      background = new Bitmap(disDef.framePixelCountX, disDef.framePixelCountY, disDef.pixelFormat);
      window = new Bitmap(disDef.screenPixelCountX, disDef.screenPixelCountY, disDef.pixelFormat);
      sprite = new Bitmap(8, 8, disDef.pixelFormat);
      spriteLayer = new Bitmap(disDef.screenPixelCountX, disDef.screenPixelCountY, disDef.pixelFormat);

      screen = new Bitmap(disDef.screenPixelCountX, disDef.screenPixelCountY, disDef.pixelFormat);
      frame = new Bitmap(disDef.framePixelCountX, disDef.framePixelCountY, disDef.pixelFormat);

      // TODO(Cristian): Remove this call eventually, when testing is not needed!
#if DEBUG
      //UpdateScreen();
#endif
    }

    internal void LoadSprites()
    {
      // We load the OAMs
      ushort spriteOAMAddress = 0xFE00;
      spriteOAMs = new OAM[spriteCount];
      for (int i = 0; i < spriteCount; i++)
      {
        SetOAM(i, memory.LowLevelArrayRead(spriteOAMAddress, 4));
        spriteOAMAddress += 4;
      }
    }


    public Bitmap GetSprite(int index)
    {
      BitmapData spriteBmd = DisplayFunctions.LockBitmap(sprite,
                                                         ImageLockMode.WriteOnly,
                                                         disDef.pixelFormat);
      OAM oam = GetOAM(index);
      DrawSprite(spriteBmd, oam.spriteCode, 0, 0);
      sprite.UnlockBits(spriteBmd);
      return sprite;
    }

    internal void DrawSprite(BitmapData spriteBmd, int spriteCode, int pX, int pY)
    {
      byte[] pixels = DisplayFunctions.GetTileData(disDef, memory, 0x8000, spriteCode);
      DisplayFunctions.DrawTile(disDef, spriteBmd, pixels, pX, pY,
                                disDef.screenPixelCountX, disDef.screenPixelCountY);
    }

    public void UpdateScreen()
    {
      LoadSprites();

      // TODO(Cristian): Do the image composition line-based instead of image-based
      byte lcdRegister = this.memory.LowLevelRead((ushort)MemoryMappedRegisters.LCDC);
      bool LCDBit3 = Utils.UtilFuncs.TestBit(lcdRegister, 3) != 0;
      bool LCDBit4 = Utils.UtilFuncs.TestBit(lcdRegister, 4) != 0;

      DisplayFunctions.SetupTilePallete(disDef, memory);
      DisplayFunctions.SetupSpritePalletes(disDef, memory);

      BitmapData screenBmpData = DisplayFunctions.LockBitmap(screen,
                                                             ImageLockMode.ReadWrite,
                                                             disDef.pixelFormat);

      #region BACKGROUND


      int SCX = this.memory.LowLevelRead((ushort)MemoryMappedRegisters.SCX);
      int SCY = this.memory.LowLevelRead((ushort)MemoryMappedRegisters.SCY);
      BitmapData backgroundBmpData = DisplayFunctions.LockBitmap(background, 
                                                                 ImageLockMode.ReadWrite, 
                                                                 disDef.pixelFormat);
      for (int row = 0; row < disDef.framePixelCountY; row++)
      {
        uint[] rowPixels = DisplayFunctions.GetRowPixels(disDef, memory, row, LCDBit3, LCDBit4);
        DisplayFunctions.DrawLine(disDef, backgroundBmpData, rowPixels, 
                                  0, row, 
                                  0, disDef.framePixelCountY);

        // TODO(Cristian): Move the background render to a DrawLine call instead of copying
        //                 from one bitmap to another
      }

      bool drawBackground = Utils.UtilFuncs.TestBit(lcdRegister, 0) != 0;
      if(drawBackground)
      {
        // We copy the information from the background tile to the effective screen
        int backgroundUintStride = backgroundBmpData.Stride / disDef.bytesPerPixel;
        int screenUintStride = screenBmpData.Stride / disDef.bytesPerPixel;
        for (int y = 0; y < disDef.screenPixelCountY; y++)
        {
          for (int x = 0; x < disDef.screenPixelCountX; x++)
          {
            int pX = (x + SCX) % disDef.framePixelCountX;
            int pY = (y + SCY) % disDef.framePixelCountY;

            unsafe
            {
              uint* bP = (uint*)backgroundBmpData.Scan0 + (pY * backgroundUintStride) + pX;
              uint* sP = (uint*)screenBmpData.Scan0 + (y * screenUintStride) + x;
              sP[0] = bP[0];
            }
          }
        }
      }

      bool drawRectangle = true;
      if(drawRectangle)
      {
        uint rectangleColor = 0xFFFF8822;
        DisplayFunctions.DrawRectangle(disDef, backgroundBmpData, 
                                       SCX, SCY, 
                                       disDef.screenPixelCountX, disDef.screenPixelCountY, 
                                       rectangleColor);
      }

      background.UnlockBits(backgroundBmpData);

      #endregion

      #region WINDOW

      int WX = this.memory.LowLevelRead((ushort)MemoryMappedRegisters.WX);
      int rWX = WX - 7; // The window pos is (WX - 7, WY)
      int WY = this.memory.LowLevelRead((ushort)MemoryMappedRegisters.WY);

      BitmapData windowBmpData = DisplayFunctions.LockBitmap(window,
                                                             ImageLockMode.ReadWrite,
                                                             disDef.pixelFormat);
      DisplayFunctions.DrawTransparency(disDef, windowBmpData, 0, 0, disDef.screenPixelCountX, WY);
      DisplayFunctions.DrawTransparency(disDef, windowBmpData, 0, WY, rWX, disDef.screenPixelCountY);

      bool drawWindow = Utils.UtilFuncs.TestBit(lcdRegister, 5) != 0;
      for (int row = 0; row < disDef.screenPixelCountY; row++)
      {
        if (row >= WY)
        {
          // The offset indexes represent that the window is drawn from it's beggining
          // at (WX, WY)
          uint[] rowPixels = DisplayFunctions.GetRowPixels(disDef, memory, row - WY, LCDBit3, LCDBit4);
          
          // Independent target
          DisplayFunctions.DrawLine(disDef, windowBmpData, rowPixels, 
                                    rWX, row, 
                                    0, disDef.screenPixelCountX - rWX);

          // Screen target
          if (drawWindow)
          {
            DisplayFunctions.DrawLine(disDef, screenBmpData, rowPixels,
                                      rWX, row,
                                      0, disDef.screenPixelCountX - rWX);
          }
        }
      }

      window.UnlockBits(windowBmpData);

      #endregion

      #region SPRITES

      // *** SPRITES ***
      BitmapData spriteLayerBmp = DisplayFunctions.LockBitmap(spriteLayer,
                                                              ImageLockMode.ReadWrite,
                                                              disDef.pixelFormat);
      DisplayFunctions.DrawTransparency(disDef, spriteLayerBmp, 0, 0, spriteLayerBmp.Width, spriteLayerBmp.Height);

      bool drawSprites = Utils.UtilFuncs.TestBit(lcdRegister, 1) != 0;
      for (int row = 0; row < disDef.screenPixelCountY; row++)
      {
        // Independent target
        uint[] pixels = new uint[disDef.screenPixelCountX];
        DisplayFunctions.GetSpriteRowPixels(disDef, memory, spriteOAMs, pixels, row);
        DisplayFunctions.DrawLine(disDef, spriteLayerBmp, pixels, 0, row, 0, disDef.screenPixelCountX);

        // Screen Target
        if(drawSprites)
        {
          uint[] linePixels = DisplayFunctions.GetPixelRowFromBitmap(disDef, screenBmpData, row);
          DisplayFunctions.GetSpriteRowPixels(disDef, memory, spriteOAMs, linePixels, row);
          DisplayFunctions.DrawLine(disDef, screenBmpData, linePixels, 0, row, 0, disDef.screenPixelCountX);
        }
      }

      spriteLayer.UnlockBits(spriteLayerBmp);

      #endregion

      // *** SCREEN RECTANGLE ***

      screen.UnlockBits(screenBmpData);
    }


    private const int screenStep = 96905; // Aprox. ~16.6687 ms
    private int screenSum = 0;
    /// <summary>
    /// Simulates the update of the display for a period of time of a given number of ticks.
    /// </summary>
    /// <param name="ticks">The number of ticks ellapsed since the last call.
    /// A tick is a complete source clock oscillation, ~238.4 ns (2^-22 seconds).</param>
    internal void Step(byte ticks)
    {
      // Count ticks and then..
      // OAM Access?
      // Do Line Magics?
      // H-Blank?
      // V-Blank?

      screenSum += ticks;
      if(screenSum > screenStep)
      {
        screenSum %= screenStep;
        UpdateScreen();
        if (RefreshScreen != null)
        {
          RefreshScreen();
        }
      } 
    }
  }
}
