using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Controllers.Animation;
using System;

namespace Search_Shell.Grid
{
  public class GridManager : MonoBehaviour
  {

    public bool debug = true;
    public Vector2 dims;

    public int maxGravityInterations = 20;

    private HashSet<GridObject> affectingObjects = new HashSet<GridObject>();
    HashSet<GridObject> objects = new HashSet<GridObject>();
    Dictionary<Vector3, GridObject> position2Object = new Dictionary<Vector3, GridObject>();

    private HashSet<IGridEvents> listeners = new HashSet<IGridEvents>();
    IEnumerator Start()
    {

      yield return new WaitForFixedUpdate();

      GridObject[] objs = GetComponentsInChildren<GridObject>();

      foreach (GridObject obj in objs)
      {
        obj.SnapPosition();
        obj.CalculateVolume();
        // Debug.Log("Registered:" + obj.name);
        RegisterObject(obj);
      }
    }

    void OnDrawGizmos()
    {

      if (!debug) return;

      Vector3 local = transform.position + Vector3.one / 2f;
      for (int i = -(int)dims.x; i < dims.x; i++)
      {
        for (int j = -(int)dims.y; j < dims.y; j++)
        {
          Gizmos.DrawLine(new Vector3(-dims.x, j, i) + local, local + new Vector3(dims.x, j, i));
          Gizmos.DrawLine(new Vector3(i, j, -dims.y) + local, local + new Vector3(i, j, dims.y));
        }
      }
      Gizmos.color = Color.white;
      foreach (Vector3 pos in position2Object.Keys)
      {
        Gizmos.DrawCube(transform.position + pos, Vector3.one * 0.5f);
      }

    }

    public void AddListener(IGridEvents lstnr)
    {
      listeners.Add(lstnr);
    }

    public void RemoveListener(IGridEvents lstnr)
    {
      listeners.Remove(lstnr);
    }

    public void CallListeners(Action<IGridEvents> action)
    {
      foreach (IGridEvents e in listeners)
      {
        action(e);
      }
    }

    public GridObject AssignObjectToPosition(GridObject obj, Vector3 pos)
    {

      GridObject solid;
      //Debug.Log(pos.x + " - " + pos.y  + " - " + pos.z);
      if (position2Object.TryGetValue(pos, out solid))
        return solid;

      position2Object.Add(pos, obj);

      return null;
    }

    public void RemoveObjectFromPosition(GridObject obj, Vector3 pos)
    {
      if (position2Object.ContainsKey(pos))
        if (position2Object[pos] == obj)
          position2Object.Remove(pos);
    }

    public void ClearObject(GridObject obj, List<Vector3> positions)
    {
      foreach (Vector3 position in positions)
      {
        RemoveObjectFromPosition(obj, position);
      }
    }

    public GridObject CheckObject(Vector3 pos)
    {
      GridObject obj;

      position2Object.TryGetValue(pos, out obj);

      return obj;
    }

    public HashSet<GridObject> CheckCollision(GridObject obj, List<Vector3> positions)
    {

      HashSet<GridObject> objs = new HashSet<GridObject>();
      foreach (Vector3 position in positions)
      {
        GridObject res = CheckObject(position);
        if (res == null || res == obj) continue;
        objs.Add(res);
      }

      return objs;
    }

    public void ClearObject(GridObject obj)
    {
      ClearObject(obj, obj.GetVolumePositions());
    }

    public void SlideObject(GridObject obj, Vector3 movement)
    {
      ClearObject(obj);
      List<Vector3> calculatedMovement = obj.CalculateSlide(movement);
      if (RegisterObject(obj, calculatedMovement))
      {
        obj.Slide(movement);
      }
      else
      {
        ClearObject(obj, calculatedMovement);
        RegisterObject(obj);
      }
    }

    private bool CheckGround(GridObject obj, Vector3 dir)
    {
      return CheckCollision(obj, obj.CalculateSlide(dir)).Count > 0;
    }

    private void FinishGravityAnimation(GridObject obj, Vector3 dir)
    {
      affectingObjects.Remove(obj);
      obj.Slide(dir);

      if (affectingObjects.Count == 0)
      {
        CallListeners(e => e.OnFinishedGravity());
      }
    }

    public void LoadNextLevel()
    {
      CallListeners(e => e.OnLoadNextLevel());
    }

    public void LoadNextSubLevel(string sublevel)
    {
      CallListeners(e => e.OnLoadNextSubLevel(sublevel));
    }

    private void MoveObject(GridObject obj, Vector3 dir)
    {
      affectingObjects.Add(obj);
      Debug.Log(obj.name + "DIR:" + dir + "POS:" + obj.finalPosition);
      ClearObject(obj);
      obj.Slide(dir);
      Debug.Log("DIR:" + dir + "POS:" + obj.finalPosition);
      RegisterObject(obj);
      obj.Slide(-dir);
      Debug.Log("DIR:" + dir + "POS:" + obj.finalPosition);
      LinearAnimation anim = obj.GetComponent<LinearAnimation>();
      anim.Animate(dir, () => FinishGravityAnimation(obj, dir));
    }

    public bool PushObject(GridObject obj, Vector3 dir, Action endAction)
    {
      if (obj.properties.isStatic)
      {
        return false;
      }

      HashSet<GridObject> possibleColisions = CheckCollision(obj, obj.CalculateSlide(dir));

      foreach (GridObject otherObj in possibleColisions)
      {
        if (!PushObject(otherObj, dir, endAction))
        {
          return false;
        }
      }

      affectingObjects.Add(obj);
      ClearObject(obj);
      obj.Slide(dir);
      RegisterObject(obj);
      obj.Slide(-dir);
      LinearAnimation anim = obj.GetComponent<LinearAnimation>();
      anim.Animate(dir, () =>
      {
        affectingObjects.Remove(obj);
        obj.Slide(dir);
        Debug.Log(affectingObjects.Count);
        if (affectingObjects.Count == 0)
          endAction();
      });

      return true;
    }

    public bool VerifyGravity(HashSet<GridObject> movedObjs)
    {

      List<GridObject> objs = new List<GridObject>(movedObjs);
      objs.Sort((o1, o2) =>
      {
        if (o1.finalPosition.y > o2.finalPosition.y) return 1;
        else if (o1.finalPosition.y < o2.finalPosition.y) return -1;
        return 0;
      });

      bool moved = false;
      foreach (GridObject obj in objs)
      {
        Debug.Log("Analysing:" + obj.name);
        int n = 0;
        while (++n <= maxGravityInterations)
        {
          if (CheckGround(obj, Vector3.down * n))
          {
            if (n == 1) break;
            MoveObject(obj, Vector3.down * (n - 1));
            moved = true;
            break;
          }
        }
      }
      return moved;
    }

    public HashSet<GridObject> GetMovingObjects()
    {
      HashSet<GridObject> objs = new HashSet<GridObject>();

      foreach (GridObject obj in objects)
      {
        if (!obj.properties.isStatic)
          objs.Add(obj);
      }

      return objs;
    }

    public HashSet<GridObject> GetObjects()
    {
      HashSet<GridObject> objs = new HashSet<GridObject>();

      foreach (GridObject obj in objects)
      {
        objs.Add(obj);
      }

      return objs;
    }
		public HashSet<GridObject> GetObjectsRaw()
    {

      return new HashSet<GridObject>(GetComponentsInChildren<GridObject>());
    }

    public bool RegisterObject(GridObject obj, List<Vector3> positions)
    {

      foreach (Vector3 position in positions)
      {
        GridObject existing = AssignObjectToPosition(obj, position);
        if (existing != null)
        {
          Debug.Log(existing.name + " " + position);
          ClearObject(obj);
          return false;
        }
      }

      objects.Add(obj);
      return true;
    }

    public bool RegisterObject(GridObject obj)
    {
      if (!RegisterObject(obj, obj.GetVolumePositions()))
      {
        Debug.Log(obj.name);
        Debug.LogError("Cannot Assign Object to this position! Overlap!!");
        return false;
      }
      return true;
    }

    void Update()
    {

    }
  }
}