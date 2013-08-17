// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using NUnit.Framework;

namespace MfGames.GtkExt.Tests
{
	[TestFixture]
	public class PangoUtilityTests
	{
		#region Methods

		[Test]
		public void ToPangoOnBeginningOfSimpleString()
		{
			// Arrange
			const int stringIndex = 0;
			const string text = "simple";

			// Act
			int pangoIndex = PangoUtility.TranslateStringToPangoIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(0, pangoIndex);
		}

		[Test]
		public void ToPangoOnBeginningOfTwoByteString()
		{
			// Arrange
			const int stringIndex = 0;
			const string text = "éâäàçê";

			// Act
			int pangoIndex = PangoUtility.TranslateStringToPangoIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(0, pangoIndex);
		}

		[Test]
		public void ToPangoOnBeyondSimpleString()
		{
			// Arrange
			const int stringIndex = 7;
			const string text = "simple";

			// Act
			var exception =
				Assert.Throws<ArgumentOutOfRangeException>(
					() => PangoUtility.TranslateStringToPangoIndex(text, stringIndex));

			// Assert
			Assert.AreEqual(
				"Provided index (7) cannot be greater than the length of the text (6).\r\nParameter name: stringIndex",
				exception.Message);
		}

		[Test]
		public void ToPangoOnBeyondTwoByteString()
		{
			// Arrange
			const int stringIndex = 7;
			const string text = "éâäàçê";

			// Act
			var exception =
				Assert.Throws<ArgumentOutOfRangeException>(
					() => PangoUtility.TranslateStringToPangoIndex(text, stringIndex));

			// Assert
			Assert.AreEqual(
				"Provided index (7) cannot be greater than the length of the text (6).\r\nParameter name: stringIndex",
				exception.Message);
		}

		[Test]
		public void ToPangoOnEmptyString()
		{
			// Arrange
			const int stringIndex = 0;
			const string text = "";

			// Act
			int pangoIndex = PangoUtility.TranslateStringToPangoIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(0, pangoIndex);
		}

		[Test]
		public void ToPangoOnEndOfSimpleString()
		{
			// Arrange
			const int stringIndex = 6;
			const string text = "simple";

			// Act
			int pangoIndex = PangoUtility.TranslateStringToPangoIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(6, pangoIndex);
		}

		[Test]
		public void ToPangoOnEndOfTwoByteString()
		{
			// Arrange
			const int stringIndex = 6;
			const string text = "éâäàçê";

			// Act
			int pangoIndex = PangoUtility.TranslateStringToPangoIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(12, pangoIndex);
		}

		[Test]
		public void ToPangoOnMiddleOfSimpleString()
		{
			// Arrange
			const int stringIndex = 4;
			const string text = "simple";

			// Act
			int pangoIndex = PangoUtility.TranslateStringToPangoIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(4, pangoIndex);
		}

		[Test]
		public void ToPangoOnMiddleOfTwoByteString()
		{
			// Arrange
			const int stringIndex = 4;
			const string text = "éâäàçê";

			// Act
			int pangoIndex = PangoUtility.TranslateStringToPangoIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(8, pangoIndex);
		}

		[Test]
		public void ToStringOnBeginningOfSimpleString()
		{
			// Arrange
			const int stringIndex = 0;
			const string text = "simple";

			// Act
			int pangoIndex = PangoUtility.TranslatePangoToStringIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(0, pangoIndex);
		}

		[Test]
		public void ToStringOnBeginningOfTwoByteString()
		{
			// Arrange
			const int stringIndex = 0;
			const string text = "éâäàçê";

			// Act
			int pangoIndex = PangoUtility.TranslatePangoToStringIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(0, pangoIndex);
		}

		[Test]
		public void ToStringOnEmptyString()
		{
			// Arrange
			const int stringIndex = 0;
			const string text = "";

			// Act
			int pangoIndex = PangoUtility.TranslatePangoToStringIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(0, pangoIndex);
		}

		[Test]
		public void ToStringOnEndOfSimpleString()
		{
			// Arrange
			const int stringIndex = 6;
			const string text = "simple";

			// Act
			int pangoIndex = PangoUtility.TranslatePangoToStringIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(6, pangoIndex);
		}

		[Test]
		public void ToStringOnEndOfTwoByteString()
		{
			// Arrange
			const int stringIndex = 12;
			const string text = "éâäàçê";

			// Act
			int pangoIndex = PangoUtility.TranslatePangoToStringIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(6, pangoIndex);
		}

		[Test]
		public void ToStringOnMiddleOfSimpleString()
		{
			// Arrange
			const int stringIndex = 4;
			const string text = "simple";

			// Act
			int pangoIndex = PangoUtility.TranslatePangoToStringIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(4, pangoIndex);
		}

		[Test]
		public void ToStringOnMiddleOfTwoByteString()
		{
			// Arrange
			const int stringIndex = 8;
			const string text = "éâäàçê";

			// Act
			int pangoIndex = PangoUtility.TranslatePangoToStringIndex(text, stringIndex);

			// Assert
			Assert.AreEqual(4, pangoIndex);
		}

		#endregion
	}
}
