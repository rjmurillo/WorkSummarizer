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
    internal class ViewModelLocator
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
                SimpleIoc.Default.Register<LocationStatusWidgetViewModel>();
                SimpleIoc.Default.Register<ThemeSwitchWidgetViewModel>();
                SimpleIoc.Default.Register<ToastNotificationWidgetViewModel>();
                SimpleIoc.Default.Register<UserWidgetViewModel>();
            }            
        }

        public static void RegisterService<T>(T instance) where T : class
        {
            SimpleIoc.Default.Register<T>(() => instance);
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

        public LocationStatusWidgetViewModel LocationStatusWidget
        {
            get
            {
                return SimpleIoc.Default.GetInstance<LocationStatusWidgetViewModel>();
            }
        }

        public ToastNotificationWidgetViewModel ToastNotificationWidget
        {
            get
            {
                return SimpleIoc.Default.GetInstance<ToastNotificationWidgetViewModel>();
            }
        }

        public UserWidgetViewModel UserWidget
        {
            get
            {
                return SimpleIoc.Default.GetInstance<UserWidgetViewModel>();
            }
        }
    }
}
