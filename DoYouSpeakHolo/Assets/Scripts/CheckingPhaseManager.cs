﻿using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using UnityEngine;
using static EventManager;

public class CheckingPhaseManager : MonoBehaviour {

    private List<string> SceneObjects;
    private ObjectPooler objectPooler;
    private int findObjectCounter = 0;

    void Start() {
        objectPooler = ObjectPooler.GetPooler();
        SceneObjects = objectPooler.GetObjects();
        StartListening(Triggers.CheckingPhase, HandleStartCheckingPhase);
        StartListening(Triggers.FoundObject, FoundObject);
    }



    private void Update() {


    }

    private void HandleStartCheckingPhase() {
        CheckingPhase();
    }

    //Spawn the objects in random order and ask the user to pick a specific one
    private void CheckingPhase() {

        Debug.Log("Entered in CheckingPhase");
        CreateAllObjectsAndDisplayInRandomOrder();
        //Shuffle the collection again
        SceneObjects = Shuffle(SceneObjects);
        SetTargetObject();
    }

   
    private void FoundObject() {
        //increase the counter
        findObjectCounter++;
        if (findObjectCounter == SceneObjects.Count - 1) {
            //Objects are finished
        }
        else {
            SetTargetObject();
        }
    }

    //Set the object that the user has to find
    private void SetTargetObject() {

        //VA tells the User which object he needs to find to complete the task
        Debug.Log("Object to find " + SceneObjects[findObjectCounter]);

        //Set the current target
        objectPooler.GetPooledObject(SceneObjects[findObjectCounter]).GetComponent<FindObjectTask>().IsTarget = true;
    }

    private void CreateAllObjectsAndDisplayInRandomOrder() {

        //Shuffle the collection
        SceneObjects = Shuffle(SceneObjects);
        GameObject gameObj;
        //Define initial spawning position
        Vector3 startPosition = Positions.startPositionInlineFour;
        foreach (string obj in SceneObjects) {
            //Activate the object and attach to it the script for the task
            gameObj = objectPooler.ActivateObject(obj, startPosition);
            FindObjectTask task = gameObj.AddComponent<FindObjectTask>();
            gameObj.AddComponent<Interactable>().AddReceiver<InteractableOnPressReceiver>().OnPress.AddListener(() => task.Check() );
            startPosition += new Vector3(0.5f, 0, 0);
        }
    }

    //Stop listening to events and trigger the new phase
    private void End() {
        StopListening(Triggers.CheckingPhase, HandleStartCheckingPhase);

        //Trigger the new phase

    }

    //Randomize a List
    public List<string> Shuffle(List<string> list) {
        List<string> randomList = new List<string>();

        System.Random random = new System.Random();
        int randomIndex = 0;

        while (list.Count > 0) {
            randomIndex = random.Next(0, list.Count);
            randomList.Add(list[randomIndex]);
            list.RemoveAt(randomIndex);
        }

        return randomList;
    }
}
