﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sprite_Packer {
    /// <summary>
    /// Interaction logic for Window_ExportPreview.xaml
    /// </summary>
    public partial class Window_ExportPreview : Window {
        public Window_ExportPreview( ) {
            InitializeComponent( );
        }

        private void Execute_Close( object sender, ExecutedRoutedEventArgs e ) {
            this.Close( );
        }

        private void CanExecute_Always( object sender, CanExecuteRoutedEventArgs e ) {
            e.CanExecute = true;
        }
    }
}
