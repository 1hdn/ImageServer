using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ImageServer.Tests
{
    public class UrlParserTests
    {
        [Fact]
        public void TestInvalidUrl()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            Assert.Throws<UrlParserException>(() => 
            {
                urlParser.Parse(new Uri("http://www.test.com/images/100x50-/test.jpg"));
            });
        }

        [Fact]
        public void TestMinimumUrl()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            UrlParserResult parserResult = urlParser.Parse(new Uri("http://www.test.com/images/100x50/test.png"));
            Assert.True(parserResult.Width == 100);
            Assert.True(parserResult.Height == 50);
            Assert.Null(parserResult.ResizeMethod);
            Assert.Null(parserResult.Quality);
            Assert.Null(parserResult.Compression);
            Assert.Null(parserResult.PostProcess);
        }

        [Fact]
        public void TestCommonUrl()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            UrlParserResult parserResult = urlParser.Parse(new Uri("http://www.test.com/images/100x50-scaledown/test.jpg"));
            Assert.True(parserResult.Width == 100);
            Assert.True(parserResult.Height == 50);
            Assert.True(parserResult.ResizeMethod == ResizeMethod.ScaleDown);
            Assert.Null(parserResult.Quality);
            Assert.Null(parserResult.Compression);
            Assert.Null(parserResult.PostProcess);
        }

        [Fact]
        public void TestFullUrl()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            UrlParserResult parserResult = urlParser.Parse(new Uri("http://www.test.com/images/100x50-cover-q32-c4-npp/test.gif"));
            Assert.True(parserResult.Width == 100);
            Assert.True(parserResult.Height == 50);
            Assert.True(parserResult.ResizeMethod == ResizeMethod.Cover);
            Assert.True(parserResult.Quality == 32);
            Assert.True(parserResult.Compression == 4);
            Assert.False(parserResult.PostProcess);
        }

        [Fact]
        public void TestFullUrlRandomOrder()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            UrlParserResult parserResult = urlParser.Parse(new Uri("http://www.test.com/images/100x50-npp-q32-c4-cover/test.jpeg"));
            Assert.True(parserResult.Width == 100);
            Assert.True(parserResult.Height == 50);
            Assert.True(parserResult.ResizeMethod == ResizeMethod.Cover);
            Assert.True(parserResult.Quality == 32);
            Assert.True(parserResult.Compression == 4);
            Assert.False(parserResult.PostProcess);
        }

        [Fact]
        public void TestFill()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            UrlParserResult parserResult = urlParser.Parse(new Uri("http://www.test.com/images/100x50-fill/test.jpeg"));
            Assert.True(parserResult.ResizeMethod == ResizeMethod.Fill);
        }

        [Fact]
        public void TestCover()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            UrlParserResult parserResult = urlParser.Parse(new Uri("http://www.test.com/images/100x50-cover/test.jpeg"));
            Assert.True(parserResult.ResizeMethod == ResizeMethod.Cover);
        }

        [Fact]
        public void TestContain()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            UrlParserResult parserResult = urlParser.Parse(new Uri("http://www.test.com/images/100x50-contain/test.jpeg"));
            Assert.True(parserResult.ResizeMethod == ResizeMethod.Contain);
        }

        [Fact]
        public void TestScaleDown()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            UrlParserResult parserResult = urlParser.Parse(new Uri("http://www.test.com/images/100x50-scaledown/test.jpeg"));
            Assert.True(parserResult.ResizeMethod == ResizeMethod.ScaleDown);
        }

        [Fact]
        public void TestUnknownModifier()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            Assert.Throws<UrlParserException>(() => 
            {
                urlParser.Parse(new Uri("http://www.test.com/images/100x50-elastic/test.jpeg"));
            });
        }

        [Fact]
        public void TestPostProcess()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            UrlParserResult parserResult = urlParser.Parse(new Uri("http://www.test.com/images/100x50-pp/test.jpeg"));
            Assert.True(parserResult.PostProcess);
        }

        [Fact]
        public void TestNoPostProcess()
        {
            UrlParser urlParser = new UrlParser("/source", new Location(new Uri("http://www.test.com/images"), "/destination"));
            UrlParserResult parserResult = urlParser.Parse(new Uri("http://www.test.com/images/100x50-npp/test.jpeg"));
            Assert.False(parserResult.PostProcess);
        }

        [Fact]
        public void TestRemoteSourceAndDestination()
        {
            UrlParser urlParser;
            UrlParserResult parserResult;

            urlParser = new UrlParser("http://site1.com/images", new Location(new Uri("http://site2.com/img/"), @"c:\inetpub\site2.com\img"));
            parserResult = urlParser.Parse(new Uri("http://site2.com/img/400x400/test.jpg"));
            Assert.True(parserResult.Source == "http://site1.com/images/test.jpg");
            Assert.True(parserResult.Destination == @"c:\inetpub\site2.com\img\400x400\test.jpg");
            Assert.True(parserResult.ImageType == ImageType.Jpeg);

            urlParser = new UrlParser("http://site1.com/images/", new Location(new Uri("http://site2.com/img"), @"c:\inetpub\site2.com\img\"));
            parserResult = urlParser.Parse(new Uri("http://site2.com/img/path/400x400/test.jpeg"));
            Assert.True(parserResult.Source == "http://site1.com/images/path/test.jpeg");
            Assert.True(parserResult.Destination == @"c:\inetpub\site2.com\img\path\400x400\test.jpeg");
            Assert.True(parserResult.ImageType == ImageType.Jpeg);

            urlParser = new UrlParser("http://site1.com/IMAGES", new Location(new Uri("http://site2.com/Img"), @"c:\Inetpub\Site2.com\Img"));
            parserResult = urlParser.Parse(new Uri("http://site2.com/Img/path/to/IMAGE/400x400/test.gif"));
            Assert.True(parserResult.Source == "http://site1.com/IMAGES/path/to/IMAGE/test.gif");
            Assert.True(parserResult.Destination == @"c:\Inetpub\Site2.com\Img\path\to\IMAGE\400x400\test.gif");
            Assert.True(parserResult.ImageType == ImageType.Gif);

            urlParser = new UrlParser("https://site1.com/images", new Location(new Uri("https://site2.com/img"), @"/var/www/site2.com/img"));
            parserResult = urlParser.Parse(new Uri("https://site2.com/img/400x400/test.png"));
            Assert.True(parserResult.Source == "https://site1.com/images/test.png");
            Assert.True(parserResult.Destination == @"/var/www/site2.com/img/400x400/test.png");
            Assert.True(parserResult.ImageType == ImageType.Png);

            urlParser = new UrlParser("https://site1.com/images/", new Location(new Uri("https://site2.com/img/"), @"/var/www/site2.com/img/"));
            parserResult = urlParser.Parse(new Uri("https://site2.com/img/path/400x400/test.jpg"));
            Assert.True(parserResult.Source == "https://site1.com/images/path/test.jpg");
            Assert.True(parserResult.Destination == @"/var/www/site2.com/img/path/400x400/test.jpg");
            Assert.True(parserResult.ImageType == ImageType.Jpeg);

            urlParser = new UrlParser("https://site1.com/IMAGES", new Location(new Uri("https://site2.com/Img"), @"/var/www/Site2.com/Img"));
            parserResult = urlParser.Parse(new Uri("https://site2.com/Img/path/to/IMAGE/400x400/test.jpg"));
            Assert.True(parserResult.Source == "https://site1.com/IMAGES/path/to/IMAGE/test.jpg");
            Assert.True(parserResult.Destination == @"/var/www/Site2.com/Img/path/to/IMAGE/400x400/test.jpg");
            Assert.True(parserResult.ImageType == ImageType.Jpeg);

            Assert.Throws<UrlParserException>(() =>
            {
                parserResult = urlParser.Parse(new Uri("https://site2.com/incorrect/path/img/400x400/test.jpg"));
            });
        }

        [Fact]
        public void TestLocalSourceAndDestination()
        {
            UrlParser urlParser;
            UrlParserResult parserResult;

            urlParser = new UrlParser(@"c:\source", new Location(new Uri("http://site2.com/img/"), @"c:\inetpub\site2.com\img"));
            parserResult = urlParser.Parse(new Uri("http://site2.com/img/400x400/test.jpg"));
            Assert.True(parserResult.Source == @"c:\source\test.jpg");
            Assert.True(parserResult.Destination == @"c:\inetpub\site2.com\img\400x400\test.jpg");
            Assert.True(parserResult.ImageType == ImageType.Jpeg);

            urlParser = new UrlParser(@"c:\source\", new Location(new Uri("http://site2.com/img"), @"c:\inetpub\site2.com\img\"));
            parserResult = urlParser.Parse(new Uri("http://site2.com/img/path/400x400/test.jpeg"));
            Assert.True(parserResult.Source == @"c:\source\path\test.jpeg");
            Assert.True(parserResult.Destination == @"c:\inetpub\site2.com\img\path\400x400\test.jpeg");
            Assert.True(parserResult.ImageType == ImageType.Jpeg);

            urlParser = new UrlParser(@"C:\Source", new Location(new Uri("http://site2.com/Img"), @"c:\Inetpub\Site2.com\Img"));
            parserResult = urlParser.Parse(new Uri("http://site2.com/Img/path/to/IMAGE/400x400/test.gif"));
            Assert.True(parserResult.Source == @"C:\Source\path\to\IMAGE\test.gif");
            Assert.True(parserResult.Destination == @"c:\Inetpub\Site2.com\Img\path\to\IMAGE\400x400\test.gif");
            Assert.True(parserResult.ImageType == ImageType.Gif);

            urlParser = new UrlParser("/var/source", new Location(new Uri("https://site2.com/img"), @"/var/www/site2.com/img"));
            parserResult = urlParser.Parse(new Uri("https://site2.com/img/400x400/test.png"));
            Assert.True(parserResult.Source == "/var/source/test.png");
            Assert.True(parserResult.Destination == @"/var/www/site2.com/img/400x400/test.png");
            Assert.True(parserResult.ImageType == ImageType.Png);

            urlParser = new UrlParser("/var/source/", new Location(new Uri("https://site2.com/img/"), @"/var/www/site2.com/img/"));
            parserResult = urlParser.Parse(new Uri("https://site2.com/img/path/400x400/test.jpg"));
            Assert.True(parserResult.Source == "/var/source/path/test.jpg");
            Assert.True(parserResult.Destination == @"/var/www/site2.com/img/path/400x400/test.jpg");
            Assert.True(parserResult.ImageType == ImageType.Jpeg);

            urlParser = new UrlParser("/var/SOURCE", new Location(new Uri("https://site2.com/Img"), @"/var/www/Site2.com/Img"));
            parserResult = urlParser.Parse(new Uri("https://site2.com/Img/path/to/IMAGE/400x400/test.jpg"));
            Assert.True(parserResult.Source == "/var/SOURCE/path/to/IMAGE/test.jpg");
            Assert.True(parserResult.Destination == @"/var/www/Site2.com/Img/path/to/IMAGE/400x400/test.jpg");
            Assert.True(parserResult.ImageType == ImageType.Jpeg);

            Assert.Throws<UrlParserException>(() =>
            {
                parserResult = urlParser.Parse(new Uri("https://site2.com/incorrect/path/img/400x400/test.jpg"));
            });
        }
    }
}
