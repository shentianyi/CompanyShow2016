  
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
        public MainWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Signle 和 Multi 的点击事件都绑定同一个事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showLightWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            new LightWindow()
            {
                lightMode = (sender as Button).Name == "singleBtn" ? LightMode.Sigle : LightMode.Multi
            }.ShowDialog();
         
        }
         
    }
} 
