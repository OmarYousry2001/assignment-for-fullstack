
using Resources.Data.Resources.Enumerations;
using System.Globalization;

namespace Resources.Data.Resources
{
    public static class ResourceManager
    {
        private static Language _currentLanguage;
  

        // Property to get or set the current culture
        public static Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                SetCultureForResources(_currentLanguage);

               
            }
        }

        public static string GetCultureName(Language language)
        {
            switch (language)
            {
                case Language.English:
                    return "en-US";
                default:
                    return "ar-EG";
            }
        }


        // Method to set the culture for all resources
        private static void SetCultureForResources(Language language)
        {
            var cultureName = GetCultureName(language);
            var culture = CultureInfo.GetCultureInfo(cultureName);
            ActionsResources.Culture = culture;
            FormResources.Culture = culture;
            GeneralResources.Culture = culture;
            GeneralUIResources.Culture = culture;
            NotifiAndAlertsResources.Culture = culture;
            UserResources.Culture = culture;
            ValidationResources.Culture = culture;
            SystemResources.Culture = culture;
        }

        public static void SetCurrentLanguage(Language language)
        {
            CurrentLanguage = language;
            SetCultureForResources(language);
        }

        public static void ChangeLanguage()
        {
            var newLanguage = CurrentLanguage == Language.Arabic ?
                Language.English : Language.Arabic;
            SetCurrentLanguage(newLanguage);
        }
    }
}