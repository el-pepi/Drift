#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("EuqkkUiL8WDZRZaP/uh11bc9XcT8YEKCI0so9C8DsBDisLLkR8bYvtVDBG74tbb7qxZdfPE0u4A9BptRSY8YLQ33ug6enrCVeYTbNdRCtidwSaiZiSh4A+XBaoreYbS/oOp6rJQXGRYmlBccFJQXFxajLfbIvN1EW/BrbSEOVgX/6OaZo+H/jieXrJkT+LzEtwRR1aU3zOHQnUCdentEwMhwKgnjfJ+cKKlRsmWEcfNokEmILnM8XcHQST0g3zBS4XNdSIjy2hs6zle7vIGWqIdn4jPvyYdrtinPLCaUFzQmGxAfPJBekOEbFxcXExYVWjtWHHLmE7/+A9mJPs4Oev3JPfW+K6etVNm9/IzMTu38Og7Gs9Q9mIPliOe0w8vFpRQVFxYX");
        private static int[] order = new int[] { 13,10,12,5,8,10,11,11,9,12,12,13,12,13,14 };
        private static int key = 22;

        public static byte[] Data() {
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
