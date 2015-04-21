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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Collections.ObjectModel; // ObservableCollection
using System.ComponentModel; // INotifyPropertyChanged
using Microsoft.Win32; // Windows API
using System.Xml.Linq; // Linq to xml

namespace Sprite_Packer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public SpriteSheet SpriteSheet { set; get; }

        public MainWindow( ) {
            SpriteSheet = new SpriteSheet( ) { Spacing = 5 };
            DataContext = this;

            InitializeComponent( );

            listAnimView.ItemsSource = SpriteSheet.AnimationList;
        }


        public void UpdatePreview( object sender = null, RoutedEventArgs e = null ) {
            SpriteAnimation foo = listAnimView.SelectedItem as SpriteAnimation;

            if( foo != null && SpriteSheet.AnimationList.Contains( foo ) ) {

                canvasImage.Children.Clear( );

                double x_size = SpriteSheet.Spacing;
                double y_size = SpriteSheet.Spacing;

                foreach( SpriteImage bar in foo.SpriteList ) {
                    Image foobar = new Image( );
                    foobar.Source = bar.Image;

                    Canvas.SetTop( foobar, SpriteSheet.Spacing );
                    Canvas.SetLeft( foobar, x_size );

                    canvasImage.Children.Add( foobar );

                    x_size += bar.Image.PixelWidth + SpriteSheet.Spacing;
                    if( y_size < SpriteSheet.Spacing + SpriteSheet.Spacing + bar.Image.PixelHeight ) {
                        y_size = SpriteSheet.Spacing + SpriteSheet.Spacing + bar.Image.PixelHeight;
                    }
                }

                canvasImage.Width = x_size;
                canvasImage.Height = y_size;
            }
        }


        private void ListViewSelectionChange( object sender, SelectionChangedEventArgs e ) {
            UpdatePreview( );
        }


        private void Command_Execute_New( object sender, ExecutedRoutedEventArgs e ) {

            string title = "New";
            string confirm = "Confirm new? All unsaved data will be lost?";

            if( MessageBox.Show( confirm, title, MessageBoxButton.OKCancel, MessageBoxImage.Question ) == MessageBoxResult.OK ) {
                SpriteSheet = new SpriteSheet( );
                canvasImage.Children.Clear( );
                listAnimView.ItemsSource = SpriteSheet.AnimationList;
                listSpriteView.ItemsSource = null;
            }

            UpdatePreview( );
        }
        private void Command_Execute_Save( object sender, ExecutedRoutedEventArgs e ) {

        }
        private void Command_Execute_Close( object sender, ExecutedRoutedEventArgs e ) {
            this.Close( );
        }


        private void Command_Execute_AnimAdd( object sender, ExecutedRoutedEventArgs e ) {
            SpriteSheet.AnimationList.Add( new SpriteAnimation( ) );
        }
        private void Command_Execute_AnimEdit( object sender, ExecutedRoutedEventArgs e ) {

        }
        private void Command_Execute_AnimRemove( object sender, ExecutedRoutedEventArgs e ) {
            SpriteAnimation foo = listAnimView.SelectedItem as SpriteAnimation;

            if( foo != null && SpriteSheet.AnimationList.Contains( foo ) ) {

                foreach( SpriteImage bar in foo.SpriteList ) {
                    bar.Image = null;
                }

                foo.SpriteList.Clear( );

                SpriteSheet.AnimationList.RemoveAt( SpriteSheet.AnimationList.IndexOf( foo ) );
            }

            UpdatePreview( );
        }


        private void Command_Execute_SpriteAdd( object sender, ExecutedRoutedEventArgs e ) {
            SpriteAnimation foo = listAnimView.SelectedItem as SpriteAnimation;

            if( foo != null && SpriteSheet.AnimationList.Contains( foo ) ) {
                WriteableBitmap newBitmap = BitmapFactory.New( 32, 32 );
                newBitmap.Clear( Colors.DarkGreen );
                SpriteImage newSprite = new SpriteImage( ) { Image = newBitmap };
                foo.AddSprite( newSprite );
            }

            UpdatePreview( );
        }
        private void Command_Execute_SpriteEdit( object sender, ExecutedRoutedEventArgs e ) {

        }
        private void Command_Execute_SpriteRemove( object sender, ExecutedRoutedEventArgs e ) {
            SpriteAnimation foo = listAnimView.SelectedItem as SpriteAnimation;

            if( foo != null && SpriteSheet.AnimationList.Contains( foo ) ) {

                SpriteImage bar = listSpriteView.SelectedItem as SpriteImage;

                if( bar != null && foo.SpriteList.Contains( bar ) ) {
                    bar.Image = null;

                    foo.SpriteList.RemoveAt( foo.SpriteList.IndexOf( bar ) );
                }
            }

            UpdatePreview( );
        }


        private void Command_Execute_SpriteMoveUp( object sender, ExecutedRoutedEventArgs e ) {
            SpriteAnimation foo = listAnimView.SelectedItem as SpriteAnimation;

            if( foo != null && SpriteSheet.AnimationList.Contains( foo ) ) {

                SpriteImage bar = listSpriteView.SelectedItem as SpriteImage;

                if( bar != null && foo.SpriteList.Contains( bar ) ) {
                    int index = foo.SpriteList.IndexOf( bar );
                    if( index > 0 ) {
                        foo.SpriteList.Move( index, index - 1 );
                    }
                }
            }

            UpdatePreview( );
        }
        private void Command_Execute_SpriteMoveDown( object sender, ExecutedRoutedEventArgs e ) {
            SpriteAnimation foo = listAnimView.SelectedItem as SpriteAnimation;

            if( foo != null && SpriteSheet.AnimationList.Contains( foo ) ) {

                SpriteImage bar = listSpriteView.SelectedItem as SpriteImage;

                if( bar != null && foo.SpriteList.Contains( bar ) ) {
                    int index = foo.SpriteList.IndexOf( bar );
                    if( index < foo.SpriteList.Count - 1 ) {
                        foo.SpriteList.Move( index, index + 1 );
                    }
                }
            }

            UpdatePreview( );
        }


        private void Command_CanExecute_AnimationSelected( object sender, CanExecuteRoutedEventArgs e ) {
            e.CanExecute = false;
            if( listAnimView != null ) {
                e.CanExecute = listAnimView.SelectedItem != null;
            }
        }
        private void Command_CanExecute_SpriteSelected( object sender, CanExecuteRoutedEventArgs e ) {
            e.CanExecute = false;
            if( listSpriteView != null ) {
                e.CanExecute = listSpriteView.SelectedItem != null;
            }
        }
        private void Command_CanExecute_AlwaysTrue( object sender, CanExecuteRoutedEventArgs e ) {
            e.CanExecute = true;
        }
        private void Command_CanExecute_DataExists( object sender, CanExecuteRoutedEventArgs e ) {
            bool animExits = SpriteSheet.AnimationList.Count > 0; // True if any animations exist.

            bool spriteExists = false;
            foreach( SpriteAnimation anim in SpriteSheet.AnimationList ) {
                spriteExists = anim.SpriteList.Count > 0 || spriteExists; // Returns true if there are any sprites in the animation OR spriteExists is already true;
            }

            e.CanExecute = animExits && spriteExists;
        }
    }

    public class SpriteImage : INotifyPropertyChanged {
        private string name;
        public string Name {
            get { return this.name; }
            set {
                if( this.name != value ) {
                    this.name = value;
                    this.NotifyPropertyChanged( "Name" );
                }
            }
        }

        public WriteableBitmap Image { set; get; }

        // Getters for the WirteableBitmap
        public int Width { get { return this.Image.PixelWidth; } }
        public int Height { get { return this.Image.PixelHeight; } }

        public SpriteImage( ) {
            Name = "SpriteImage";
            Image = null;
        }

        public XElement ToXElement( ) {
            XAttribute Xname = new XAttribute( "Name", Name );
            XElement Xsize = new XElement( "Size", new XAttribute( "x", Image.PixelWidth ), new XAttribute( "y", Image.PixelHeight ) );

            XElement Ximage = new XElement( "Image", Xname, Xsize );

            return Ximage;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged( string propertyName ) {
            if( PropertyChanged != null ) { PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) ); }
        }
    }

    public class SpriteAnimation : INotifyPropertyChanged {
        private string name;
        public string Name {
            get { return this.name; }
            set {
                if( this.name != value ) {
                    this.name = value;
                    this.NotifyPropertyChanged( "Name" );
                }
            }
        }

        public ObservableCollection<SpriteImage> SpriteList { set; get; }

        public SpriteAnimation( ) {
            SpriteList = new ObservableCollection<SpriteImage>( );
            Name = "Animation";
        }

        public XElement ToXElement( ) {
            XElement element = new XElement( "Animation" );

            // For loops to get each image's xml
            foreach( SpriteImage foo in SpriteList ) {
                element.Add( foo.ToXElement( ) );
            }

            element.Add( new XAttribute( "Count", SpriteList.Count ) );

            return element;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged( string propertyName ) {
            if( PropertyChanged != null ) { PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) ); }
        }

        public void AddSprite( SpriteImage foo ) {
            SpriteList.Add( foo );
            foreach( SpriteImage bar in SpriteList ) {
                bar.Name = SpriteList.IndexOf( bar ).ToString( );
            }
        }

    }

    public class SpriteSheet {
        public ObservableCollection<SpriteAnimation> AnimationList { set; get; }

        public int Spacing { set; get; }

        public SpriteSheet( ) {
            AnimationList = new ObservableCollection<SpriteAnimation>( );
            AnimationList.Add( new SpriteAnimation( ) );
            Spacing = 0;
        }

        public XDocument ToXML( ) {
            XDocument xmlDocument = new XDocument( );
            XElement sheet = new XElement( "SpriteSheet" );
            xmlDocument.Add( sheet );

            // For loops to get each animation's xml
            foreach( SpriteAnimation foo in AnimationList ) {
                xmlDocument.Add( foo.ToXElement( ) );
            }

            xmlDocument.Add( new XAttribute( "Count", AnimationList.Count ) );

            return xmlDocument;
        }
    }
}
