using UnityEngine;
using System.Collections;

public class StoneBehavior : MonoBehaviour {

	public Color ObjectColor;
	
	private Color currentColor;
	private Material materialColored;

	// Use this for initialization
	void Start () {
		if (ObjectColor != currentColor)
		{
			// stop the leaks
			if (materialColored != null)
			{
				UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(materialColored));
			}
			
			//create a new material
			materialColored = new Material(Shader.Find("Diffuse"));
			materialColored.color = currentColor = Color.red;
			this.renderer.material = materialColored;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
