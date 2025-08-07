using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Reflection;
//using GlobalEnums;
using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using Satchel;
//using UnityEngine.PlayerLoop;
//using HutongGames.PlayMaker.Actions;
//using HKMirror;
//using HKMirror.Hooks.ILHooks;
using MonoMod.Cil;

namespace TestMod
{
    public class TestModMod : Mod, ITogglableMod, IGlobalSettings<GlobalSettings>
    {
        private static TestModMod? _instance;
        public GameObject crawlerPrefab;
        public Dictionary<string, GameObject> preloads = new Dictionary<string, GameObject>();
        public HatchlingHandler hatchlingHandler;

        public GlobalSettings Settings { get; private set; } = new GlobalSettings();

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        internal static TestModMod Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(TestModMod)} was never constructed");
                }
                return _instance;
            }
        }

        public TestModMod() : base("TestMod")
        {
            _instance = this;
            hatchlingHandler = new HatchlingHandler(_instance);
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            //Don't set preload objects if preloads already exist
            if (preloadedObjects != null)
            {
                // Preload entities into a public dictionary for spawning purposes
                var crawlerPrefab = preloadedObjects["Crossroads_01"]["_Enemies/Crawler 1"];
                preloads.Add("Crawler", crawlerPrefab);
            }

            RegisterCallbacks();

            Log("Initialized");
        }


        public void RegisterCallbacks()
        {
            ModHooks.NewGameHook += OnNewGame;
            On.NailSlash.StartSlash += StartSlash;
            ModHooks.HeroUpdateHook += HeroUpdate;
            ModHooks.AfterSavegameLoadHook += AfterSavegameLoad;
            IL.HeroController.TakeDamage += GrubsongChanges;
        }

        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("Crossroads_01", "_Enemies/Crawler 1")
            };
        }

        private static void OnNewGame()
        {
            //PlayerData.instance.maxHealthBase = PlayerData.instance.maxHealth = PlayerData.instance.health = 8;
            //PlayerData.instance.charmSlots += 2;
        }

        public void Unload()
        {
            ModHooks.NewGameHook -= OnNewGame;
            On.NailSlash.StartSlash -= StartSlash;
            ModHooks.HeroUpdateHook -= HeroUpdate;
            ModHooks.AfterSavegameLoadHook -= AfterSavegameLoad;
        }

        public void AfterSavegameLoad(SaveGameData data)
        {
            GameManager.instance.StartCoroutine(HeroFinder());
        }

        private IEnumerator HeroFinder()
        {
            //Wait until hero is loaded
            yield return new WaitWhile(() => HeroController.instance == null);

            //Get a reference to the charm effects object
            var charmEffects = GameObject.Find("Charm Effects");

            //Set up Hatchling Handler
            hatchlingHandler.Setup(charmEffects);
        }

        public void GrubsongChanges(ILContext il)
        {
            Log("Took damage");
        }

        public void HeroUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Log("Space was pressed");
                if (PlayerData.instance.GetBool("equippedCharm_22"))
                {
                    Log("Charm is equipped");
                    hatchlingHandler.TrySpawnHatchling();
                }               
            }
        }

        public void StartSlash(On.NailSlash.orig_StartSlash orig, NailSlash self)
        {
            orig(self);                       
        }
        

        public void OnLoadGlobal(GlobalSettings s) => Settings = s;

        public GlobalSettings OnSaveGlobal() => Settings;
        
    }
}
