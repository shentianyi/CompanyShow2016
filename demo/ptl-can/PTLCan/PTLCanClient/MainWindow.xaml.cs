﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PTLCanClient.Properties;

namespace PTLCanClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        PTLCanController.Controller controller;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                controller = new PTLCanController.Controller(Settings.Default.com);
                controller.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                controller.Send(int.Parse(LightIdTB.Text), int.Parse(NumberTB.Text), textBox.Text);

            }
            catch (Exception ex)
            {
 MessageBox.Show(ex.Message);
            }
        }
    }
}
