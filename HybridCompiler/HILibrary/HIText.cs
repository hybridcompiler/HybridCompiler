using System;
using System.Globalization;
using System.Linq;
using static HILibrary.HIDebug;

namespace HILibrary
{
    // 문자열 배열과 문자열 배열의 Index인 열거자를 이용하여 다국어를 지원한다.
    public class HIText
    {
        public HIText(Type enumType, string[,] texts)
        {
            Assert(enumType.IsEnum);
            Assert(texts);
            AssertEqual(Enum.GetValues(enumType).Length - 1, texts.GetUpperBound(0));
            AssertEqual(Enum.GetValues(enumType).Cast<int>().Max(), texts.GetUpperBound(0));

            this.enumType = enumType;
            this.texts = texts;
            SelectLanguage();
        }

        public string this[object id]
        {
            get
            {
                AssertEqual(id.GetType(), enumType);
                return texts[(int)id, cultureId];
            }
        }

        // 지역 코드를 이용하여 언어를 선택한다.
        public void SelectLanguage(string culture = null)
        {
            if (string.IsNullOrEmpty(culture)) { culture = CultureInfo.CurrentCulture.ToString(); }

            for (cultureId = 0; cultureId <= texts.GetUpperBound(1); cultureId++)
            {
                if (texts[0, cultureId] == culture) { return; }
            }
            cultureId = 0;
        }

        private readonly Type enumType;
        private readonly string[,] texts;
        private int cultureId;
    }
}