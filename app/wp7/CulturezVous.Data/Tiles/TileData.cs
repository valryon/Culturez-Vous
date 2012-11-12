using System;
using System.Linq;
using Microsoft.Phone.Shell;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Resources;
using System.Windows.Resources;

namespace CulturezVous.Data
{
    public static class TileData
    {
        private const string tileImgPath = "/Shared/ShellContent/tile-bg.png";

        /// <summary>
        /// Create data for the tile
        /// </summary>
        /// <param name="element"></param>
        /// <param name="unreadElementCount"></param>
        /// <param name="tileLogoUri"></param>
        /// <returns></returns>
        public static StandardTileData CreateTileData(Element element, int unreadElementCount)
        {
            // Generate image
            createTileBitmap(element, unreadElementCount);

            // Create tile data
            var tile = new StandardTileData
            {
                BackgroundImage = new Uri("isostore:" + tileImgPath, UriKind.RelativeOrAbsolute),
                Title = "Culturez-Vous !"
            };

            if (element != null)
            {
                tile.BackContent = element.Title;
                tile.BackTitle = "à lire";
            }

            return tile;
        }

        /// <summary>
        /// Create an image for the tile
        /// </summary>
        /// <param name="element"></param>
        /// <param name="unreadElementCount"></param>
        /// <param name="tileLogoUri"></param>
        /// <returns></returns>
        private static void createTileBitmap(Element element, int unread)
        {
            string unreadText = unread.ToString();

            // Stop at 99 to keep a nice tile
            if (unread > 99)
            {
                unreadText = "∞";
            }

            Grid grid = new Grid();
            grid.Height = 173;
            grid.Width = 173;
            grid.Background = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            Image image = new Image();
            image.Stretch = Stretch.None;
            image.VerticalAlignment = VerticalAlignment.Center;
            image.HorizontalAlignment = HorizontalAlignment.Right;
            image.Margin = new Thickness { Left = 0, Bottom = 0, Right = 0, Top = 0 };

            // HACK
            Uri uri = new Uri("/CulturezVous.Data;component/Tiles/tileLogo2.png", UriKind.Relative);
            var bitmap = new BitmapImage(uri);
            StreamResourceInfo streamInfo = Application.GetResourceStream(uri);

            image.Source = bitmap;
            Grid.SetColumn(image, 1);
            grid.Children.Add(image);

            if (unread > 0)
            {
                TextBlock unreadTb = new TextBlock();
                unreadTb.FontSize = 52f;
                unreadTb.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                unreadTb.Margin = new Thickness { Left = 6, Bottom = 0, Right = 6, Top = 0 };
                unreadTb.Text = unreadText;
                unreadTb.HorizontalAlignment = HorizontalAlignment.Left;
                unreadTb.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(unreadTb, 2);
                grid.Children.Add(unreadTb);
            }
            else
            {
                image.Margin = new Thickness { Left = 24, Bottom = 24, Right = 24, Top = 24 };
            }

            //call measure, arrange and updatelayout to prepare for rendering
            grid.Measure(new Size(173, 173));
            grid.Arrange(new Rect(0, 0, 173, 173));
            grid.UpdateLayout();

            WriteableBitmap wbm = new WriteableBitmap(173, 173);
            wbm.Render(grid, null);
            wbm.Invalidate();

            //write image to isolated storage - note that the \Shared\ShellContent folder is required
            string sIsoStorePath = @"\Shared\ShellContent\tile-bg.png";
            using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //ensure directory exists
                String sDirectory = System.IO.Path.GetDirectoryName(sIsoStorePath);
                if (!appStorage.DirectoryExists(sDirectory))
                {
                    appStorage.CreateDirectory(sDirectory);
                }

                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(sIsoStorePath, System.IO.FileMode.Create, appStorage))
                {
                    wbm.SaveJpeg(stream, 173, 173, 0, 100);
                }
            }
        }

        public static void UpdateTile(ShellTile tile, Element element, int unreadElementCount)
        {
            StandardTileData newTileData = TileData.CreateTileData(element, unreadElementCount);

            Settings.LiveTileElementId = element.Id;

            tile.Update(newTileData);
        }

        public static void FindAndUpdateTile()
        {
            ShellTile tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("/Views/DetailsPage"));
            if (tileToFind != null)
            {
                // Update the tile is required
                if (CacheManager.Instance.IsInitialized == false)
                {
                    CacheManager.Instance.Initialize(InitializeLevel.Readonly);
                }

                int count = CacheManager.Instance.Elements.Where(e => e.IsRead == false).Count();

                Element lastElement = CacheManager.Instance.GetCacheLast<Element>();

                TileData.UpdateTile(tileToFind, lastElement, count);
            }
        }
    }
}
