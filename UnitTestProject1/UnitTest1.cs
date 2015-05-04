#define _VERBOSE

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sprite_Packer;
using System.Windows.Media.Imaging; // Writeable Bitmap
using System.Linq;
using System.Xml.Linq;

namespace UnitTestProject1 {
    [TestClass]
    public class UnitTest1 {
        [TestCategory( "SpriteImage" ), TestCategory( "Constructor" ), TestMethod( )]
        public void SpriteImage_Construct( ) {
            SpriteImage image = new SpriteImage( ) { Name = "TEST", Image = BitmapFactory.New( 32, 64 ) };
            Assert.AreEqual( "TEST", image.Name, "image.Name failed to construct" );
            Assert.AreEqual( 32, image.Width, "image.Width failed to construct" );
            Assert.AreEqual( 64, image.Height, "image.Height failed to construct" );
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
            SpriteAnimation anim = new SpriteAnimation( ) { Name = "TEST" };
            XElement actual = anim.ToXElement( padding );

            // XML does not have equality. Must manually check.
            Assert.AreEqual( expected.Attribute( "Name" ).Value, actual.Attribute( "Name" ).Value, "Name check failed" );
            Assert.AreEqual( expected.Attribute( "Count" ).Value, actual.Attribute( "Count" ).Value, "Count check failed" );
        }

        [TestCategory( "SpriteSheet" ), TestCategory( "Constructor" ), TestMethod( )]
        public void SpriteSheet_Construct( ) {
            SpriteSheet sheet = new SpriteSheet( ) { Padding = 5 };
            Assert.AreEqual( 5, sheet.Padding, "sheet.Padding failed to construct" );
            Assert.AreNotEqual( null, sheet.AnimationList, "sheet.AnimationList failed to construct" );
            Assert.AreEqual( 0, sheet.AnimationList.Count, "anim.SpriteList.Count should construct to 0" );
        }
    }
}
