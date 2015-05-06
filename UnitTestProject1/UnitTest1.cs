#define _VERBOSE

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sprite_Packer;
using System.Windows.Media.Imaging; // Writeable Bitmap
using System.Linq;
using System.Xml.Linq;

namespace UnitTestProject1 {

    [TestClass( )]
    public class SpriteImage_Testing {

        [TestCategory( "SpriteImage" ), TestCategory( "Constructor" ), TestMethod( )]
        public void SpriteImage_Construct( ) {
            SpriteImage image = null;

            // Test default constructor.
            image = new SpriteImage( );

            Assert.AreEqual( typeof( SpriteImage ).ToString( ), image.Name, "image.Name should construct to the name of the class." );
            Assert.IsNull( image.Image, "image.Image should construct to null" );
            Assert.AreEqual( -1, image.Width, "image.Width should return -1 if image.Image is null" );
            Assert.AreEqual( -1, image.Height, "image.Height should return -1 if image.Image is null" );

            // Test object initializer.
            image = new SpriteImage( ) { Name = "TEST", Image = BitmapFactory.New( 32, 64 ) };

            Assert.AreEqual( "TEST", image.Name, "image.Name should have been set to the value provided in the in-line constructor" );
            Assert.IsNotNull( image.Image, "image.Image should have been set to the value provided in the in-line constructor" );
            Assert.AreEqual( 32, image.Width, "image.Width should return the width of the image it contains" );
            Assert.AreEqual( 64, image.Height, "image.Height should return the height of the image it contains" );

            // Test xml constructor
            XElement spriteXML = new XElement("Image",
                                 new XAttribute("Name", "Test"),
                                 new XElement("Size", new XAttribute("Width", 32), new XAttribute("Height",64)),
                                 new XElement("Position", new XAttribute("X", 1), new XAttribute("Y",2)));

            image = new SpriteImage( spriteXML, BitmapFactory.New( 128, 128 ) );

            Assert.AreEqual( "Test", image.Name, "image.Name should have been set to the value provided in the xml constructor" );
            Assert.IsNotNull( image.Image, "image.Image should have been set to the value provided in the xml constructor" );
            Assert.AreEqual( 32, image.Width, "image.Width should return the width of the image it contains" );
            Assert.AreEqual( 64, image.Height, "image.Height should return the height of the image it contains" );
        }

        [TestCategory( "SpriteImage" ), TestCategory( "XML" ), TestMethod( )]
        public void SpriteImage_XML( ) {
            XElement expected = new XElement( "Image" );
            expected.Add( new XAttribute( "Name", "TEST" ) );
            expected.Add( new XElement( "Size",
                new XAttribute( "Width", 32 ),
                new XAttribute( "Height", 64 ) ) );
            SpriteImage image = new SpriteImage( ) { Name = "TEST", Image = BitmapFactory.New( 32, 64 ) };
            XElement actual = image.ToXElement( );

            // XML does not have equality. Must manually check.
            Assert.AreEqual( expected.Attribute( "Name" ).Value, actual.Attribute( "Name" ).Value, "Name check failed" );
            Assert.AreEqual( expected.Element( "Size" ).Attribute( "Width" ).Value, actual.Element( "Size" ).Attribute( "Width" ).Value, "Width check failed" );
            Assert.AreEqual( expected.Element( "Size" ).Attribute( "Height" ).Value, actual.Element( "Size" ).Attribute( "Height" ).Value, "Height check failed" );
        }
    }

    [TestClass( )]
    public class SpriteAnimation_Testing {

        [TestCategory( "SpriteAnimation" ), TestCategory( "Constructor" ), TestMethod( )]
        public void SpriteAnimation_Construct( ) {
            SpriteAnimation anim = new SpriteAnimation( ) { Name = "TEST" };
            Assert.AreEqual( "TEST", anim.Name, "anim.Name failed to construct" );
            Assert.AreNotEqual( null, anim.SpriteList, "anim.SpriteList failed to construct" );
            Assert.AreEqual( 0, anim.SpriteList.Count, "anim.SpriteList.Count should construct to 0" );
        }

        [TestCategory( "SpriteAnimation" ), TestCategory( "XML" ), TestMethod( )]
        public void SpriteAnimation_XML( ) {
            int padding = 5;
            XElement expected = new XElement( "Animation" );
            expected.Add( new XAttribute( "Name", "TEST" ) );
            expected.Add( new XAttribute( "Count", 0 ) );
            XElement animSprites = new XElement( "Sprites" );
            expected.Add( animSprites );
            SpriteAnimation anim = new SpriteAnimation( ) { Name = "TEST" };
            XElement actual = anim.ToXElement( padding );

            // XML does not have equality. Must manually check.
            Assert.AreEqual( expected.Attribute( "Name" ).Value, actual.Attribute( "Name" ).Value, "Name check failed" );
            Assert.AreEqual( expected.Attribute( "Count" ).Value, actual.Attribute( "Count" ).Value, "Count check failed" );

            // Add a sprite to the animation
            expected = new XElement( "Animation" );
            expected.Add( new XAttribute( "Name", "TEST" ) );
            expected.Add( new XAttribute( "Count", 1 ) );
            animSprites = new XElement( "Sprites" );
            expected.Add( animSprites );
            anim.SpriteList.Add( new SpriteImage( ) { Name = "TEST", Image = BitmapFactory.New( 32, 64 ) } );
            actual = anim.ToXElement( padding );
            Assert.AreEqual( expected.Attribute( "Count" ).Value, actual.Attribute( "Count" ).Value, "Count check failed" );

            // Test XML constructor
        }
    }

    [TestClass( )]
    public class SpriteSheet_Testing {

        [TestCategory( "SpriteSheet" ), TestCategory( "Constructor" ), TestMethod( )]
        public void SpriteSheet_Construct( ) {
            SpriteSheet sheet = new SpriteSheet( ) { Padding = 5 };
            Assert.AreEqual( 5, sheet.Padding, "sheet.Padding failed to construct" );
            Assert.AreNotEqual( null, sheet.AnimationList, "sheet.AnimationList failed to construct" );
            Assert.AreEqual( 0, sheet.AnimationList.Count, "anim.SpriteList.Count should construct to 0" );
        }
    }
}
