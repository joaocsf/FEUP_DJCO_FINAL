using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Grid;

namespace Search_Shell.Controllers
{
	[RequireComponent(typeof(GridObject))]
    public class BaseController: MonoBehaviour
    {
        private GridManager _gridManager;
		private GridObject _obj;

        public GridManager gridManager {
            get { 
                if(!_gridManager) _gridManager = FindGridManager(); 
                return _gridManager; }
        }
        public GridObject obj {
            get {
                if(!_obj) _obj = GetComponent<GridObject>(); return _obj;
            }
        }
        private GridManager FindGridManager() {
            Transform t = transform.parent;
            GridManager manager = null;

            while((manager = t.GetComponent<GridManager>()) == null)
                t = t.parent;
            
            return manager;
        }
    }
}