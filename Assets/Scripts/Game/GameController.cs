﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Grid;
using Search_Shell.Controllers;
using Search_Shell.Controllers.Detector;
using Search_Shell.Controllers.Movement;
using Search_Shell.Game.Controll;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

namespace Search_Shell.Game
{

    public class GameController : MonoBehaviour, IMovementListener, IGridEvents
    {

        public GameObject music;

        public AK.Wwise.Event[] defaultSoulEvents;

        public SkyboxEffect skyboxEffect;

        public AK.Wwise.Switch defaultSwitch;
        public AK.Wwise.State defaultState;

        [Header("FX")]
        public Color highlighted;
        public Color selected;
        public Color subSelected;
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

        private HashSet<GridObject> nearObjects = new HashSet<GridObject>();
        private List<GridObject> nearObjectsList = new List<GridObject>();

        private HashSet<GridObject> movedObjects = new HashSet<GridObject>();

        private SaveManager saveManager;

        private CameraFollow cameraFollow;

        public bool canControll = true;

        private int subSelectedObj = 0;

        void Start()
        {
            saveManager = new SaveManager(this);
            SetupCamera();
            canControll = false;
            PostProcessVolume volume = GameObject.Find("PostProcessGlobal").GetComponent<PostProcessVolume>();
            volume.profile.TryGetSettings(out skyboxEffect);
            skyboxEffect.blend.value = 0; 
            cameraFollow = subLevelCamera.GetComponent<CameraFollow>();
        }

        void OnDrawGizmos()
        {
        }

       public void SetCanControl(bool active) {
            canControll = active;
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

        public void PlaySwitch(AK.Wwise.Switch _switch){
            AkSoundEngine.SetSwitch((uint)_switch.groupID, (uint)_switch.ID, music);
        }

        public void PlayState(AK.Wwise.State _state){
            AkSoundEngine.SetState((uint)_state.groupID, (uint)_state.ID);
        }

        public void Reset(){
            subLevelCamera.transform.parent = null;
            levelCamera.transform.parent = null;
            ClearSave();
            ClearLevel(level);
            ClearLevel(subLevel);
            PlayState(defaultState);
            PlaySwitch(defaultSwitch);
            LoadStartingLevel("Box/Introduction");
            cameraFollow.enabled = true;
            canControll = true;
        }

        public void ClearSave(){
            saveManager = new SaveManager(this);
        }

        public void Undo ()
        {
            saveManager.Undo();
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
            AkSoundEngine.SetSwitch((uint)properties._switch.groupID, (uint)properties._switch.ID, music);
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

            SoulState state = obj.GetComponent<SoulState>();

            AK.Wwise.Event _event = (state != null? state._event : defaultSoulEvents[UnityEngine.Random.Range(0,defaultSoulEvents.Length)]);
            _event.Post(music); 
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
                hl.subSelected = subSelected;
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

            nearObjectsList = new List<GridObject>(nearObjects);

            subSelectedObj = 0;
            UpdateSubSelected(0);
        }

        void UpdateSubSelected(int increase = 0){
            int old = subSelectedObj;
            subSelectedObj+=increase;
            if(nearObjectsList.Count <= 0) return;
            subSelectedObj %= nearObjectsList.Count;

            if(subSelectedObj != old && old < nearObjectsList.Count){
                GetHighLighter(nearObjectsList[old]).SetSelected(HighLight.Highlighted);
            }

            GetHighLighter(nearObjectsList[subSelectedObj]).SetSelected(HighLight.SubSelected);
        }

        public void DisableHighlights() {
           UpdateMaterial(HighLight.None);
        }

        void SwitchObject(GridObject obj2 = null)
        {
            RaycastHit hit;
            Ray r = subLevelCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out hit) || obj2)
            {
                GridObject obj = obj2 != null? obj2 : hit.collider.GetComponent<GridObject>();

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
 

            if (Input.GetKeyDown("u") || Input.GetButtonDown("B"))
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

            if (Input.GetMouseButtonDown(0))
                SwitchObject();

            if(Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.LeftControl)){
                SwitchObject(nearObjectsList.Count > subSelectedObj? nearObjectsList[subSelectedObj] : null);
            }

            if(Input.GetButtonDown("X") || Input.GetKeyDown(KeyCode.Space)){
               UpdateSubSelected(1); 
            }

            Vector3 input = new Vector3(
                Input.GetAxisRaw("Horizontal"),
                0,
                Input.GetAxisRaw("Vertical"));

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

        private void SetSkyboxEffect(float f){
            if(skyboxEffect != null)
                skyboxEffect.blend.value = f;
        }

        IEnumerator InverseCameraTansition(String sublevelName)
        {
            canControll = false;
            yield return new WaitForFixedUpdate();
            CameraFollow cam = subLevelCamera.GetComponent<CameraFollow>();
            cam.animating = true;
            
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
                SetSkyboxEffect(1f - time / transitionTime);
                cam.radius = radius + cameraCurve.Evaluate(time / transitionTime) * diff;
                yield return new WaitForFixedUpdate();
            }
            SetSkyboxEffect(0);
            cam.radius = radius + cameraCurve.Evaluate(time / transitionTime) * diff;
            canControll = true;
            cam.animating = false;
        }

        IEnumerator CameraTransition()
        {
            canControll = false;
            CameraFollow cam = subLevelCamera.GetComponent<CameraFollow>();
            cam.animating = true;
            LevelProperties subLevelProperties = subLevel.GetComponent<LevelProperties>();
            cam.SetTransform(subLevel.transform);
            float time = 0;

            float radius = cam.radius;
            float diff = subLevelProperties.scale * defaultCameraZoom - radius;

            while (time < transitionTime)
            {
                time += Time.fixedDeltaTime;
                SetSkyboxEffect(time / transitionTime);
                cam.radius = radius + cameraCurve.Evaluate(time / transitionTime) * diff;
                yield return new WaitForFixedUpdate();
            }
            SetSkyboxEffect(0);

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