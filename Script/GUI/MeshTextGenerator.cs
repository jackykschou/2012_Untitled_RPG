using UnityEngine;
using System.Collections;

/**
 * control the creation of message show on the screen which disappear over time
 **/

public class MeshTextGenerator : MonoBehaviour {
	
	//show the text in a fixed position relative to the transform
	public void ShowMessage(TextMesh text, Transform tran,  float x_offset = 0, float y_offset = 0)
	{	
		//find the position where the text comes out (perfer getting the center of the renderer or collider, 
		//if the transform doesn't have those compomenets, use the position of the tranform)
		Vector3 pos;
		if(tran.renderer != null)
		{
			pos = tran.renderer.bounds.center;
		}
		else if(tran.collider != null)
		{
			pos = tran.collider.bounds.center;
		}
		else
		{
			pos = tran.position;
		}
		pos += new Vector3(x_offset, y_offset, 0.0f);
		TextMesh text_instance = TextMesh.Instantiate(text, pos, Quaternion.identity) as TextMesh;
		text_instance.transform.parent = Camera.main.transform;
		text_instance.transform.rotation = Quaternion.identity;
		text_instance.renderer.enabled = true;
		StartCoroutine(StartFading(text_instance, 0.3f));
	}
	
	//show the text in a random position relative to the transform
	public void ShowMessageRandom(TextMesh text, Transform tran)
	{
		ShowMessage(text, tran, Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
	}
	
	//keep animating the text (fade towards camera the shrinking size)
	private IEnumerator StartFading(TextMesh text, float time)
	{
		while(time > 0)
		{
			text.transform.rotation = text.transform.parent.rotation; //make the text always facing the camera
			text.transform.Translate(0f, 0f, -0.001f); //move the text towards the camera
			text.characterSize *= 1.05f; //expand the size of the text
			time -= Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		Destroy(text);
	}
	
}
