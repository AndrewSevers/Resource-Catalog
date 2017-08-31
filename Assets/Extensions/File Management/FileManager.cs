using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Extensions {

    public static class FileManager {

        #region Saving
        /// <summary>Save a file to the provided location</summary>
        /// <param name="aFilePath">Path of the file to save</param>
        /// <param name="aData">Data to save</param>
        /// <param name="aFormatAsJson">Format the data as json</param>
        /// <returns>True on successful save</returns>
        public static bool Save(string aFilePath, object aData, bool aFormatAsJson = false) {
            bool success = false;

            try {
                using (FileStream file = File.Open(aFilePath, FileMode.Create, FileAccess.Write)) {
                    BinaryFormatter formatter = new BinaryFormatter();

                    if (aFormatAsJson) {
                        formatter.Serialize(file, JsonUtility.ToJson(aData));
                    } else {
                        formatter.Serialize(file, aData);
                    }

                    success = true;
                }
            } catch (Exception e) {
                Debug.LogWarning("[FileManager] Error saving file: " + e.ToString());
            }

            return success;
        }
        #endregion

        #region Loading
        /// <summary>Load a file at the provided file path</summary>
        /// <param name="aFilePath">Path of the file to load</param>
        /// <returns>Casted object of type provided in calling function</returns>
        public static T Load<T>(string aFilePath) {
            object data = Load(aFilePath);
            return (data != null) ? (T) data : default(T);
        }

        /// <summary>Load a file at the provided file path</summary>
        /// <param name="aFilePath">Path of the file to load</param>
        /// <returns>Generic un-casted object</returns>
        public static object Load(string aFilePath) {
            object data = null;
            
            try {
                if (File.Exists(aFilePath)) {
                    using (FileStream file = File.Open(aFilePath, FileMode.Open, FileAccess.Read)) {
                        BinaryFormatter formatter = new BinaryFormatter();
                        data = formatter.Deserialize(file);
                    }
                }
            } catch (Exception e) {
                Debug.LogWarning("[FileManager] Error loading file: " + e.ToString());
            }

            return data;
        }
        #endregion

        #region Deleting
        /// <summary>Delete the file located at the file path</summary>
        /// <param name="aFilePath">Path of the file to delete (this is an extension of the Application path)</param>
        public static void Delete(string aFilePath) {
            File.Delete(aFilePath);
        }
        #endregion

    }

}
