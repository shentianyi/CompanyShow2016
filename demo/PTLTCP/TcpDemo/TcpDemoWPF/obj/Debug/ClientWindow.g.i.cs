﻿#pragma checksum "..\..\ClientWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3056C04A3435D5ED04767C88DE856BA8"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using TcpDemoWPF;


namespace TcpDemoWPF {
    
    
    /// <summary>
    /// ClientWindow
    /// </summary>
    public partial class ClientWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\ClientWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button MakeConnect;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\ClientWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ReceiveMessageText;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\ClientWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ReceiveMessage;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\ClientWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock SendMessage;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\ClientWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SendMessageText;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\ClientWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\ClientWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox serverIPTB;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\ClientWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label1;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\ClientWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox serverPortTB;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\ClientWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Stabilitybutton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TcpDemoWPF;component/clientwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ClientWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\ClientWindow.xaml"
            ((TcpDemoWPF.ClientWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MakeConnect = ((System.Windows.Controls.Button)(target));
            
            #line 10 "..\..\ClientWindow.xaml"
            this.MakeConnect.Click += new System.Windows.RoutedEventHandler(this.MakeConnect_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ReceiveMessageText = ((System.Windows.Controls.TextBox)(target));
            
            #line 11 "..\..\ClientWindow.xaml"
            this.ReceiveMessageText.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.ReceiveMessageText_TextChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ReceiveMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.SendMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.SendMessageText = ((System.Windows.Controls.TextBox)(target));
            
            #line 14 "..\..\ClientWindow.xaml"
            this.SendMessageText.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.SendMessageText_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.label = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.serverIPTB = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.label1 = ((System.Windows.Controls.Label)(target));
            return;
            case 10:
            this.serverPortTB = ((System.Windows.Controls.TextBox)(target));
            return;
            case 11:
            this.Stabilitybutton = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\ClientWindow.xaml"
            this.Stabilitybutton.Click += new System.Windows.RoutedEventHandler(this.Stabilitybutton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

