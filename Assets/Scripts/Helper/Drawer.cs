
using System.Collections.Generic;
using UnityEngine;

namespace Search_Shell.Helper
{
    public class Drawer
    { 
        List<Drawable> drawables = new List<Drawable>();

        public void AddDrawable(Drawable drawable, int lifeTime = -1){
            drawable.lifeTime = lifeTime;
            drawables.Add(drawable);
        }

        public void Draw(){
            
            for(int i = 0; i < drawables.Count; i++){
                drawables[i].Draw();
                if(drawables[i].IsDead()){
                    drawables.RemoveAt(i--);
                }
            }

        }

    }

    public abstract class Drawable
    {
        public int lifeTime = -1; 
        public bool IsDead(){
            if(lifeTime < 0) return false;

            return --lifeTime == 0;
        }

        public void Draw(){
            OnDraw();
        }
        protected abstract void OnDraw();
    }

    public class DrawableCube : Drawable
    {
        public Vector3 position;
        public Vector3 size;
        public Color color;
        bool wire;

        public DrawableCube(Vector3 position) {
            Initialize(position, Vector3.one, Color.red);
        }

        public DrawableCube(Vector3 position, Color color){
            Initialize(position, Vector3.one, color);
        }
        public DrawableCube(Vector3 position, Vector3 size){
            Initialize(position, size, Color.red);
        }

        public DrawableCube(Vector3 position, Vector3 size, Color color, bool wire=true)
        {
            Initialize(position, size, color, wire);
        }

        private void Initialize(Vector3 position, Vector3 size, Color color, bool wire = true){
            this.position = position;
            this.size = size;
            this.color = color;
            this.wire = wire;
        }

        protected override void OnDraw()
        {
            Gizmos.color = color;

            if(wire)
                Gizmos.DrawWireCube(position, size);
            else
                Gizmos.DrawCube(position, size);
        }
    }

    public class DrawableLine : Drawable
    {
        Vector3 p1, p2;
        Color color;

        public DrawableLine(Vector3 p1, Vector3 p2){
            Initialize(p1, p2, Color.blue);
        }
        public DrawableLine(Vector3 p1, Vector3 p2, Color color){
            Initialize(p1, p2, color);
        }

        private void Initialize(Vector3 p1, Vector3 p2, Color color){
            this.p1 = p1;
            this.p2 = p2;
            this.color = color;
        }
        
        protected override void OnDraw()
        {
            Gizmos.color = color;
            Gizmos.DrawLine(p1, p2);
        }
    }


}