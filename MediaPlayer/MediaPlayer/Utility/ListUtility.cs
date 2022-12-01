using System;
using System.Collections.Generic;


namespace MusicPlayer.Utility {

    public static class ListUtility {

        #region constant

        private static readonly Random RNG = new Random();

        #endregion

        #region logic

        public static void Shuffle<T>(this IList<T> list) {
            if (list == null) throw new ArgumentNullException(nameof(list));
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = RNG.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        #endregion

    }

}
