﻿#pragma checksum "..\..\..\..\..\Views\SupportGuideScreen.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "24B98FAE288B739408FFEE24DE5282809734512F"
//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using NeuroApp;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace NeuroApp {
    
    
    /// <summary>
    /// SupportGuideScreen
    /// </summary>
    public partial class SupportGuideScreen : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 70 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ProtocolDialogButton;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SearchDialogButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/NeuroApp;component/views/supportguidescreen.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 41 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BackButton_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ProtocolDialogButton = ((System.Windows.Controls.Button)(target));
            
            #line 75 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
            this.ProtocolDialogButton.Click += new System.Windows.RoutedEventHandler(this.ProtocolDialogButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.SearchDialogButton = ((System.Windows.Controls.Button)(target));
            
            #line 92 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
            this.SearchDialogButton.Click += new System.Windows.RoutedEventHandler(this.SearchDialogButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 115 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenStaticGuide_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 134 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenGuide_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 153 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenGuide_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 173 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenGuide_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 192 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenGuide_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 211 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenStaticGuide_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 231 "..\..\..\..\..\Views\SupportGuideScreen.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenStaticGuide_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

