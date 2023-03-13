﻿using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Singleton]
public partial class TracksPage : Page, INavigationPage
{
    public TracksPage()
    {
        InitializeComponent();
    }

    public void Display()
    {
        throw new System.NotImplementedException();
    }
}