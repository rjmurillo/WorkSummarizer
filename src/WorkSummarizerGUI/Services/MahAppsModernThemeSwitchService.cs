using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MahApps.Metro;

namespace WorkSummarizerGUI.Services
{
    public interface IThemeSwitchService
    {
        string AccentName { get; }

        string BackgroundName { get; }

        void CycleAccent();

        void CycleBackground();
    }

    public class MahAppsModernThemeSwitchService : IThemeSwitchService
    {
        private IEnumerator<string> m_themeAccentChooser = GetThemeAccentChooser();
        private IEnumerator<Theme> m_themeBackgroundChooser = GetThemeBackgroundChooser();

        public MahAppsModernThemeSwitchService()
        {
            m_themeAccentChooser.MoveNext();
            m_themeBackgroundChooser.MoveNext();
        }

        public string AccentName { get { return m_themeAccentChooser.Current; } }

        public string BackgroundName { get { return m_themeBackgroundChooser.Current.ToString(); } }

        public void CycleAccent() 
        {
            m_themeAccentChooser.MoveNext();
            ThemeManager.ChangeTheme(Application.Current, ThemeManager.DefaultAccents.First(p => p.Name.Equals(m_themeAccentChooser.Current)), m_themeBackgroundChooser.Current);
        }

        public void CycleBackground() 
        {
            m_themeBackgroundChooser.MoveNext();
            ThemeManager.ChangeTheme(Application.Current, ThemeManager.DefaultAccents.First(p => p.Name.Equals(m_themeAccentChooser.Current)), m_themeBackgroundChooser.Current);
        }

        private static IEnumerator<Theme> GetThemeBackgroundChooser()
        {
            while (true)
            {
                yield return Theme.Light;
                yield return Theme.Dark;
            }
        }

        private static IEnumerator<string> GetThemeAccentChooser()
        {
            while (true)
            {
                yield return "Sienna";
                yield return "Amber";
                yield return "Blue";
                yield return "Brown";
                yield return "Cobalt";
                yield return "Crimson";
                yield return "Cyan";
                yield return "Emerald";
                yield return "Green";
                yield return "Indigo";
                yield return "Lime";
                yield return "Magenta";
                yield return "Mauve";
                yield return "Olive";
                yield return "Orange";
                yield return "Pink";
                yield return "Purple";
                yield return "Red";
                yield return "Steel";
                yield return "Teal";
                yield return "Violet";
                yield return "Yellow";
            }
        }
    }
}
