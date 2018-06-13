using UnityEngine;
using Search_Shell.Grid;

namespace Search_Shell.Game
{
    public class LevelProperties : MonoBehaviour
    {
        public GridObject selectedObj;
        public string nextLevel;

        public float scale;

        public Vector3 offset;

        public AK.Wwise.Switch _switch;
    }
}