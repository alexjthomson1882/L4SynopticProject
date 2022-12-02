using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MusicPlayer.Utility;

namespace MusicPlayer.Tests.Utility {

    [TestClass]
    public class ListUtiltiyTests {

        [TestMethod]
        [DataRow(   0, 0.00f)]
        [DataRow( 500, 0.60f)]
        [DataRow(1000, 0.70f)]
        [DataRow(2500, 0.80f)]
        [DataRow(5000, 0.90f)]
        /// <summary>
        /// Creates and shuffles a list. Verifies if the contents of the list are as expected.
        /// </summary>
        /// <param name="listSize">Size of the list to test.</param>
        /// <param name="shuffleQuota">Percentage of the list that should be shuffled.</param>
        public void ShuffleTest(int listSize, float shuffleQuota) {
            if (listSize < 0) throw new ArgumentOutOfRangeException(nameof(listSize));
            if (shuffleQuota < 0.0f || shuffleQuota > 1.0f) throw new ArgumentOutOfRangeException(nameof(shuffleQuota));
            // create list:
            int[] list = new int[listSize];
            for (int i = 0; i < listSize; i++) {
                list[i] = i;
            }
            // shuffle list:
            ListUtility.Shuffle(list);
            // check list has been shuffled according to quota:
            int shuffledCount = 0;
            HashSet<int> set = new HashSet<int>(listSize);
            for (int i = 0; i < listSize; i++) {
                // get current value:
                int value = list[i];
                // check if current value has moved position:
                if (value != i) shuffledCount++; // add to the shuffled count
                // check for duplicates:
                Assert.IsFalse(set.Contains(value));
                set.Add(value);
            }
            Assert.IsTrue(shuffledCount >= listSize * shuffleQuota); // shuffle quota met
        }

    }

}
