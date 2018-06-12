using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GrassFX : MonoBehaviour {

	public Mesh grass;

	public Material grassMat;
	
	public float unitsPerMeter = 1.0f;

	public Vector3 rotationOffset;

	public Vector2 offset;
	public Vector3 randomize;

	public Vector3 scale;

	private Vector2 min;
	private Vector2 max;

	private RenderTexture rt;
	private Material mat;
	private Material stampDrawer;
	public Shader stampShader;

	void Start () {
		stampDrawer = new Material(stampShader);	
		GenerateMesh();
		rt = new RenderTexture(128,128,0,RenderTextureFormat.RGB565);
		mat = GetComponent<Renderer>().material;
		mat.SetTexture("_Map", rt);
		mat.SetVector("_Bounds", new Vector4(min.x, min.y, max.x, max.y));
	}
	public void Paint(Vector3 xy){
		xy = transform.InverseTransformPoint(xy);
		Debug.Log("Painting");
		Vector2 distance = max - min;		
		Vector2 relPos = new Vector2(xy.x, xy.z) - min;
		Vector3 mm = Vector2.one;
		mm.x = distance.x;
		mm.y = distance.y;
		float dist = mm.magnitude*2;
		xy.x = relPos.x/distance.x;
		xy.y = relPos.y/distance.y;
		stampDrawer.SetVector("_Coord", new Vector4(xy.x, xy.y,mm.y,mm.x));
		Debug.Log(xy + " - " + mm);
		stampDrawer.SetVector("_Distance", new Vector4(1, 1,0,0));
		RenderTexture tmp = RenderTexture.GetTemporary(128,128,0, RenderTextureFormat.RGB565);
		Graphics.Blit(rt, tmp);
		Graphics.Blit(tmp, rt, stampDrawer);
		RenderTexture.ReleaseTemporary(tmp);
	}

	public void GenerateMesh(){
		List<CombineInstance> instances = new List<CombineInstance>();
		BoxCollider[] colliders = GetComponents<BoxCollider>();

		foreach(BoxCollider col in colliders)
			PopulateCollider(col, ref instances);

		Mesh m = new Mesh();
		m.CombineMeshes(instances.ToArray());
		GetComponent<MeshFilter>().mesh = m;
		Material mat = new Material(grassMat);
		GetComponent<MeshRenderer>().material = mat;
	}

	void UpdateMinMax(Vector3 pos){

		min.x = Mathf.Min(min.x, pos.x);
		min.y = Mathf.Min(min.y, pos.z);

		max.x = Mathf.Max(max.x, pos.x);
		max.y = Mathf.Max(max.y, pos.z);
	}

	void PopulateCollider(BoxCollider collider, ref List<CombineInstance> instances){
		Bounds bounds = collider.bounds;
		float distance = bounds.size.x;
		Vector2 spacement = Vector2.zero;
		spacement.x = bounds.size.x * unitsPerMeter;
		spacement.y = bounds.size.z * unitsPerMeter;

		spacement.x = (bounds.size.x - 2 * offset.x)/spacement.x;
		spacement.y = (bounds.size.z - 2 * offset.y)/spacement.y;

		int maxIter = 200;
		for(float i = bounds.min.x + offset.x; i <= bounds.max.x - offset.x;){
			for(float j = bounds.min.z + offset.y; j <= bounds.max.z - offset.y;){
				if(maxIter -- <= 0) return;
				CombineInstance instance = new CombineInstance();
				Vector3 position = new Vector3(i, bounds.max.y, j);
				position = transform.InverseTransformPoint(position);
				instance.mesh = grass;
				Vector3 randomRotation = new Vector3(Random.Range(0f,360f), Random.Range(0f,360f), Random.Range(0f,360f));
				UpdateMinMax(position);
				randomRotation.x *= randomize.x;
				randomRotation.y *= randomize.y;
				randomRotation.z *= randomize.z;
				instance.transform = Matrix4x4.Translate(position) * Matrix4x4.Scale(scale) *  Matrix4x4.Rotate(Quaternion.Euler(rotationOffset + randomRotation)) * Matrix4x4.identity;
				instances.Add(instance);
				if(j != bounds.max.z - offset.y)
					j=Mathf.Clamp(spacement.y+j, bounds.min.z + offset.y, bounds.max.z - offset.y);
				else break;
			}

			if(i != bounds.max.x - offset.x)
				i=Mathf.Clamp(i+spacement.x, bounds.min.x + offset.x, bounds.max.x - offset.x);
			else break;
		}
	}
	
	void Update () {

	}
}
