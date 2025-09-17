using System.Globalization;
namespace Domains.Data.Commons
{
    //public class GeneralLocalizableEntity
    //{
    //    public string Localize(string textAr, string textEN)
    //    {
    //        CultureInfo CultureInfo = Thread.CurrentThread.CurrentCulture;
    //        if (CultureInfo.TwoLetterISOLanguageName.ToLower().Equals("ar"))
    //            return textAr;
    //        return textEN;
    //    }
    //}

        public static class GeneralLocalizableEntity
        {
            /// <summary>
            /// Returns the Arabic or English text based on the current thread's culture.
            /// </summary>
            /// <param name="textAr">Arabic text.</param>
            /// <param name="textEN">English text.</param>
            /// <returns>The localized string.</returns>
            public static string Localize(string textAr, string textEN)
            {
                var culture = Thread.CurrentThread.CurrentCulture;
                if (culture.TwoLetterISOLanguageName.ToLower() == "ar")
                    return textAr;
                return textEN;
            }
        }

}
