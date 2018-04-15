using UnityEngine;

namespace Search_Shell.Grid
{
    public class GridBounds
    {
        private bool initialized = false;
        public Vector3 min;
        public Vector3 max;

        public Vector3 size {
            get {return max - min;}
        }

        public Vector3 center {
            get {
                return (max + min)/2f;
            }
        }
        public void Encapsulate(Vector3 point){
            if(!initialized){
                min = max = point;
                initialized = true;
            }

            min.x = Mathf.Min(point.x, min.x);
            max.x = Mathf.Max(point.x, max.x);
            min.y = Mathf.Min(point.y, min.y);
            max.y = Mathf.Max(point.y, max.y);
            min.z = Mathf.Min(point.z, min.z);
            max.z = Mathf.Max(point.z, max.z);
        }
    }
}