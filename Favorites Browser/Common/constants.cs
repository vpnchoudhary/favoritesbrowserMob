using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favorites_Browser.Common
{
    static class constants
    {
        public const string AppName = "RoamingFavoritesApp";
        public const string bookmark = "Bookmark";
        public const string AppSession = "Session";
        public const string PreferenceOpenInIE = "PreferenceOpenInIE";
        public const string FavoriteXML = "Favorites.xml";
        public const string SearchEngine = "SearchEngine";
        public static string SearchEngineBing = @"http://www.bing.com/search?q=";
        public static string SearchEngineGoogle = @"https://www.google.com/#q=";

    }
    public enum NewSignupState
    {
        NewUser,
        FavoriteFolderCreated
    }
    public enum UserPinState
    {
        PinNotSet,
        PinSet
    }
    public enum AppVisualState
    {
        Snapped,
        Narrow,
        Filled,
        FullScreenLandscape,
        FullScreenPortrait
    }

    public enum Platform
    {
        X86,
        X64,
        IA64,
        ARM,
        Unknown
    }
}
