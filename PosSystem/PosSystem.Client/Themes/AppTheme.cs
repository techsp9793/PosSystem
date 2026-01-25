using MudBlazor;

namespace PosSystem.Client.Themes
{
    public static class AppTheme
    {
        public static MudTheme Default = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = Colors.Blue.Default,
                AppbarBackground = Colors.BlueGray.Darken4,
                Background = Colors.Gray.Lighten5,
                Surface = "#FFFFFF",
            },
            PaletteDark = new PaletteDark()
            {
                Primary = Colors.Blue.Lighten1,
                AppbarBackground = Colors.BlueGray.Darken4,
                Background = "#1a1a1a",
                Surface = "#1e1e1e",
                BackgroundGray = "#1a1a1a",
            }
        };
    }
}

