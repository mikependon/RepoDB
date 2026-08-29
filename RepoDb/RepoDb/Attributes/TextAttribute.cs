// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a text value on any object.
    /// </summary>
    public class TextAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="TextAttribute"/> class.
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