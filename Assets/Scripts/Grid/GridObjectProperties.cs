﻿using System;
using System.Collections;
using System.Collections.Generic;
using Search_Shell.Helper;
using UnityEngine;

namespace Search_Shell.Grid
{
    [System.Flags]
    public enum SurfaceType {
        None,
        Grass,
        Wood
    }

    [Serializable]
	public class GridObjectProperties{
        public bool canControll = false; 
        public bool isStatic = true;



        public SurfaceType surface;
	}
}
