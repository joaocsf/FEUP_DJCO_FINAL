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

        [Header("FX")]
        public Color highlighted;
        public Color selected;
        public Material selectedShaderMat;

        public GridManager level;

        public string levelLayer = "Level";
        public string subLevelLayer = "SubLevel";

        public string startingLevel = "";
        public float defaultCameraZoom = 5;
        public AnimationCurve cameraCurve;


        public float transitionTime = 1;
        public GridManager subLevel;

        private GridObject levelObject;
        public GridObject subLevelObject {get; private set;}

        public Camera levelCamera;
        public Camera subLevelCamera;

        private UIManager UIManager;

        private Vector3 lastInput;

        private HashSet<GridObject> nearObjects = new HashSet<GridObject>();

        private HashSet<GridObject> movedObjects = new HashSet<GridObject>();


        private SaveManager saveManager;

        private bool canControll = true;

        void Start()
        {
            saveManager = new SaveManager(this);
            SetupCamera();
            UIManager = GameObject.FindObjectOfType<UIManager>();
            
        }

        void OnDrawGizmos()
        {
        }

        void SetupCamera()
        {
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



            this.subLevel = subLevel;
            LevelProperties properties = subLevel.GetComponent<LevelProperties>();
            this.subLevelObject = properties.selectedObj;
            subLevelCamera.transform.parent = this.subLevel.transform;
            levelCamera.GetComponent<ScreenCapture>().scale = properties.scale;
            levelCamera.GetComponent<ScreenCapture>().offset = properties.offset;
            subLevelCamera.GetComponent<SkyboxHandler>().SetOpacity(0);
            subLevel.AddListener(this);
            subLevel.Initialize();
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
                GetHighLighter(subLevelObject).SetSelected(HighLight.Highlighted);
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
                hl.CopyMaterial(selectedShaderMat);
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
            DetectorController detect = subLevelObject.GetComponent<DetectorController>();
            if (detect == null) return;
            HashSet<GridObject> objs = detect.NearObjects();
            objs.Add(subLevelObject);
            nearObjects.RemoveWhere((obj) => objs.Contains(obj));
            UpdateMaterial(HighLight.None);
            objs.Remove(subLevelObject);
            nearObjects = objs;
            UpdateMaterial(HighLight.Highlighted);
        }

        void SwitchObject()
        {
            RaycastHit hit;
            Ray r = subLevelCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out hit))
            {
                GridObject obj = hit.collider.GetComponent<GridObject>();
                if(obj == null) return;
                if (nearObjects.Contains(obj))
                {
                    saveManager.SaveTurn();
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
                saveManager.Undo();
            }

            if (Input.GetKeyDown("k"))
            {
                saveManager.Save();
            }
            if (Input.GetKeyDown("l"))
            {
                saveManager.Load();
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

            saveManager.SaveTurn();
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
            if(levelName.Length == 0)
                return null;
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
            cam.animating = true;
            SkyboxHandler handler = subLevelCamera.GetComponent<SkyboxHandler>();
            
            saveManager.Save();
            ClearLevel(level);
            SetLevel(subLevel, subLevelObject);
            SetSubLevel(LoadLevel(sublevelName));

            saveManager.Load();
            saveManager.LoadLevel();


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
            cam.animating = false;
        }

        IEnumerator CameraTransition()
        {
            canControll = false;
            CameraFollow cam = subLevelCamera.GetComponent<CameraFollow>();
            cam.animating = true;
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
            saveManager.Save();
            SetSubLevel(level);
            ClearLevel(sub);
            SetLevel(LoadLevel(levelProperties.nextLevel));

            saveManager.Load();
            saveManager.LoadLevel();


            canControll = true;
            cam.animating = false;
        }

    }
}