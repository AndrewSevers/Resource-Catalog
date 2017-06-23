using UnityEngine;

namespace Extensions.Utils {

    public static class JsonUtils {
        #region Wrapper Class
        [System.Serializable]
        private class Wrapper<T> {
            public T[] Items;
        }
        #endregion

        #region Conversion Methods
        public static T[] FromJson<T>(string aJson) {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(aJson);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] aArray) {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = aArray;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] aArray, bool aPrettyPrint) {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = aArray;
            return JsonUtility.ToJson(wrapper, aPrettyPrint);
        }
        #endregion

    }

}
