using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Grid;
using Search_Shell.Controllers;
using Search_Shell.Controllers.Detector;
using Search_Shell.Controllers.Movement;
using Search_Shell.Game.Controll;
using System;
using UnityEngine.SceneManagement;

namespace Search_Shell.Game
{
  public class SaveManager
  {
    [Serializable]
    private struct Transformation
    {
      public Vector3 initialPosition;
      public Vector3 initialAngles;

      public Transformation(Vector3 initialPosition, Vector3 initialAngles)
      {
        this.initialPosition = initialPosition;
        this.initialAngles = initialAngles;
      }
    }

    [Serializable]
    private struct Turn
    {
      public Dictionary<String, Transformation> transformations;
      public String currentObjectName;

      public Turn(GridObject obj, Dictionary<GridObject, Transformation> movements)
      {
        this.transformations = new Dictionary<String, Transformation>();
        foreach (GridObject g in movements.Keys)
        {
          this.transformations.Add(g.name, movements[g]);
        }
        this.currentObjectName = obj.name;
      }

      public Turn(GridObject obj, Dictionary<String, Transformation> movements)
      {
        this.transformations = movements;
        this.currentObjectName = obj.name;
      }

      public Turn(String objectName, Dictionary<String, Transformation> movements)
      {
        this.transformations = movements;
        this.currentObjectName = objectName;
      }
    }

    [Serializable]
    private class TurnStack
    {
      public Stack<Turn> turns = new Stack<Turn>();
      public int turnNumber = 0;
    }

    private Dictionary<String, TurnStack> savedata = new Dictionary<String, TurnStack>();

    private GameController controller;

    public SaveManager(GameController gmc)
    {
      controller = gmc;
    }

    public void SaveTurn()
    {
      String level = controller.subLevel.name;
      if (!savedata.ContainsKey(level))
        savedata[level] = new TurnStack();


      Dictionary<String, Transformation> transformations = new Dictionary<String, Transformation>();
      foreach (GridObject obj in controller.subLevel.GetObjectsRaw())
      {
        Transformation transformation = new Transformation(obj.finalPosition, obj.finalAngles);
        transformations.Add(obj.name, transformation);
      }

      Turn turn = new Turn(controller.subLevelObject, transformations);
      savedata[level].turns.Push(turn);
    }

    public void LoadTurn(int turnNumber)
    {
      String level = controller.subLevel.name;
      if (!savedata.ContainsKey(level) || savedata[level].turns.Count == 0) return;

      TurnStack turnsStack = savedata[level];
      Stack<Turn> turns = turnsStack.turns;
      Turn turn;
      while (turns.Count > turnNumber)
        turn = turns.Pop();
      turn = turns.Pop();

      HashSet<GridObject> movingObjects = controller.subLevel.GetObjectsRaw();
      Dictionary<String, Transformation> transformations = turn.transformations;
      GridObject current = null;

      foreach (GridObject obj in movingObjects)
      {
        obj.transform.parent.GetComponent<GridManager>().ClearObject(obj);
      }

      foreach (GridObject obj in movingObjects)
      {
        // Get Transform
        Transformation transform = transformations[obj.name];

        //Check if is selected
        if (obj.name == turn.currentObjectName)
          current = obj;

        //Set Positions
        obj.finalAngles = transform.initialAngles;
        obj.finalPosition = transform.initialPosition;
        obj.transform.localEulerAngles = transform.initialAngles;
        obj.transform.localPosition = transform.initialPosition;
        obj.transform.parent.GetComponent<GridManager>().RegisterObject(obj);
      }
      Debug.LogWarning("SubLevel ========= " + controller.subLevel.name);
      Debug.LogWarning("(sub)level ========= " + controller.level.name);
      controller.ControllObject(current);
    }


    public void Undo()
    {
      String level = controller.subLevel.name;
      if (savedata.ContainsKey(level))
        LoadTurn(savedata[level].turns.Count);
    }

    public void Save()
    {
      String level = controller.subLevel.name;

      SaveTurn();
      savedata[level].turnNumber = savedata[level].turns.Count;

      Debug.LogWarning("Save  ====>" + savedata[level].turns.Peek().currentObjectName);
    }

    public void Load()
    {
      String level = controller.subLevel.name;
      if (savedata.ContainsKey(level))
        LoadTurn(savedata[level].turnNumber);
    }



    //Nao gosto
    public void LoadLevel()
    {
      String level = controller.level.name;
      if (!savedata.ContainsKey(level) || savedata[level].turns.Count == 0) return;

      TurnStack turnsStack = savedata[level];
      Stack<Turn> turns = turnsStack.turns;
      Turn turn;
      while (turns.Count > savedata[level].turnNumber)
        turn = turns.Pop();
      turn = turns.Pop();

      HashSet<GridObject> movingObjects = controller.level.GetObjectsRaw();
      Dictionary<String, Transformation> transformations = turn.transformations;
      GridObject current = null;

         Debug.LogWarning("Moving ========= " + movingObjects.Count);
      foreach (GridObject obj in movingObjects)
      {
        obj.transform.parent.GetComponent<GridManager>().ClearObject(obj);
      }

      foreach (GridObject obj in movingObjects)
      {
        // Get Transform
        Transformation transform = transformations[obj.name];

        //Check if is selected
        if (obj.name == turn.currentObjectName)
          current = obj;

        //Set Positions
        obj.finalAngles = transform.initialAngles;
        obj.finalPosition = transform.initialPosition;
        obj.transform.localEulerAngles = transform.initialAngles;
        obj.transform.localPosition = transform.initialPosition;
        obj.transform.parent.GetComponent<GridManager>().RegisterObject(obj);
      }
      Debug.LogWarning("expected ========= " + current);
      Debug.LogWarning("level ========= " + controller.level.name);
      controller.level.GetComponent<LevelProperties>().selectedObj = current;
    }
  }
}