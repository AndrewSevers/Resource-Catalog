using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Resource {

    public static class FileManager {

        #region Saving
        /// <summary>Save a file to the provided location</summary>
        /// <param name="aFilePath">Path of the file to save (this is an extension of the Application path)</param>
        /// <param name="aData">Data to save</param>
        /// <param name="aFormatAsJson">Format the data as json</param>
        /// <param name="aUseStreamingPath">Save to the streaming assets path rather than the persistent data path</param>
        /// <returns>True on successful save</returns>
        public static bool Save(string aFilePath, object aData, bool aFormatAsJson = false, bool aUseStreamingPath = false) {
            string assetsPath = (aUseStreamingPath) ? Application.streamingAssetsPath : Application.persistentDataPath;

            FileStream file = null;
            try {
                file = File.Open(assetsPath + aFilePath, FileMode.Create, FileAccess.Write);

                BinaryFormatter formatter = new BinaryFormatter();

                if (aFormatAsJson) {
                    formatter.Serialize(file, JsonUtility.ToJson(aData));
                } else {
                    formatter.Serialize(file, aData);
                }

                return true;
            } catch (Exception e) {
                Debug.LogWarning("[FileManager] Error saving file: " + e.ToString());
            } finally {
                if (file != null) {
                    file.Close();
                }

                file = null;
            }

            return false;
        }
        #endregion

        #region Loading
        /// <summary>Load a file at the provided file path</summary>
        /// <param name="aFilePath">Path of the file to load (this is an extension of the Application path)</param>
        /// <param name="aCreateEmptyFile">If the file does not exist create a new empty file</param>
        /// <param name="aUseStreamingPath">Load from the streaming assets path rather than the persistent data path</param>
        public static object Load(string aFilePath, bool aUseStreamingPath = false) {
            string assetsPath = (aUseStreamingPath) ? Application.streamingAssetsPath : Application.persistentDataPath;
            object loadedData = null;

            FileStream file = null;
            try {
                if (File.Exists(assetsPath + aFilePath)) {
                    file = File.Open(assetsPath + aFilePath, FileMode.Open, FileAccess.Read);

                    BinaryFormatter formatter = new BinaryFormatter();
                    loadedData = formatter.Deserialize(file);
                }
            } catch (Exception e) {
                Debug.LogWarning("[FileManager] Error saving file: " + e.ToString());
            } finally {
                if (file != null) {
                    file.Close();
                }

                file = null;
            }

            return loadedData;
        }
        #endregion

        #region Deleting
        /// <summary>Delete the file located at the file path</summary>
        /// <param name="aFilePath">Path of the file to delete (this is an extension of the Application path)</param>
        /// <param name="aUseStreamingPath">Load from the streaming assets path rather than the persistent data path</param>
        public static void Delete(string aFilePath, bool aUseStreamingPath = false) {
            string assetsPath = (aUseStreamingPath) ? Application.streamingAssetsPath : Application.persistentDataPath;
            File.Delete(assetsPath + aFilePath);
        }
        #endregion

    }

}
