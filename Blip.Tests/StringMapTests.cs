using Blip.Diagram.Components;
using Blip.Diagram.Styles;
using Blip.Format;
using Blip.Transform;

namespace Blip.Tests;

[TestFixture]
public class StringMapTests {
    [SetUp]
    public void Setup() { }

    [TestFixture]
    public class ToStringTests {
        [Test]
        public void SingleLine() {
            // Don't assert on the contents here; we're not creating anything.
            var width = 10;

            Assert.That(new StringMap(width, 1).ToString(), Has.Length.EqualTo(width));
        }

        [Test]
        public void NoLines() {
            var width = 10;

            Assert.That(new StringMap(width, 0).ToString(), Has.Length.EqualTo(0));
        }

        [Test]
        public void MultipleDegenerateLines() {
            var height = 10;

            Assert.That(new StringMap(0, height).ToString(), Has.Length.EqualTo(0));
        }

        [Test]
        public void MultipleSingletonLines() {
            var height = 10;

            Assert.That(new StringMap(1, height).ToString(), Has.Length.EqualTo(2 * height - 1));
        }

        [Test]
        public void NoLinesOrWidth() {
            Assert.That(new StringMap(0, 0).ToString(), Has.Length.EqualTo(0));
        }

        [TestFixture]
        public class FromLineDelimitedStringTests {
            [Test]
            public void SingleLineWithData() {
                var expected = "abcdef";

                Assert.That(StringMap.FromLineDelimitedString(expected).ToString(), Is.EqualTo(expected));
            }

            [Test]
            public void MultipleLinesWithData() {
                var expected = "abcdef\nghijkl\nmnopqr";

                Assert.That(StringMap.FromLineDelimitedString(expected).ToString(), Is.EqualTo(expected));
            }

            [Test]
            public void MultipleLinesMismatchedLengths() {
                var attempt = "abcd\nef";

                Assert.Throws<ArgumentException>(() => StringMap.FromLineDelimitedString(attempt));
            }

            [Test]
            public void MultipleEmptyLines() {
                var expected = "\n\n\n";

                Assert.That(StringMap.FromLineDelimitedString(expected).ToString(), Is.EqualTo(string.Empty));
            }

            [Test]
            public void DegenerateStringMap() {
                Assert.That(new StringMap(0, 0).ToString(), Has.Length.EqualTo(0));
            }
        }
    }

    [TestFixture]
    [TestFixtureSource(nameof(SetAndGetCharTestsSource))]
    public class SetAndGetCharTests(int width, int height) {
        [SetUp]
        public void Setup() {
            this.map = new StringMap(this.width, this.height);
        }

        private readonly int width = width;
        private readonly int height = height;

        private StringMap map;

        private static object[] SetAndGetCharTestsSource = {
            new object[] { 15, 10 },
            new object[] { 10, 10 },
            new object[] { 10, 15 }
        };

        [Test]
        public void CanSetAndGetValidCharacterInEveryValidPosition() {
            var expected = 'a';

            for (var i = 0; i < this.width * this.height; i++) {
                int x = i % this.width;
                int y = i / this.width;

                this.map.SetChar(expected, x, y);
                Assert.That(this.map.GetChar(x, y), Is.EqualTo(expected));
            }

            Assert.That(this.map.ToString(), Has.Exactly(this.width * this.height).EqualTo(expected));
        }

        [Test]
        public void XCoordinatePrecedesStart_Throws() {
            int x = -1;

            Assert.Throws<ArgumentOutOfRangeException>(() => this.map.GetChar(x, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => this.map.SetChar('a', x, 0));
        }

        [Test]
        public void XCoordinateExceedsEnd_Throws() {
            int x = this.width;

            Assert.Throws<ArgumentOutOfRangeException>(() => this.map.GetChar(x, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => this.map.SetChar('a', x, 0));
        }

        [Test]
        public void YCoordinateExceedsTop_Throws() {
            int y = -1;

            Assert.Throws<ArgumentOutOfRangeException>(() => this.map.GetChar(0, y));
            Assert.Throws<ArgumentOutOfRangeException>(() => this.map.SetChar('a', 0, y));
        }

        [Test]
        public void YCoordinateExceedsBottom_Throws() {
            int y = this.height;

            Assert.Throws<ArgumentOutOfRangeException>(() => this.map.GetChar(0, y));
            Assert.Throws<ArgumentOutOfRangeException>(() => this.map.SetChar('a', 0, y));
        }
    }

    [TestFixture]
    [TestFixtureSource(nameof(FillRectangleTestsSource))]
    public class FillRectangleTests(int width, int height) {
        [SetUp]
        public void Setup() {
            this.map = new StringMap(this.width, this.height);
        }

        private readonly int width = width;
        private readonly int height = height;

        private static object[] FillRectangleTestsSource = {
            new object[] { 15, 10 },
            new object[] { 10, 10 },
            new object[] { 10, 15 }
        };

        private StringMap map;

        [Test]
        public void FillEntireStringMap() {
            var expected = 'x';
            Assert.That(this.map.FillRectangle(expected, 0, 0, this.width, this.height).ToString(),
                Has.Exactly(this.width * this.height).EqualTo(expected));
        }

        [Test]
        public void ZeroWidthFillsNoCharacters() {
            var expected = 'x';
            Assert.That(this.map.FillRectangle(expected, 0, 0, 0, this.height).ToString(),
                Has.Exactly(0).EqualTo(expected));
        }

        [Test]
        public void ZeroHeightFillsNoCharacters() {
            var expected = 'x';
            Assert.That(this.map.FillRectangle(expected, 0, 0, this.width, 0).ToString(),
                Has.Exactly(0).EqualTo(expected));
        }

        [Test]
        public void OneWidthFillsFirstColumn() {
            var expected = 'x';
            Assert.That(this.map.FillRectangle(expected, 0, 0, 1, this.height).ToString(),
                Has.Exactly(this.height).EqualTo(expected));
            for (var y = 0; y < this.height; y++) {
                Assert.That(this.map.GetChar(0, y), Is.EqualTo(expected));
            }
        }

        [Test]
        public void OneHeightFillsFirstRow() {
            var expected = 'x';
            Assert.That(this.map.FillRectangle(expected, 0, 0, this.width, 1).ToString(),
                Has.Exactly(this.width).EqualTo(expected));
            for (var x = 0; x < this.width; x++) {
                Assert.That(this.map.GetChar(x, 0), Is.EqualTo(expected));
            }
        }

        [Test]
        public void CanCreateCentredRectangle() {
            var border = 3;
            int startX = border, startY = border, endX = this.width - 2 * border, endY = this.height - 2 * border;
            var expected = 'x';

            Assert.That(this.map.FillRectangle(expected, startX, startY, endX, endY).ToString(), Has.Exactly(
                (this.width - 2 * border) * (this.height - 2 * border)).EqualTo(expected));

            for (int y = startY; y < endY; y++)
            for (int x = startX; x < endX; x++) {
                Assert.That(this.map.GetChar(x, y), Is.EqualTo(expected));
            }
        }

        [Test]
        public void FillExceedingBoundaries() {
            var expected = 'x';
            Assert.That(this.map.FillRectangle(expected, -1, -1, this.width + 1, this.height + 1).ToString(),
                Has.Exactly(this.width * this.height).EqualTo(expected));
        }
    }

    [TestFixture]
    [TestFixtureSource(nameof(DrawStringMapTestsSource))]
    public class DrawStringMapTests(int width, int height) {
        [SetUp]
        public void Setup() {
            string line = string.Join("", Enumerable.Repeat(this.filler, this.targetWidth));
            string grid = string.Join("\n", Enumerable.Repeat(line, this.targetHeight));

            this.sourceMap = StringMap.FromLineDelimitedString(grid);
            this.targetMap = new StringMap(this.targetWidth, this.targetHeight);
        }

        private readonly int targetWidth = width;
        private readonly int targetHeight = height;
        private readonly char filler = 'a';

        private StringMap sourceMap;
        private StringMap targetMap;

        private readonly IDrawTransform transform = new OriginOnlyTransform();

        private static object[] DrawStringMapTestsSource = {
            new object[] { 15, 10 },
            new object[] { 10, 10 },
            new object[] { 10, 15 }
        };

        [Test]
        public void CanDrawSourceMapUncropped() {
            this.targetMap.DrawStringMap(this.sourceMap, 0, 0);

            Assert.That(this.targetMap.ToString(), Is.EqualTo(this.sourceMap.ToString()));
        }

        [Test]
        public void CanDrawSourceMapCroppedToZeroWidth() {
            var emptyMap = new StringMap(this.targetWidth, this.targetHeight);

            this.targetMap.DrawStringMap(this.sourceMap, this.transform, 0, 0, 0, this.targetHeight);

            Assert.That(this.targetMap.ToString(), Is.EqualTo(emptyMap.ToString()));
        }

        [Test]
        public void CanDrawSourceMapCroppedToZeroHeight() {
            var emptyMap = new StringMap(this.targetWidth, this.targetHeight);

            this.targetMap.DrawStringMap(this.sourceMap, this.transform, 0, 0, this.targetWidth, 0);

            Assert.That(this.targetMap.ToString(), Is.EqualTo(emptyMap.ToString()));
        }

        [Test]
        public void CanDrawHalfColumnsByCropFromSourceMap() {
            int cols = this.targetWidth / 2;
            int rem = this.targetWidth - cols;

            this.targetMap.DrawStringMap(this.sourceMap, this.transform, 0, 0, cols, this.targetHeight);

            Assert.That(this.targetMap.ToString(), Has.Exactly(cols * this.targetHeight).EqualTo(this.filler));
            Assert.That(this.targetMap.ToString(), Has.Exactly(rem * this.targetHeight).EqualTo(StringMap.EMPTY_CHAR));
        }

        [Test]
        public void CanDrawHalfRowsByCropFromSourceMap() {
            int rows = this.targetHeight / 2;
            int rem = this.targetHeight - rows;

            this.targetMap.DrawStringMap(this.sourceMap, this.transform, 0, 0, this.targetWidth, rows);

            Assert.That(this.targetMap.ToString(), Has.Exactly(rows * this.targetWidth).EqualTo(this.filler));
            Assert.That(this.targetMap.ToString(), Has.Exactly(rem * this.targetWidth).EqualTo(StringMap.EMPTY_CHAR));
        }

        [Test]
        public void CanDrawHalfColumnsByOffsetFromSourceMap() {
            int cols = this.targetWidth / 2;
            int rem = this.targetWidth - cols;

            this.targetMap.DrawStringMap(this.sourceMap, this.transform, cols, 0, this.targetWidth, this.targetHeight);

            Assert.That(this.targetMap.ToString(), Has.Exactly(rem * this.targetHeight).EqualTo(this.filler));
            Assert.That(this.targetMap.ToString(), Has.Exactly(cols * this.targetHeight).EqualTo(StringMap.EMPTY_CHAR));
        }

        [Test]
        public void CanDrawHalfRowsByOffsetFromSourceMap() {
            int rows = this.targetHeight / 2;
            int rem = this.targetHeight - rows;

            this.targetMap.DrawStringMap(this.sourceMap, this.transform, 0, rows, this.targetWidth, this.targetHeight);

            Assert.That(this.targetMap.ToString(), Has.Exactly(rem * this.targetWidth).EqualTo(this.filler));
            Assert.That(this.targetMap.ToString(), Has.Exactly(rows * this.targetWidth).EqualTo(StringMap.EMPTY_CHAR));
        }
    }

    [TestFixture]
    [TestFixtureSource(nameof(DrawRectangleTestsSource))]
    public class DrawRectangleTests(int width, int height) {
        [SetUp]
        public void Setup() {
            this.map = new StringMap(this.width, this.height);
        }

        private readonly int width = width;
        private readonly int height = height;
        private readonly char filler = 'x';

        private static object[] DrawRectangleTestsSource = {
            new object[] { 15, 10 },
            new object[] { 10, 10 },
            new object[] { 10, 15 }
        };

        private StringMap map;

        [Test]
        public void DrawRectangleOnEntireStringMap() {
            this.map.DrawRectangle(this.filler, 0, 0, this.width, this.height);

            Assert.That(this.map.ToString(), Has.Exactly(2 * this.width + 2 * this.height - 4).EqualTo(this.filler));

            for (var y = 1; y < this.height - 1; y++)
            for (var x = 1; x < this.width - 1; x++) {
                Assert.That(this.map.GetChar(x, y), Is.EqualTo(StringMap.EMPTY_CHAR));
            }
        }

        [Test]
        public void DrawRectangleExceedingAllBoundaries() {
            this.map.DrawRectangle(this.filler, -1, -1, this.width + 2, this.height + 2);

            Assert.That(this.map.ToString(), Has.Exactly(0).EqualTo(this.filler));

            for (var y = 0; y < this.height; y++)
            for (var x = 0; x < this.width; x++) {
                Assert.That(this.map.GetChar(x, y), Is.EqualTo(StringMap.EMPTY_CHAR));
            }
        }

        [Test]
        public void DrawRectangleExceedingLeftBoundary() {
            this.map.DrawRectangle(this.filler, -1, 0, this.width + 1, this.height);

            Assert.That(this.map.ToString(), Has.Exactly(2 * this.width + this.height - 2).EqualTo(this.filler));

            for (var y = 1; y < this.height - 1; y++)
            for (var x = 0; x < this.width - 1; x++) {
                Assert.That(this.map.GetChar(x, y), Is.EqualTo(StringMap.EMPTY_CHAR));
            }
        }

        [Test]
        public void DrawRectangleExceedingTopBoundary() {
            this.map.DrawRectangle(this.filler, 0, -1, this.width, this.height + 1);

            Assert.That(this.map.ToString(), Has.Exactly(2 * this.height + this.width - 2).EqualTo(this.filler));

            for (var y = 0; y < this.height - 1; y++)
            for (var x = 1; x < this.width - 1; x++) {
                Assert.That(this.map.GetChar(x, y), Is.EqualTo(StringMap.EMPTY_CHAR));
            }
        }

        [Test]
        public void DrawRectangleExceedingRightBoundary() {
            this.map.DrawRectangle(this.filler, 0, 0, this.width + 1, this.height);

            Assert.That(this.map.ToString(), Has.Exactly(2 * this.width + this.height - 2).EqualTo(this.filler));

            for (var y = 1; y < this.height - 1; y++)
            for (var x = 1; x < this.width; x++) {
                Assert.That(this.map.GetChar(x, y), Is.EqualTo(StringMap.EMPTY_CHAR));
            }
        }

        [Test]
        public void DrawRectangleExceedingBottomBoundary() {
            this.map.DrawRectangle(this.filler, 0, 0, this.width, this.height + 1);

            Assert.That(this.map.ToString(), Has.Exactly(2 * this.height + this.width - 2).EqualTo(this.filler));

            for (var y = 1; y < this.height; y++)
            for (var x = 1; x < this.width - 1; x++) {
                Assert.That(this.map.GetChar(x, y), Is.EqualTo(StringMap.EMPTY_CHAR));
            }
        }

        [Test]
        public void DrawCentredRectangle() {
            var border = 3;
            int startX = border, startY = border, width = this.width - 2 * border, height = this.height - 2 * border;

            this.map.DrawRectangle(this.filler, startX, startY, width, height);

            Assert.That(this.map.ToString(), Has.Exactly(
                2 * width + 2 * height - 4).EqualTo(this.filler));
        }

        [Test]
        public void Blah() {
            var title = "Ally is the gayest yeet yeet";
            var msg =
                "Ally loves pandas and bunnies and all the things, so it comes as no surprise that she would love boxes.\nDid you know boxes are handy dandy maths things?";
            Console.WriteLine(
                new Box(title,
                    msg) {
                    MaxWidth = 40, MaxHeight = 50,
                    TitlePadding = new Padding() { Left = 1, Right = 1, },
                    MessagePadding = new Padding() { Top = 1, Bottom = 1, Left = 3, Right = 3 },
                    TitleAlignment = Alignment.CENTER,
                    MessageAlignment = Alignment.JUSTIFY,
                    BorderWidth = 1,
                    BorderHeight = 1,
                }.AsStringMap().ToString());
        }
    }
}