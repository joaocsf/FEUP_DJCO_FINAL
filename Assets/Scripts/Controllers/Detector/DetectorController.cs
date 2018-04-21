using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Search_Shell.Grid;
using Search_Shell.Helper;
using Search_Shell.Controllers;
using System;

namespace Search_Shell.Controllers.Detector {

	public abstract class DetectorController : BaseController {
    public abstract HashSet<GridObject> NearObjects();
  }
}