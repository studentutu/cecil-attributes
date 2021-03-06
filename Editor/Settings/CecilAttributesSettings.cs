﻿using System;
using System.IO;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
    [Serializable]
    public class CecilAttributesSettings : ScriptableObject
    {
        private const string DIRECTORY = "ProjectSettings/Packages/se.hertzole.cecilattributes";
        private const string PATH = DIRECTORY + "/CecilAttributesSettings.asset";

        [SerializeField]
        private bool includeResetStaticInBuild = true;
        [SerializeField]
        private RuntimeInitializeLoadType resetStaticMode = RuntimeInitializeLoadType.SubsystemRegistration;
        [SerializeField]
        private bool includeLogsInBuild = true;
        [SerializeField]
        private string methodLogFormat = "%class% %method% (%parameters%)";
        [SerializeField]
        private string parametersSeparator = ", ";
        [SerializeField]
        private string propertyGetLogFormat = "%property% Get %value%";
        [SerializeField]
        private string propertySetLogFormat = "%property% Set (Old: %old_value%, New: %new_value%)";
        [SerializeField]
        private bool includeTimedInBuild = false;
        [SerializeField]
        private string timedMethodFormat = "%class% %method% took %milliseconds% milliseconds (%ticks% ticks)";
        [SerializeField]
        private string timedPropertyGetFormat = "%class% %property% Get took %milliseconds% milliseconds (%ticks% ticks)";
        [SerializeField]
        private string timedPropertySetFormat = "%class% %property% Set took %milliseconds% milliseconds (%ticks% ticks)";
        [SerializeField]
        private string markInProfilerFormat = "%class% :: %method%";

        public bool IncludeResetStaticInBuild
        {
            get { return includeResetStaticInBuild; }
            set { includeResetStaticInBuild = value; Save(); }
        }

        public RuntimeInitializeLoadType ResetStaticMode
        {
            get { return resetStaticMode; }
            set { resetStaticMode = value; Save(); }
        }

        public bool IncludeLogsInBuild
        {
            get { return includeLogsInBuild; }
            set { includeLogsInBuild = value; Save(); }
        }

        public string MethodLogFormat
        {
            get { return methodLogFormat; }
            set { methodLogFormat = value; Save(); }
        }

        public string ParametersSeparator
        {
            get { return parametersSeparator; }
            set { parametersSeparator = value; Save(); }
        }

        public string PropertyGetLogFormat
        {
            get { return propertyGetLogFormat; }
            set { propertyGetLogFormat = value; Save(); }
        }

        public string PropertySetLogFormat
        {
            get { return propertySetLogFormat; }
            set { propertySetLogFormat = value; Save(); }
        }

        public bool IncludeTimedInBuild
        {
            get { return includeTimedInBuild; }
            set { includeTimedInBuild = value; Save(); }
        }

        public string TimedMethodFormat
        {
            get { return timedMethodFormat; }
            set { timedMethodFormat = value; Save(); }
        }

        public string TimedPropertyGetFormat
        {
            get { return timedPropertyGetFormat; }
            set { timedPropertyGetFormat = value; Save(); }
        }

        public string TimedPropertySetFormat
        {
            get { return timedPropertySetFormat; }
            set { timedPropertySetFormat = value; Save(); }
        }

        public string MarkInProfilerFormat
        {
            get { return markInProfilerFormat; }
            set { markInProfilerFormat = value; Save(); }
        }

        private static CecilAttributesSettings instance;
        public static CecilAttributesSettings Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = GetOrCreate();
                return instance;
            }
        }

        public static void Save()
        {
            SaveInstance(Instance);
        }

        private static CecilAttributesSettings GetOrCreate()
        {
            CecilAttributesSettings settings;

            // Backwards compatibility.
            string oldPath = "ProjectSettings/CecilAttributesSettings.asset";

            if (File.Exists(oldPath))
            {
                settings = LoadSettings(oldPath);
                RemoveFile(oldPath);

                if (settings != null)
                {
                    return settings;
                }
            }

            if (!File.Exists(PATH))
            {
                settings = CreateNewSettings();
            }
            else
            {
                settings = LoadSettings();

                if (settings == null)
                {
                    RemoveFile(PATH);
                    settings = CreateNewSettings();
                }
            }

            settings.hideFlags = HideFlags.HideAndDontSave;

            return settings;
        }

        private static CecilAttributesSettings CreateNewSettings()
        {
            CecilAttributesSettings settings = CreateInstance<CecilAttributesSettings>();
            SaveInstance(settings);
            return settings;
        }

        private static CecilAttributesSettings LoadSettings(string path = "")
        {
            CecilAttributesSettings settings;

            try
            {
                settings = (CecilAttributesSettings)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(string.IsNullOrEmpty(path) ? PATH : path)[0];
            }
            catch (Exception)
            {
                Debug.Log("Couldn't load cecil attribute settings. Settings will be reset.");
                settings = null;
            }

            return settings;
        }

        private static void RemoveFile(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            FileAttributes attributes = File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.ReadOnly))
            {
                File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
            }

            File.Delete(path);
        }

        private static void SaveInstance(CecilAttributesSettings settings)
        {
            if (!Directory.Exists(DIRECTORY))
            {
                Directory.CreateDirectory(DIRECTORY);
            }

            try
            {
                UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new UnityEngine.Object[] { settings }, PATH, true);
            }
            catch (Exception ex)
            {
                Debug.LogError("Can't save cecil attribute settings!\n" + ex);
            }
        }
    }
}
