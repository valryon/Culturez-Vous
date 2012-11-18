using System;
using CulturezVous.Data;
using Microsoft.Phone.Shell;

namespace CulturezVous.WP8.Services
{
    /// <summary>
    /// 
    /// </summary>
    public static class TileCreator
    {
        public static void CreateTile(Element element, int unreadElementCount)
        {
            StandardTileData newTileData = TileData.CreateTileData(element, unreadElementCount);

            string uri = string.Format("/Views/DetailsPage.xaml?id={0}", -1);

            Settings.LiveTileElementId = element.Id;

            ShellTile.Create(new Uri(uri, UriKind.Relative), newTileData); //exits application
        }
    }
}
