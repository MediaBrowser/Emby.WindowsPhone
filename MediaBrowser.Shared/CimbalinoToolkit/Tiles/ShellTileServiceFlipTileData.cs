// ****************************************************************************
// <copyright file="ShellTileServiceFlipTileData.cs" company="Pedro Lamas">
// Copyright © Pedro Lamas 2013
// </copyright>
// ****************************************************************************
// <author>Pedro Lamas</author>
// <email>pedrolamas@gmail.com</email>
// <date>21-05-2013</date>
// <project>Cimbalino.Phone.Toolkit.Background</project>
// <web>http://www.pedrolamas.com</web>
// <license>
// See license.txt in this solution or http://www.pedrolamas.com/license_MIT.txt
// </license>
// ****************************************************************************

using System;
using Microsoft.Phone.Shell;

namespace MediaBrowser.WindowsPhone.CimbalinoToolkit.Tiles
{
    /// <summary>
    /// Describes a Tile template that flips from the front to the back side. Allows customization of the background image and text for both the front and back Tile.
    /// </summary>
    public class ShellTileServiceFlipTileData : ShellTileServiceStandardTileData
    {
        #region Properties

        /// <summary>
        /// Gets or sets the front-side background image for the small Tile size.
        /// </summary>
        /// <value>The front-side background image for the small Tile size.</value>
        public Uri SmallBackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the back-side background image for the wide Tile size.
        /// </summary>
        /// <value>The back-side background image for the wide Tile size.</value>
        public Uri WideBackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the text that displays above the title, on the back-side of the wide Tile size.
        /// </summary>
        /// <value>The text that displays above the title, on the back-side of the wide Tile size.</value>
        public Uri WideBackBackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the front-side background image for the wide Tile size.
        /// </summary>
        /// <value>The front-side background image for the wide Tile size.</value>
        public string WideBackContent { get; set; }

        #endregion

        public override ShellTileData ToShellTileData()
        {
            return new FlipTileData()
            {
                Title = Title,
                BackBackgroundImage = BackBackgroundImage,
                BackContent = BackContent,
                BackTitle = BackTitle,
                BackgroundImage = BackgroundImage,
                Count = Count,
                SmallBackgroundImage = SmallBackgroundImage,
                WideBackBackgroundImage = WideBackBackgroundImage,
                WideBackContent = WideBackContent,
                WideBackgroundImage = WideBackgroundImage
            };
        }
    }
}