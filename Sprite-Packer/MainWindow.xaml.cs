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
        public SpriteSheet SpriteSheet { set; get; }

        public MainWindow( ) {
            SpriteSheet = new SpriteSheet( ) { Padding = 5 };
            DataContext = this;

            InitializeComponent( );

            listAnimView.ItemsSource = SpriteSheet.AnimationList;
            listAnimView.SelectedIndex = SpriteSheet.AnimationList.Count - 1;
            listAnimView.Focus( );
        }

        public void UpdatePreview( object sender = null, RoutedEventArgs e = null ) {
            SpriteAnimation foo = listAnimView.SelectedItem as SpriteAnimation;

            if( foo != null ) {

                canvasImage.Children.Clear( );

                double x_size = SpriteSheet.Padding;
                double y_size = SpriteSheet.Padding;

                foreach( SpriteImage bar in foo.SpriteList ) {
                    Image foobar = new Image( );
                    foobar.Source = bar.Image;

                    Canvas.SetTop( foobar, SpriteSheet.Padding );
                    Canvas.SetLeft( foobar, x_size );

                    canvasImage.Children.Add( foobar );

                    x_size += bar.Image.PixelWidth + SpriteSheet.Padding;
                    if( y_size < SpriteSheet.Padding + SpriteSheet.Padding + bar.Image.PixelHeight ) {
                        y_size = SpriteSheet.Padding + SpriteSheet.Padding + bar.Image.PixelHeight;
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
                SpriteSheet = new SpriteSheet( ) { Padding = 5 };
                canvasImage.Children.Clear( );
                listAnimView.ItemsSource = SpriteSheet.AnimationList;
                listSpriteView.ItemsSource = null;
            }

            UpdatePreview( );
        }
        private void Execute_Open( object sender, ExecutedRoutedEventArgs e ) {
            OpenFileDialog openFile = new OpenFileDialog( );
            openFile.Filter = "XML File (*.xml)|*.xml";

            if( MessageBox.Show( (string)FindResource( "strDataLossWarning1" ) + "\n" + (string)FindResource( "strDataLossWarning2" ), "Confirm?", MessageBoxButton.OKCancel, MessageBoxImage.Question ) == MessageBoxResult.OK ) {
                if( openFile.ShowDialog( ) == true ) {
                    XDocument xmlDoc = XDocument.Load( openFile.FileName );
                    SpriteSheet = new SpriteSheet( xmlDoc.Root, System.IO.Path.GetDirectoryName( openFile.FileName ) + "\\" );

                    UpdatePreview( );

                    listAnimView.ItemsSource = SpriteSheet.AnimationList;
                    if( SpriteSheet.AnimationList.Count > 0 ) {
                        listAnimView.SelectedIndex = SpriteSheet.AnimationList.Count - 1;
                    }
                    listAnimView.Focus( );
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

                XElement sheet = SpriteSheet.ToXML( fileName );
                XDocument xmlDocument = new XDocument( sheet );
                xmlDocument.Save( saveFile.FileName, SaveOptions.None );

                for( int i = 0; i < SpriteSheet.AnimationList.Count; ++i ) {
                    string thisFile = fileName + "_" + i.ToString( ) + ".png";
                    WriteableBitmap animImage = SpriteSheet.AnimationList[i].BitmapExport( SpriteSheet.Padding );

                    FileStream stream = null;

                    try {
                        BitmapFrame animBitmap = BitmapFrame.Create( animImage );
                        PngBitmapEncoder encoder = new PngBitmapEncoder( );
                        encoder.Frames.Add( animBitmap );

                        stream = File.Create( folder + "\\" + thisFile );

                        encoder.Save( stream );

                        Console.WriteLine( "Exported Image: " + thisFile );
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
                
                Console.WriteLine( "Done Saving!" );
            }
        }
        private void Execute_Close( object sender, ExecutedRoutedEventArgs e ) {
            if( MessageBox.Show( (string)FindResource( "strDataLossWarning1" ) + "\n" + (string)FindResource( "strDataLossWarning2" ), "Confirm?", MessageBoxButton.OKCancel, MessageBoxImage.Question ) == MessageBoxResult.OK ) {
                this.Close( );
            }
        }

        private void Execute_AnimAdd( object sender, ExecutedRoutedEventArgs e ) {
            SpriteSheet.AnimationList.Add( new SpriteAnimation( ) );
            listAnimView.SelectedIndex = SpriteSheet.AnimationList.Count( ) - 1;
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
                SpriteSheet.AnimationList.RemoveAt( index );
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

                UpdatePreview( );
                if( foo.SpriteList.Count > 0 ) {
                    listSpriteView.SelectedIndex = foo.SpriteList.Count - 1;
                }
                listSpriteView.Focus( );
            }
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

            if( foo != null && SpriteSheet.AnimationList.Contains( foo ) ) {

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

            if( foo != null && SpriteSheet.AnimationList.Contains( foo ) ) {

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

        public SpriteImage( XElement spriteXML, WriteableBitmap animationImage ) {
            Name = spriteXML.Attribute( "Name" ).Value;

            Rect shape = new Rect( Convert.ToDouble( spriteXML.Element( "Position" ).Attribute( "X" ).Value ),
                                   Convert.ToDouble( spriteXML.Element( "Position" ).Attribute( "Y" ).Value ),
                                   Convert.ToDouble( spriteXML.Element( "Size" ).Attribute( "Width" ).Value ),
                                   Convert.ToDouble( spriteXML.Element( "Size" ).Attribute( "Height" ).Value ) );

            Image = BitmapFactory.New( (int)shape.Width, (int)shape.Height );
            Image.Blit( new Rect( 0, 0, (int)shape.Width, (int)shape.Height ), animationImage, shape );
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
            Name = "Animation";
            SpriteList = new ObservableCollection<SpriteImage>( );
        }

        public SpriteAnimation( XElement animationXML, string fileDir ) {
            Name = animationXML.Attribute( "Name" ).Value;
            SpriteList = new ObservableCollection<SpriteImage>( );

            WriteableBitmap animImage = null;
            FileStream animImage_Stream = null;

            try {
                animImage_Stream = File.Open( fileDir + "\\" + animationXML.Attribute( "FileName" ).Value, FileMode.Open, FileAccess.Read );
                animImage = BitmapFactory.New( 1, 1 ).FromStream( animImage_Stream );
            }
            catch {
                if( animImage == null ) {
                    int width = Convert.ToInt32( animationXML.Element( "Size" ).Attribute( "Width" ).Value );
                    int height = Convert.ToInt32( animationXML.Element( "Size" ).Attribute( "Height" ).Value );
                    animImage = BitmapFactory.New( width, height );
                    animImage.Clear( Colors.Purple );
                }
            }
            finally {
                if( animImage_Stream != null ) {
                    animImage_Stream.Close( );
                    animImage_Stream = null;
                }
            }

            foreach( XElement image in animationXML.Elements( "Image" ) ) {
                SpriteImage newSprite = new SpriteImage( image, animImage );
                SpriteList.Add( newSprite );
            }
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

            // For loops to get each image's xml
            foreach( SpriteImage sprite in SpriteList ) {
                XElement xSprite = sprite.ToXElement( );
                XElement pos = new XElement( "Position", new XAttribute( "X", pos_x ), new XAttribute( "Y", padding ) );
                pos_x += sprite.Width + padding;
                xSprite.Add( pos );
                anim.Add( xSprite );
            }

            anim.Add( new XAttribute( "Count", SpriteList.Count ) );
            XElement size = new XElement( "Size" );
            size.Add( new XAttribute( "Width", GetSize( padding ).Width ), new XAttribute( "Height", GetSize( padding ).Height ) );
            anim.Add( size );

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

        public SpriteSheet( XElement sheetXML, string uri ) {
            Padding = Convert.ToInt32( sheetXML.Attribute( "Padding" ).Value );
            AnimationList = new ObservableCollection<SpriteAnimation>( );

            foreach( XElement animXML in sheetXML.Elements( "Animation" ) ) {
                SpriteAnimation newAnim = new SpriteAnimation( animXML, System.IO.Path.GetDirectoryName( uri ) );
                AnimationList.Add( newAnim );
            }
        }

        public XElement ToXML( string fileName ) {
            XElement sheet = new XElement( "SpriteSheet" );
            sheet.Add( new XAttribute( "Count", AnimationList.Count ) ); 

            for( int i = 0; i < AnimationList.Count; ++i ) {
                XElement animXML = AnimationList[i].ToXElement( Padding );
                animXML.Add( new XAttribute( "FileName", fileName + "_" + i.ToString( ) + ".png" ) );
                sheet.Add( animXML );
            }

            return sheet;
        }
    }

}