﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace upScreenLib
{
    public static class Settings
    {
        private static readonly string AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"upScreen");
        private static readonly string ConfigPath = Path.Combine(AppDataFolder, "settings.json");

        public static List<Profile> Profiles = new List<Profile>();

        private static Profile CurrentProfile { get { return Profiles.Count > 0 ? Profiles[DefaultProfile] : new Profile(); } }
        public static string[] ProfileTitles { get { return Profiles.Select(p => string.Format("{0}@{1}", p.Username, p.Host)).ToArray(); } }
        public static int DefaultProfile { get { return Profiles.Any(a=>a.IsDefaultAccount) ?  Profiles.IndexOf(Profiles.Where(p => p.IsDefaultAccount).ToArray()[0]) : 0; } }

        /// <summary>
        /// Load the user settings from the configuration file
        /// </summary>
        public static void Load()
        {
            Common.Profile = new Profile();
            // check for existing config file
            if (!Directory.Exists(AppDataFolder)) Directory.CreateDirectory(AppDataFolder);
            if (!File.Exists(ConfigPath)) return;
            // if existing config file exists, load it to _profiles
            string config = File.ReadAllText(ConfigPath);
            Profiles.AddRange( (List<Profile>)JsonConvert.DeserializeObject(config, typeof(List<Profile>)) );

            if (Profiles.Count <= 0) return;

            // Set Common.Profile as the CurrentProfile            
            Common.Profile = CurrentProfile;
        }

        /// <summary>
        /// Save the user settings to the configuration file
        /// </summary>
        public static void Save()
        {
            var profiles = new List<Profile>(Profiles);
            var settings = new JsonSerializerSettings();
            string json = JsonConvert.SerializeObject(profiles, typeof(List<Profile>), Formatting.Indented, settings);
            File.WriteAllText(ConfigPath, json);
        }

        /// <summary>
        /// Remove all profiles from the config file
        /// </summary>
        public static void Clear()
        {
            Profiles.Clear();
            var profiles = new List<Profile>(Profiles);
            string json = JsonConvert.SerializeObject(profiles, Formatting.Indented);
            File.WriteAllText(ConfigPath, json);
            try
            {
                // The empty file isnt needed nomore, might as well delete it...
                File.Delete(ConfigPath);
            }
            catch {}
        }
    }
}