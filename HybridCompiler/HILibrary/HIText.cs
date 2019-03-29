using System;
using System.Globalization;
using System.Linq;
using static HILibrary.HIDebug;

namespace HILibrary
{
    // 문자열 배열과 문자열 배열의 Index인 열거자를 이용하여 다국어를 지원한다.
    public class HIText
    {
        public HIText(Type type, string[,] texts)
        {
            Assert(type.IsEnum);
            Assert(texts);

            // 열거자의 크기와 문자열 배열의 크기가 일치해야 하고 열거자의 최대값과
            // 문자열 배열의 크기가 일치해야 한다.
            AssertEqual(Enum.GetValues(type).Length - 1, texts.GetUpperBound(0));
            AssertEqual(Enum.GetValues(type).Cast<int>().Max(), texts.GetUpperBound(0));

            this.type = type;
            this.texts = texts;
            SelectLanguage();
        }

        // HIText[id]로 문자열을 가져올 수 있게 한다.
        public string this[object id]
        {
            get
            {
                AssertEqual(id.GetType(), type);
                return texts[(int)id, cultureId];
            }
        }

        // 지역 코드에 맞는 언어를 선택한다.
        public void SelectLanguage(string culture = null)
        {
            // 지역 코드값이 비어있으면 실행되고 있는 쓰레드의 지역 코드를 읽는다.
            if (string.IsNullOrEmpty(culture)) { culture = CultureInfo.CurrentCulture.ToString(); }

            // 다국어 지원 문자열의 첫번째 줄은 지역 코드 목록을 담고 있어야 한다.
            // 동일한 지역 코드가 없을 경우 첫번째 지역 코드를 선택한다.
            for (cultureId = 0; cultureId <= texts.GetUpperBound(1); cultureId++)
            {
                if (texts[0, cultureId] == culture) { return; }
            }
            cultureId = 0;
        }

        private readonly string[,] texts;
        private readonly Type type;
        private int cultureId;
    }
}