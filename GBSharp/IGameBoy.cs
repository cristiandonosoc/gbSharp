﻿using GBSharp.Cartridge;
using System;

namespace GBSharp
{
  public interface IGameBoy
  {
    event Action StepFinished;
    ICPU CPU { get; }
    IMemory Memory { get; }
    ICartridge Cartridge { get; }
    IDisplay Display { get; }
    void LoadCartridge(byte[] cartridgeData);
    void Run();
    void Pause();
    void Stop();
    void Step();
    void PressButton(Keypad button);
    void ReleaseButton(Keypad button);
    
  }
}