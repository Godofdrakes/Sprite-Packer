using System;
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
    /// Interaction logic for Window_Rename.xaml
    /// </summary>
    public partial class Window_Rename : Window {
        public Window_Rename( ) {
            InitializeComponent( );
            boxRename.Focus( );
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {
            Close( );
        }
    }
}
