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

using System.IO; // FIleStream
using System.Collections.ObjectModel; // ObservableCollection
using System.ComponentModel; // INotifyPropertyChanged
using Microsoft.Win32; // Windows API
using System.Xml.Linq; // Linq to xml

namespace Sprite_Packer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public SpriteSheet spriteSheet { set; get; }

        public MainWindow( ) {
            spriteSheet = new SpriteSheet( ) { Padding = 5 };
            spriteSheet.AnimationList.Add( new SpriteAnimation( ) );
            DataContext = this;

            InitializeComponent( );

            listAnimView.ItemsSource = spriteSheet.AnimationList;
            listAnimView.SelectedIndex = spriteSheet.AnimationList.Count - 1;
            listAnimView.Focus( );
        }

        public void UpdatePreview( object sender = null, RoutedEventArgs e = null ) {
            SpriteAnimation foo = listAnimView.SelectedItem as SpriteAnimation;

            if( foo != null ) {

                canvasImage.Children.Clear( );

                double x_size = spriteSheet.Padding;
                double y_size = spriteSheet.Padding;

                foreach( SpriteImage bar in foo.SpriteList ) {
                    Image foobar = new Image( );
                    foobar.Source = bar.Image;

                    Canvas.SetTop( foobar, spriteSheet.Padding );
                    Canvas.SetLeft( foobar, x_size );

                    canvasImage.Children.Add( foobar );

                    x_size += bar.Image.PixelWidth + spriteSheet.Padding;
                    if( y_size < spriteSheet.Padding + spriteSheet.Padding + bar.Image.PixelHeight ) {
                        y_size = spriteSheet.Padding + spriteSheet.Padding + bar.Image.PixelHeight;
                    }
                }

                canvasImage.Width = x_size;
                canvasImage.Height = y_size;

                listSpriteView.ItemsSource = foo.SpriteList;
            }
        }

        private void ListViewSelectionChange( object sender, SelectionChangedEventArgs e ) {
            UpdatePreview( );
        }

        private void Execute_New( object sender, ExecutedRoutedEventArgs e ) {

            if( MessageBox.Show( (string)FindResource( "strDataLossWarning1" ) + "\n" + (string)FindResource( "strDataLossWarning2" ), "Confirm?", MessageBoxButton.OKCancel, MessageBoxImage.Question ) == MessageBoxResult.OK ) {
                spriteSheet = new SpriteSheet( ) { Padding = 5 };
                canvasImage.Children.Clear( );
                listAnimView.ItemsSource = spriteSheet.AnimationList;
                listSpriteView.ItemsSource = null;
            }

            UpdatePreview( );
        }
        private void Execute_Open( object sender, ExecutedRoutedEventArgs e ) {
            OpenFileDialog openFile = new OpenFileDialog( );
            openFile.Filter = "XML File (*.xml)|*.xml";

            if( MessageBox.Show( (string)FindResource( "strDataLossWarning1" ) + "\n" + (string)FindResource( "strDataLossWarning2" ), "Confirm?", MessageBoxButton.OKCancel, MessageBoxImage.Question ) == MessageBoxResult.OK ) {
                if( openFile.ShowDialog( ) == true ) {
                    SpriteSheet newSheet = null;
                    FileStream stream = null;
                    XDocument xmlDoc = null;

                    try {
                        stream = File.OpenRead( openFile.FileName );
                        xmlDoc = XDocument.Load( stream );
                        stream.Close( );
                        stream = null;
                        newSheet = new SpriteSheet( ) {
                            Padding = Convert.ToInt32( xmlDoc.Root.Attribute( "Padding" ).Value ),
                        };

                        foreach( XElement animElement in xmlDoc.Root.Elements( "Animation" ) ) {
                            SpriteAnimation newAnim = new SpriteAnimation( ) {
                                Name = animElement.Attribute( "Name" ).Value,
                            };

                            stream = File.OpenRead( System.IO.Path.GetDirectoryName( openFile.FileName ) + "\\" + animElement.Attribute( "FileName" ).Value );
                            WriteableBitmap animImage = BitmapFactory.New( 1, 1 ).FromStream( stream );

                            foreach( XElement spriteElement in animElement.Elements( "Image" ) ) {
                                int width = Convert.ToInt32( spriteElement.Element( "Size" ).Attribute( "Width" ).Value );
                                int height = Convert.ToInt32( spriteElement.Element( "Size" ).Attribute( "Height" ).Value );
                                int x = Convert.ToInt32( spriteElement.Element( "Position" ).Attribute( "X" ).Value );
                                int y = Convert.ToInt32( spriteElement.Element( "Position" ).Attribute( "Y" ).Value );
                                WriteableBitmap spriteImage = BitmapFactory.New( width, height );
                                spriteImage.Blit( new Rect( 0, 0, width, height ), animImage, new Rect( x, y, width, height ) );

                                SpriteImage newImage = new SpriteImage( ) {
                                    Name = spriteElement.Attribute( "Name" ).Value,
                                    Image = spriteImage,
                                };

                                newAnim.SpriteList.Add( newImage );
                            }

                            newSheet.AnimationList.Add( newAnim );
                        }
                        spriteSheet = newSheet;
                        UpdatePreview( );

                        listAnimView.ItemsSource = spriteSheet.AnimationList;
                        listAnimView.SelectedIndex = spriteSheet.AnimationList.Count - 1;
                        listAnimView.Focus( );
                    }
                    catch( Exception except ) {
                        MessageBox.Show( except.Message );
                    }
                    finally {
                        if( stream != null ) {
                            stream.Close( );
                            stream = null;
                        }
                    }
                }
            }
        }
        private void Execute_Save( object sender, ExecutedRoutedEventArgs e ) {
            SaveFileDialog saveFile = new SaveFileDialog( );
            saveFile.Filter = "XML Files (*.xml)|*.xml";

            if( saveFile.ShowDialog( ) == true ) {
                string fileName = System.IO.Path.GetFileNameWithoutExtension( saveFile.FileName );
                string extension = System.IO.Path.GetExtension( saveFile.FileName );
                string folder = System.IO.Path.GetDirectoryName( saveFile.FileName );

                Console.WriteLine( "Saving..." );
                Console.WriteLine( "File: " + fileName + extension + "\nFolder: " + folder + "\\" );
                
                XDocument xmlDocument = new XDocument( );
                XElement sheet = new XElement( "SpriteSheet", new XAttribute("Padding", spriteSheet.Padding ) );

                int count = 0;

                foreach( SpriteAnimation anim in spriteSheet.AnimationList ) {
                    XElement foo = anim.ToXElement( spriteSheet.Padding );
                    string thisFile = fileName + "_" + count.ToString( ) + ".png";

                    foo.Add( new XAttribute( "FileName", thisFile ) );
                    sheet.Add( foo );

                    BitmapFrame animImage = BitmapFrame.Create( anim.BitmapExport( spriteSheet.Padding ) );
                    PngBitmapEncoder encoder = new PngBitmapEncoder( );
                    encoder.Frames.Add( animImage );

                    Directory.CreateDirectory( folder + "\\" + fileName + "\\" );
                    FileStream stream_image = null;

                    try {
                        stream_image = File.Create( folder + "\\" + fileName + "\\" + thisFile );
                        encoder.Save( stream_image );
                        Console.WriteLine( "Exported Image: " + thisFile );
                    }
                    catch( Exception except ) {
                        MessageBox.Show( except.Message );
                    }
                    finally{
                        if( stream_image != null ) {
                            stream_image.Close( );
                            stream_image = null;
                        }
                    }
                    ++count;
                }

                sheet.Add( new XAttribute( "Count", count ) );
                xmlDocument.Add( sheet );

                FileStream stream_xml = null;
                try {
                    stream_xml = File.Create( folder + "\\" + fileName + "\\" + fileName + extension );
                    xmlDocument.Save( stream_xml );
                }
                catch( Exception except ) {
                    MessageBox.Show( except.Message );
                }
                finally {
                    if( stream_xml != null ) {
                        stream_xml.Close( );
                        stream_xml = null;
                    }
                }

                Console.WriteLine( "Done Saving!" );
            }
        }
        private void Execute_Close( object sender, ExecutedRoutedEventArgs e ) {
            if( MessageBox.Show( (string)FindResource( "strDataLossWarning1" ) + "\n" + (string)FindResource( "strDataLossWarning2" ), "Confirm?", MessageBoxButton.OKCancel, MessageBoxImage.Question ) == MessageBoxResult.OK ) {
                this.Close( );
            }
        }

        private void Execute_AnimAdd( object sender, ExecutedRoutedEventArgs e ) {
            spriteSheet.AnimationList.Add( new SpriteAnimation( ) );
            listAnimView.SelectedIndex = spriteSheet.AnimationList.Count( ) - 1;
            listAnimView.Focus( );
            listSpriteView.ItemsSource = ( (SpriteAnimation)listAnimView.SelectedItem ).SpriteList;
        }
        private void Execute_AnimRename( object sender, ExecutedRoutedEventArgs e ) {
           SpriteAnimation listBoxSelected = listAnimView.SelectedItem as SpriteAnimation;
            if( listBoxSelected == null ) { return; }

            Window_Rename editWindow = new Window_Rename( );
            editWindow.DataContext = listBoxSelected;
            editWindow.ShowDialog( );
        }
        private void Execute_AnimRemove( object sender, ExecutedRoutedEventArgs e ) {
            SpriteAnimation anim = listAnimView.SelectedItem as SpriteAnimation;
            int index = listAnimView.SelectedIndex;

            if( anim != null ) {

                foreach( SpriteImage sprite in anim.SpriteList ) { sprite.Image = null; }
                anim.SpriteList.Clear( );
                spriteSheet.AnimationList.RemoveAt( index );
                --index;
            }

            UpdatePreview( );
            listAnimView.SelectedIndex = index;
            listAnimView.Focus( );
        }

        private void Execute_SpriteAdd( object sender, ExecutedRoutedEventArgs e ) {
            OpenFileDialog openFile = new OpenFileDialog( );
            openFile.Filter = "PNG Files (*.png)|*.png";
            SpriteAnimation foo = listAnimView.SelectedItem as SpriteAnimation;
            WriteableBitmap newBitmap = BitmapFactory.New( 32, 32 );
            SpriteImage newSprite = null;

            if( foo != null ) {
                if( openFile.ShowDialog( ) == true ) {
                    FileStream stream = null;

                    try {
                        stream = File.Open( openFile.FileName, FileMode.Open );
                        newBitmap = BitmapFactory.New( 1, 1 ).FromStream( stream );
                    }
                    catch( Exception except ) {
                        MessageBox.Show( except.Message, "Error opening file: " + openFile.FileName, MessageBoxButton.OK, MessageBoxImage.Error );
                        newBitmap = BitmapFactory.New( 32, 32 );
                        newBitmap.Clear( Colors.MediumPurple );
                    }
                    finally {
                        if( stream != null ) {
                            stream.Close( );
                            stream = null;
                        }
                        newSprite = new SpriteImage( ) { Image = newBitmap, Name = System.IO.Path.GetFileNameWithoutExtension( openFile.FileName ) };
                        foo.SpriteList.Add( newSprite );
                    }

                }
                else {
#if DEBUG
                    newBitmap = BitmapFactory.New( 32, 32 );
                    newBitmap.Clear( Colors.MediumPurple );
                    newSprite = new SpriteImage( ) { Image = newBitmap };
                    foo.SpriteList.Add( newSprite );
#endif
                }
            }

            UpdatePreview( );
            listSpriteView.SelectedIndex = foo.SpriteList.Count - 1;
            listSpriteView.Focus( );
        }
        private void Execute_SpriteRename( object sender, ExecutedRoutedEventArgs e ) {
            SpriteImage listBoxSelected = listSpriteView.SelectedItem as SpriteImage;
            if( listBoxSelected == null ) { return; }

            Window_Rename editWindow = new Window_Rename( );
            editWindow.DataContext = listBoxSelected;
            editWindow.ShowDialog( );
        }
        private void Execute_SpriteRemove( object sender, ExecutedRoutedEventArgs e ) {
            SpriteAnimation anim = listAnimView.SelectedItem as SpriteAnimation;
            int index = -1;

            if( anim != null ) {

                SpriteImage sprite = listSpriteView.SelectedItem as SpriteImage;
                index = listSpriteView.SelectedIndex;

                if( sprite != null ) {
                    sprite.Image = null;
                    anim.SpriteList.RemoveAt( listSpriteView.SelectedIndex );
                    --index;
                }
            }

            UpdatePreview( );
            listSpriteView.SelectedIndex = index;
            listSpriteView.Focus( );
        }
        private void Execute_SpriteMoveUp( object sender, ExecutedRoutedEventArgs e ) {
            SpriteAnimation foo = listAnimView.SelectedItem as SpriteAnimation;
            int index = -1;

            if( foo != null && spriteSheet.AnimationList.Contains( foo ) ) {

                SpriteImage bar = listSpriteView.SelectedItem as SpriteImage;
                index = listSpriteView.SelectedIndex;

                if( bar != null && foo.SpriteList.Contains( bar ) ) {
                    if( index > 0 ) {
                        foo.SpriteList.Move( index, --index );
                    }
                }
            }

            UpdatePreview( );
            listSpriteView.SelectedIndex = index;
            listSpriteView.Focus( );
        }
        private void Execute_SpriteMoveDown( object sender, ExecutedRoutedEventArgs e ) {
            SpriteAnimation foo = listAnimView.SelectedItem as SpriteAnimation;
            int index = -1;

            if( foo != null && spriteSheet.AnimationList.Contains( foo ) ) {

                SpriteImage bar = listSpriteView.SelectedItem as SpriteImage;
                index = listSpriteView.SelectedIndex;

                if( bar != null && foo.SpriteList.Contains( bar ) ) {
                    if( index < foo.SpriteList.Count - 1 ) {
                        foo.SpriteList.Move( index, ++index );
                    }
                }
            }

            UpdatePreview( );
            listSpriteView.SelectedIndex = index;
            listSpriteView.Focus( );
        }

        private void CanExecute_AnimationSelected( object sender, CanExecuteRoutedEventArgs e ) {
            e.CanExecute = false;
            if( listAnimView != null ) {
                e.CanExecute = listAnimView.SelectedItem != null;
            }
        }
        private void CanExecute_SpriteSelected( object sender, CanExecuteRoutedEventArgs e ) {
            e.CanExecute = false;
            if( listSpriteView != null ) {
                e.CanExecute = listSpriteView.SelectedItem != null;
            }
        }
        private void CanExecute_AlwaysTrue( object sender, CanExecuteRoutedEventArgs e ) {
            e.CanExecute = true;
        }
        private void CanExecute_DataExists( object sender, CanExecuteRoutedEventArgs e ) {
            bool animExits = spriteSheet.AnimationList.Count > 0; // True if any animations exist.

            bool spriteExists = false;
            foreach( SpriteAnimation anim in spriteSheet.AnimationList ) {
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

        public WriteableBitmap Image;

        // Getters for the WirteableBitmap
        public int Width {
            get {
                if( this.Image != null ) { return this.Image.PixelWidth; }
                return -1;
            }
        }
        public int Height {
            get {
                if( this.Image != null ) {  return this.Image.PixelHeight; }
                return -1;
            }
        }

        public SpriteImage( ) {
            Name = this.GetType().ToString();
            Image = null;
        }

        public XElement ToXElement( ) {
            XAttribute Xname = new XAttribute( "Name", Name );
            XElement Xsize = new XElement( "Size", new XAttribute( "Width", Width ), new XAttribute( "Height", Height ) );

            XElement Ximage = new XElement( "Image", Xname, Xsize );

            return Ximage;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged( string propertyName ) {
#if _VERBOSE
                    Console.WriteLine( "SpriteSheet.PropertyChanged : " + propertyName );
#endif
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

        public Size GetSize( int padding ) {
            Size returnSize = new Size( padding, padding );

            foreach( SpriteImage sprite in SpriteList ) {
                returnSize.Width += (double)sprite.Width + padding;
                if( returnSize.Height < padding + sprite.Height + padding ) {
                    returnSize.Height = padding + sprite.Height + padding;
                }
            }

            return returnSize;
        }

        public ObservableCollection<SpriteImage> SpriteList;

        public SpriteAnimation( ) {
            SpriteList = new ObservableCollection<SpriteImage>( );
            Name = "Animation";
        }

        public WriteableBitmap BitmapExport( int padding ) {
            WriteableBitmap export = BitmapFactory.New( (int)GetSize( padding ).Width, (int)GetSize( padding ).Height );

            int xpos = padding;

            foreach( SpriteImage source in SpriteList ) {
                export.Blit( new Rect( xpos, padding, source.Width, source.Height ), source.Image, new Rect( 0, 0, source.Width, source.Height ) );
                xpos += source.Width + padding;
            }

            return export;
        }

        public XElement ToXElement( int padding ) {
            XElement anim = new XElement( "Animation", new XAttribute( "Name", Name ) );

            int pos_x = padding;
            XElement animSprites = new XElement( "Sprites" );

            // For loops to get each image's xml
            foreach( SpriteImage sprite in SpriteList ) {
                XElement xSprite = sprite.ToXElement( );
                XElement pos = new XElement( "Position", new XAttribute( "X", pos_x ), new XAttribute( "Y", padding ) );
                pos_x += sprite.Width + padding;
                xSprite.Add( pos );
                animSprites.Add( xSprite );
            }

            animSprites.Add( new XAttribute( "Count", SpriteList.Count ) );
            XElement size = new XElement( "Size" );
            size.Add( new XAttribute( "Width", GetSize( padding ).Width ), new XAttribute( "Height", GetSize( padding ).Height ) );
            anim.Add( size, animSprites );

            return anim;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged( string propertyName ) {
#if _VERBOSE
                    Console.WriteLine( "SpriteImage.PropertyChanged : " + propertyName );
#endif
            if( PropertyChanged != null ) { PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) ); }
        }

    }

    public class SpriteSheet {
        public ObservableCollection<SpriteAnimation> AnimationList { set; get; }

        public int Padding { set; get; }

        public SpriteSheet( ) {
            AnimationList = new ObservableCollection<SpriteAnimation>( );
            Padding = 0;
        }
    }

}