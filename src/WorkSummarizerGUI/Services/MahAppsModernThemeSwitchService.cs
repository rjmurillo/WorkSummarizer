using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MahApps.Metro;

namespace WorkSummarizerGUI.Services
{
    using System;

    public interface IThemeSwitchService
    {
        string AccentName { get; }

        string BackgroundName { get; }

        void CycleAccent();

        void CycleBackground();
    }

    internal class MahAppsModernThemeSwitchService : IThemeSwitchService
    {
        private readonly ModernUserInterfaceRuntime m_runtime;

        private IEnumerator<string> m_themeAccentChooser = GetThemeAccentChooser();
        private IEnumerator<Theme> m_themeBackgroundChooser = GetThemeBackgroundChooser();

        public MahAppsModernThemeSwitchService(ModernUserInterfaceRuntime runtime)
        {
            m_runtime = runtime;
            m_themeAccentChooser.MoveNext();
            m_themeBackgroundChooser.MoveNext();

            CycleToAccent(runtime.ThemeAccentSetting.Value);
            CycleToThemeBackground(runtime.ThemeBackgroundSetting.Value);
        }

        public string AccentName { get { return m_themeAccentChooser.Current; } }

        public string BackgroundName { get { return m_themeBackgroundChooser.Current.ToString(); } }

        public void CycleAccent() 
        {
            m_themeAccentChooser.MoveNext();
            m_runtime.ThemeAccentSetting.Value = AccentName;
            ThemeManager.ChangeTheme(Application.Current, ThemeManager.DefaultAccents.First(p => p.Name.Equals(m_themeAccentChooser.Current)), m_themeBackgroundChooser.Current);
        }

        public void CycleBackground() 
        {
            m_themeBackgroundChooser.MoveNext();
            m_runtime.ThemeBackgroundSetting.Value = BackgroundName;
            ThemeManager.ChangeTheme(Application.Current, ThemeManager.DefaultAccents.First(p => p.Name.Equals(m_themeAccentChooser.Current)), m_themeBackgroundChooser.Current);
        }

        private void CycleToAccent(string accentName)
        {
            if (string.IsNullOrWhiteSpace(accentName))
            {
                return;
            }

            var currentAccentName = m_themeAccentChooser.Current;
            do
            {
                if (m_themeAccentChooser.Current == accentName)
                {
                    break;
                }

                CycleAccent();
            }
            while (m_themeAccentChooser.Current != currentAccentName);
        }

        private void CycleToThemeBackground(string themeBackground)
        {
            Theme theme;
            if (!Enum.TryParse(themeBackground, out theme))
            {
                return;
            }

            var currentThemeBackground = m_themeBackgroundChooser.Current;
            do
            {
                if (m_themeBackgroundChooser.Current == theme)
                {
                    break;
                }

                CycleBackground();
            }
            while (m_themeBackgroundChooser.Current != currentThemeBackground);
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
