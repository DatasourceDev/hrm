using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SBSModel.Common
{

    public class RandomColor
    {

        public static Color GetLstName()
        {
            Random randomGen = new Random();
            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            KnownColor randomColorName = names[randomGen.Next(names.Length)];
            Color randomColor = Color.FromKnownColor(randomColorName);

            return randomColor;
        }

        public static Color GetLstCode()
        {
            Random random = new Random();
            var color = String.Format("#{0:X6}", random.Next(0x1000000));

            return Color.FromName(color);
        }

    }
}
