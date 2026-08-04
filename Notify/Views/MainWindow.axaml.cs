using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LibVLCSharp.Shared;
using LibVLCSharp.Avalonia;
using Notify.Classes;
using Notify.Data;
using Notify.ViewModels;

namespace Notify.Views;

public partial class MainWindow : Window
{
    private ArrayList musicNames = new();
    private int _currentIdx = 0;
    
    public MainWindow()
    {
        InitializeComponent();
        //GetFileData();
    }


    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        
        if (DataContext is MainWindowViewModel vm)
        {
            //await vm.LoadFolderAsync(StorageProvider);
        }
    }
    private void ItemSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            if (vm.SelectedIdx == null)
                return;
            var file = vm.Items[vm.SelectedIdx];
            if (file is Song song)
            {
                Uri uri = new Uri(song.SongFilePath);
                vm.Play(uri);
                SelectedFile.Text = song.SongName;
            }
        }
    }
}