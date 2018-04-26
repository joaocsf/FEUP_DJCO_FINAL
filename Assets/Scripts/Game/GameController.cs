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

    public class GameController : MonoBehaviour, IMovementListener, IGridEvents
    {

        public Material highlighted;
        public Material selected;

        public GridManager level;

        public string levelLayer = "Level";
        public string subLevelLayer = "SubLevel";

        public string startingLevel = "";
        public float defaultCameraZoom = 5;
        public AnimationCurve cameraCurve;


        public float transitionTime = 1;
        public GridManager subLevel;

        private GridObject levelObject;
        private GridObject subLevelObject;

        public Camera levelCamera;
        public Camera subLevelCamera;

        private UIManager UIManager;

        private Vector3 lastInput;

        private HashSet<GridObject> nearObjects = new HashSet<GridObject>();

        private HashSet<GridObject> movedObjects = new HashSet<GridObject>();

        private Stack<MoveTurn> turns = new Stack<MoveTurn>();

        private Dictionary<string, MoveTurn> levelsSave = new Dictionary<string, MoveTurn>();

        private struct Move
        {
            public Vector3 initialPosition;
            public Vector3 initialAngles;

            public Move(Vector3 initialPosition, Vector3 initialAngles)
            {
                this.initialPosition = initialPosition;
                this.initialAngles = initialAngles;
            }
        }

        private struct MoveTurn
        {
            public Dictionary<GridObject, Move> movements;
            public GridObject current;

            public MoveTurn(GridObject subLevelObject, Dictionary<GridObject, Move> movements)
            {
                this.movements = movements;
                this.current = subLevelObject;
            }
        }

        private bool canControll = true;

        void Start()
        {
            SetupCamera();
            UIManager = GameObject.FindObjectOfType<UIManager>();
        }

        void OnDrawGizmos()
        {
        }

        void SetupCamera()
        {
            /* 
			levelCamera.transform.parent = levelObject.transform;
			subLevelCamera.transform.parent = subLevel.transform;
			subLevelCamera.transform.localPosition = Vector3.zero + Vector3.one;
			subLevelCamera.GetComponent<CameraFollow>().SetTransform(subLevelObject.transform);
			subLevelCamera.GetComponent<CameraFollow>().radius = defaultCameraZoom;
			*/
            LoadStartingLevel(startingLevel);
        }

        public void LoadStartingLevel(string name)
        {
            GridManager sublevel = LoadLevel(name);

            SetSubLevel(sublevel);
            LevelProperties properties = sublevel.GetComponent<LevelProperties>();

            GridManager level = LoadLevel(properties.nextLevel);
            SetLevel(level);
        }



        public void SetLevel(GridManager level, GridObject levelObj = null)
        {
            this.level = level;
            LevelProperties levelProperties = level.GetComponent<LevelProperties>();
            levelProperties.selectedObj = levelObj ? levelObj : levelProperties.selectedObj;
            this.levelObject = level.GetComponent<LevelProperties>().selectedObj;
            if (levelObject != null)
                levelCamera.transform.parent = levelObject.transform;

            SetLayer(level.transform,
                obj =>
                {
                    if (obj.GetComponent<Renderer>())
                    {
                        obj.gameObject.layer = LayerMask.NameToLayer(levelLayer);
                    }
                });
        }

        private void SetLayer(Transform transform, Action<Transform> action)
        {
            foreach (Transform t in transform)
            {
                action(t);
                SetLayer(t, action);
            }
        }

        public void SetSubLevel(GridManager subLevel)
        {
            if (this.subLevel != null)
                this.subLevel.RemoveListener(this);

            turns.Clear();


            this.subLevel = subLevel;
            LevelProperties properties = subLevel.GetComponent<LevelProperties>();
            this.subLevelObject = properties.selectedObj;
            subLevelCamera.transform.parent = this.subLevel.transform;
            levelCamera.GetComponent<ScreenCapture>().scale = properties.scale;
            subLevelCamera.GetComponent<SkyboxHandler>().SetOpacity(0);
            subLevel.AddListener(this);
            ControllObject(subLevelObject);
            SetLayer(subLevel.transform,
                obj =>
                {
                    if (obj.GetComponent<Renderer>())
                    {
                        obj.gameObject.layer = LayerMask.NameToLayer(subLevelLayer);
                    }
                });
        }

        public void ControllObject(GridObject obj)
        {
            if (subLevelObject != null)
                GetHighLighter(subLevelObject).SetSelected(HighLight.None);
            subLevelObject = obj;
            subLevelCamera.GetComponent<CameraFollow>().SetTransform(subLevelObject.transform);
            UpdateReachableObjects();
            GetHighLighter(obj).SetSelected(HighLight.Selected);
            IControllEvents[] events = subLevelObject.GetComponents<IControllEvents>();
            foreach (IControllEvents listener in events)
            {
                listener.OnControll();
            }
        }
        private HighLighter GetHighLighter(GridObject obj)
        {
            HighLighter hl = obj.GetComponent<HighLighter>();
            if (hl == null)
            {
                hl = obj.gameObject.AddComponent<HighLighter>();
                hl.highlighted = highlighted;
                hl.selected = selected;
            }
            return hl;
        }
        void UpdateMaterial(HighLight highlight)
        {
            foreach (GridObject obj in nearObjects)
            {
                GetHighLighter(obj).SetSelected(highlight);
            }
        }

        void UpdateReachableObjects()
        {
            UpdateMaterial(HighLight.None);
            DetectorController detect = subLevelObject.GetComponent<DetectorController>();
            if (detect == null) return;
            nearObjects = detect.NearObjects();
            UpdateMaterial(HighLight.Highlighted);
        }

        void SwitchObject()
        {
            RaycastHit hit;
            Ray r = subLevelCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out hit))
            {
                GridObject obj = hit.collider.GetComponent<GridObject>();
                if (nearObjects.Contains(obj))
                {
                    SaveTurn(subLevelObject, subLevel.GetObjects());
                    ControllObject(obj);
                }
            }
        }

        void Update()
        {

            if (!canControll) return;

            if (Input.GetKeyDown("escape"))
            {
                UIManager.ToggleMenu();
            }

            if (Input.GetKeyDown("u"))
            {
                Undo();
            }

            if (Input.GetKeyDown("k"))
            {
                Save();
            }
            if (Input.GetKeyDown("l"))
            {
                Load();
            }

            Application.Quit();

            if (Input.GetMouseButtonDown(0))
                SwitchObject();

            Vector3 input = new Vector3(
                Input.GetAxisRaw("Horizontal"),
                0,
                Input.GetAxisRaw("Vertical"));

            if (input == lastInput) return;
            lastInput = input;
            if (input.magnitude == 0) return;
            input = subLevelCamera.GetComponent<CameraFollow>().GetPlaneDirection(input);
            Debug.DrawLine(subLevelCamera.transform.position, subLevelCamera.transform.position + input, Color.black, 2);
            input = subLevel.transform.InverseTransformDirection(input);
            input.y = 0;
            input = input.normalized;
            if (Mathf.Abs(input.x) > 0.7f)
            {
                input.x = Mathf.Sign(input.x);
                input.z = 0;
            }
            else
            {
                input.z = Mathf.Sign(input.z);
                input.x = 0;
            }

            if ((int)input.sqrMagnitude != 0)
            {
                input.x = Mathf.Round(input.x);
                input.y = Mathf.Round(input.y);
                input.z = Mathf.Round(input.z);
            }

            movedObjects.Clear();

            MovementController controller = subLevelObject.GetComponent<MovementController>();
            if (controller == null) return;
            controller.SetListener(this);

            if (controller.Animating) return;

            HashSet<GridObject> colls = controller.Move(input, ref movedObjects);
            if (colls.Count > 0) return;
            canControll = false;
            movedObjects = subLevel.GetMovingObjects();

            SaveTurn(subLevelObject, subLevel.GetObjects());
        }

        public void OnFinishedMovement()
        {
            canControll = !subLevel.VerifyGravity(movedObjects);
            if (canControll)
                UpdateReachableObjects();
        }

        public void OnFinishedGravity()
        {
            canControll = true;
            UpdateReachableObjects();
        }

        public void OnLoadNextLevel()
        {
            StartCoroutine(CameraTransition());
        }

        public void OnLoadNextSubLevel(String level)
        {
            StartCoroutine(InverseCameraTansition(level));
        }

        public GridManager LoadLevel(string levelName)
        {
            GameObject level = (GameObject)Resources.Load("Levels/" + levelName);
            level = GameObject.Instantiate(level);

            return level.GetComponent<GridManager>();
        }

        public void ClearLevel(GridManager obj)
        {
            Destroy(obj.gameObject);
        }

        IEnumerator InverseCameraTansition(String sublevelName)
        {
            canControll = false;
            yield return new WaitForFixedUpdate();
            CameraFollow cam = subLevelCamera.GetComponent<CameraFollow>();
            SkyboxHandler handler = subLevelCamera.GetComponent<SkyboxHandler>();

            ClearLevel(level);
            SetLevel(subLevel, subLevelObject);
            SetSubLevel(LoadLevel(sublevelName));

            LevelProperties subLevelProperties = subLevel.GetComponent<LevelProperties>();

            cam.radius *= subLevelProperties.scale;
            cam.ForcePositionCamera(subLevelObject.transform.position, 999);
            float radius = cam.radius;
            float diff = defaultCameraZoom - cam.radius;

            float time = 0;

            while (time < transitionTime)
            {
                time += Time.fixedDeltaTime;
                handler.SetOpacity(1f - time / transitionTime);
                cam.radius = radius + cameraCurve.Evaluate(time / transitionTime) * diff;
                yield return new WaitForFixedUpdate();
            }
            handler.SetOpacity(0);
            canControll = true;
        }

        IEnumerator CameraTransition()
        {
            canControll = false;
            CameraFollow cam = subLevelCamera.GetComponent<CameraFollow>();
            SkyboxHandler handler = subLevelCamera.GetComponent<SkyboxHandler>();
            LevelProperties subLevelProperties = subLevel.GetComponent<LevelProperties>();
            cam.SetTransform(subLevel.transform);
            float time = 0;

            float radius = cam.radius;
            float diff = subLevelProperties.scale * defaultCameraZoom - radius;

            while (time < transitionTime)
            {
                time += Time.fixedDeltaTime;
                handler.SetOpacity(time / transitionTime);
                cam.radius = radius + cameraCurve.Evaluate(time / transitionTime) * diff;
                yield return new WaitForFixedUpdate();
            }
            handler.SetOpacity(0);

            float dist = levelCamera.transform.localPosition.magnitude;
            subLevelCamera.transform.position = levelCamera.transform.position;
            cam.radius = defaultCameraZoom;

            LevelProperties levelProperties = level.GetComponent<LevelProperties>();
            GridManager sub = subLevel;
            SetSubLevel(level);
            ClearLevel(sub);
            SetLevel(LoadLevel(levelProperties.nextLevel));

            canControll = true;
        }

        public void SaveTurn(GridObject current, HashSet<GridObject> movedObjects)
        {

            Dictionary<GridObject, Move> movements = new Dictionary<GridObject, Move>();
            foreach (GridObject obj in movedObjects)
            {
                Move move = new Move(obj.finalPosition, obj.finalAngles);
                movements.Add(obj, move);
            }
            turns.Push(new MoveTurn(current, movements));
        }

        public void Undo()
        {
            if (turns.Count == 0) return;
            MoveTurn turn = turns.Pop();
            Dictionary<GridObject, Move> movements = turn.movements;

            foreach (GridObject obj in movements.Keys)
            {
                obj.transform.parent.GetComponent<GridManager>().ClearObject(obj);
            }

            foreach (GridObject obj in movements.Keys)
            {
                Move move = movements[obj];
                obj.finalAngles = move.initialAngles;
                obj.finalPosition = move.initialPosition;
                obj.transform.localEulerAngles = move.initialAngles;
                obj.transform.localPosition = move.initialPosition;
                obj.transform.parent.GetComponent<GridManager>().RegisterObject(obj);
            }
            ControllObject(turn.current);
        }

        public void Save()
        {
            HashSet<GridObject> movedObjects = subLevel.GetMovingObjects();
            Dictionary<GridObject, Move> movements = new Dictionary<GridObject, Move>();
            foreach (GridObject obj in movedObjects)
            {
                Move move = new Move(obj.finalPosition, obj.finalAngles);
                movements.Add(obj, move);
            }
            levelsSave[subLevel.name] = new MoveTurn(subLevelObject, movements);
        }

        public void Load()
        {
            if (!levelsSave.ContainsKey(subLevel.name)) return;
            MoveTurn turn = levelsSave[subLevel.name];
            Dictionary<GridObject, Move> movements = turn.movements;

            foreach (GridObject obj in movements.Keys)
            {
                obj.transform.parent.GetComponent<GridManager>().ClearObject(obj);
            }

            foreach (GridObject obj in movements.Keys)
            {
                Move move = movements[obj];
                obj.finalAngles = move.initialAngles;
                obj.finalPosition = move.initialPosition;
                obj.transform.localEulerAngles = move.initialAngles;
                obj.transform.localPosition = move.initialPosition;
                obj.transform.parent.GetComponent<GridManager>().RegisterObject(obj);
            }
            ControllObject(turn.current);
        }
    }
}