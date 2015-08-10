﻿using System.Windows;
using GBSharp.ViewModel;
using Microsoft.Win32;

namespace GBSharp.View
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly GameBoyViewModel _mainWindowViewModel;
    private readonly IGameBoy _gameBoy;

    public MainWindow()
    {
      InitializeComponent();
      _gameBoy = new GameBoy();
      _mainWindowViewModel = new GameBoyViewModel(_gameBoy, new DispatcherAdapter(this));
      this.DataContext = _mainWindowViewModel;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      var openFileDialog = new OpenFileDialog();
      openFileDialog.ShowDialog();

      FileText.Text = openFileDialog.FileName;
    }
  }
}
