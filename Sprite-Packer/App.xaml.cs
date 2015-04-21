using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Sprite_Packer {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void Application_Startup( object sender, StartupEventArgs e ) {
            MainWindow window = new MainWindow( );

            // Stuff...

            window.Show( );
        }

        private void Application_DispatcherUnhandledException( object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e ) {
            MessageBox.Show( "ERROR: " + e.Exception.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error );
            e.Handled = true;
        }
    }
}
