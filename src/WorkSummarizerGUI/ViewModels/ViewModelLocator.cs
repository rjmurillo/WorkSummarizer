using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events;
using Extensibility;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Renders;

namespace WorkSummarizerGUI.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<CreateReportViewModel>(() => new CreateReportViewModel(new Dictionary<ServiceRegistration, IEventQueryService>(), new Dictionary<ServiceRegistration, IRenderEvents>()));
            }
            else
            {
                SimpleIoc.Default.Register<CreateReportViewModel>(() => new CreateReportViewModel(SimpleIoc.Default.GetInstance<IPluginRuntime>().EventQueryServices, SimpleIoc.Default.GetInstance<IPluginRuntime>().RenderEventServices));
                SimpleIoc.Default.Register<ThemeSwitchWidgetViewModel>();
            }            
        }

        public CreateReportViewModel CreateReport
        {
            get
            {
                return SimpleIoc.Default.GetInstance<CreateReportViewModel>();
            }
        }

        public ThemeSwitchWidgetViewModel ThemeSwitchWidget
        {
            get
            {
                return SimpleIoc.Default.GetInstance<ThemeSwitchWidgetViewModel>();
            }
        }
    }
}
