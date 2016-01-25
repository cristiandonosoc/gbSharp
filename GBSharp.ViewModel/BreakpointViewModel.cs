﻿using GBSharp.CPUSpace;
using System;

namespace GBSharp.ViewModel
{
  public class BreakpointViewModel : ViewModelBase
  {
    public event Action BreakpointChanged;

    private readonly IGameBoy _gameboy;
    private readonly IInstruction _instruction;

    public ushort OriginalAddress { get; private set; }
    public string Address { get; private set; }
    public string Name { get; private set; }


    #region EXECUTE

    private bool _onExecute;
    internal bool DirectOnExecute
    {
      set
      {
        _onExecute = value;
        OnPropertyChanged(() => OnExecute);
      }
    }
    public bool OnExecute
    {
      get { return _onExecute; }
      set
      {
        if(_onExecute == value) { return; }
        _onExecute = value;

        if(_onExecute)
        {
          _gameboy.CPU.AddBreakpoint(BreakpointKinds.EXECUTION, _instruction.Address);
        }
        else
        {
          _gameboy.CPU.RemoveBreakpoint(BreakpointKinds.EXECUTION, _instruction.Address);
        }

        // Execute breakpoint allways triggers because there are other views
        // that update according to this flag
        BreakpointChanged();
        OnPropertyChanged(() => OnExecute);
      }
    }
    private bool _isExecuteActive;
    public bool IsExecuteActive
    {
      get { return _isExecuteActive; }
      set
      {
        if(_isExecuteActive == value) { return; }
        _isExecuteActive = value;
        OnPropertyChanged(() => IsExecuteActive);
      }
    }

    #endregion

    #region READ

    private bool _onRead;
    internal bool DirectOnRead
    {
      set
      {
        _onRead = value;
        OnPropertyChanged(() => OnRead);
      }
    }
    public bool OnRead
    {
      get { return _onRead; }
      set
      {
        if(_onRead == value) { return; }
        _onRead = value;

        if(_onRead)
        {
          _gameboy.CPU.AddBreakpoint(BreakpointKinds.READ, _instruction.Address);
        }
        else
        {
          _gameboy.CPU.RemoveBreakpoint(BreakpointKinds.READ, _instruction.Address);
        }

        // If all kinds were disabled, we remove the breakpoint
        if(!_onExecute && !_onRead && !_onWrite && !_onJump)
        {
          BreakpointChanged();
          return;
        }
        
        OnPropertyChanged(() => OnRead);
      }
    }
    private bool _isReadActive;
    public bool IsReadActive
    {
      get { return _isReadActive; }
      set
      {
        if(_isReadActive == value) { return; }
        _isReadActive = value;
        OnPropertyChanged(() => IsReadActive);
      }
    }

    #endregion

    #region WRITE
 
    private bool _onWrite;
    internal bool DirectOnWrite
    {
      set
      {
        _onWrite = value;
        OnPropertyChanged(() => OnWrite);
      }
    }
    public bool OnWrite
    {
      get { return _onWrite; }
      set
      {
        if(_onWrite == value) { return; }
        _onWrite = value;

        if(_onWrite)
        {
          _gameboy.CPU.AddBreakpoint(BreakpointKinds.WRITE, _instruction.Address);
        }
        else
        {
          _gameboy.CPU.RemoveBreakpoint(BreakpointKinds.WRITE, _instruction.Address);
        }

        // If all kinds were disabled, we remove the breakpoint
        if(!_onExecute && !_onRead && !_onWrite && !_onJump)
        {
          BreakpointChanged();
          return;
        }

        OnPropertyChanged(() => OnWrite);
      }
    }
    private bool _isWriteActive;
    public bool IsWriteActive
    {
      get { return _isWriteActive; }
      set
      {
        if(_isWriteActive == value) { return; }
        _isWriteActive = value;
        OnPropertyChanged(() => IsWriteActive);
      }
    }

    #endregion

    #region JUMP

    private bool _onJump;
    internal bool DirectOnJump
    {
      set
      {
        _onJump = value;
        OnPropertyChanged(() => OnJump);
      }
    }
    public bool OnJump
    {
      get { return _onJump; }
      set
      {
        if(_onJump == value) { return; }
        _onJump = value;

        if(_onJump)
        {
          _gameboy.CPU.AddBreakpoint(BreakpointKinds.JUMP, _instruction.Address);
        }
        else
        {
          _gameboy.CPU.RemoveBreakpoint(BreakpointKinds.JUMP, _instruction.Address);
        }

        // If all kinds were disabled, we remove the breakpoint
        if(!_onExecute && !_onRead && !_onWrite && !_onJump)
        {
          BreakpointChanged();
          return;
        }

        OnPropertyChanged(() => OnJump);
      }
    }
    private bool _isJumpActive;
    public bool IsJumpActive
    {
      get { return _isJumpActive; }
      set
      {
        if(_isJumpActive == value) { return; }
        _isJumpActive = value;
        OnPropertyChanged(() => IsJumpActive);
      }
    }

    #endregion

    public BreakpointViewModel(IGameBoy gameboy, IInstruction inst)
    {
      _gameboy = gameboy;
      _instruction = inst;

      OriginalAddress = inst.Address;
      Address = "0x" + inst.Address.ToString("x2");
      Name = inst.Name;
    }
  }
}