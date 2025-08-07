using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Satchel;
using UnityEngine.PlayerLoop;
using HutongGames.PlayMaker.Actions;

namespace TestMod
{
    public class HatchlingHandler
    {
        private TestModMod TestModRef;

        private int hatchlingCount = 0;

        private GameObject hatchlingRef = new GameObject();        
        private PlayMakerFSM hatchlingFSM = new PlayMakerFSM();
        private KnightHatchling hatchlingComp = new KnightHatchling();

        private Vector3 spawnPosition;

        public HatchlingHandler(TestModMod TestModRef)
        {
            this.TestModRef = TestModRef;
        }

        public void Setup(GameObject charmEffects)
        {
            //Get a reference to the hatchling spawn FSM
            hatchlingFSM = charmEffects.LocateMyFSM("Hatchling Spawn");
            hatchlingComp = hatchlingFSM.GetComponent<KnightHatchling>();
            
            //Get a reference to the hatchling game object and add it to the preload dictionary
            hatchlingRef = hatchlingFSM.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Hatch").gameObject.Value;
            TestModRef.preloads.Add("Hatchling", hatchlingRef);

            //This modifies the existing charm's max hatchling count to 10
            hatchlingFSM.InsertAction("Equipped", new SetIntValue { intVariable = hatchlingFSM.Fsm.GetFsmInt("Hatchling Max"), intValue = 10, everyFrame = false }, 0);
        }

        public void TrySpawnHatchling()
        {
            if (hatchlingRef != null)
            {
                if (hatchlingCount < 8)
                {
                    //Get knight position to spawn the hatchlings
                    //spawnPosition = HeroController.instance.transform.position;
                    //TestModRef.Log(HeroController.instance.transform.position + " - " + spawnPosition);

                    TestModRef.Log("Trying to Spawn Hatchling");

                    //Set spawn state to check for hatching conditions
                    hatchlingFSM.SetState("Can Hatch?");

                    /* Force spawn individual hatchlings
                    GameObject hatchlingObj = GameObject.Instantiate(TestModRef.preloads["Hatchling"]);
                    
                    //Directly modify hatchling states and bools by getting a reference to its unique state machine component
                    var hatchComponent = hatchlingObj.GetComponent<KnightHatchling>();
                    hatchComponent.FsmQuickSpawn();
                    hatchlingObj.transform.position = spawnPosition;
                    hatchlingObj.SetActive(true);
                    */

                    //Increment Hatchling count
                    //hatchlingCount += 1;
                }
                else
                {
                    TestModRef.Log("Too many hatchlings");
                }
            }
        }
    }
}
