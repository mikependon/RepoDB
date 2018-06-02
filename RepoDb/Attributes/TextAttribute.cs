using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a text value on any object.
    /// </summary>
    public class TextAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.Attributes.TextAttribute</i> object.
        /// </summary>
        /// <param name="text">A value of the text.</param>
        public TextAttribute(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Gets a value of the text.
        /// </summary>
        public string Text { get; }
    }
}