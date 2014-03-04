using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace WorkSummarizerGUI.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ThemeSwitchViewModel>();
        }

        public ThemeSwitchViewModel ThemeSwitch
        {
            get
            {
                return SimpleIoc.Default.GetInstance<ThemeSwitchViewModel>();
            }
        }
    }
}
