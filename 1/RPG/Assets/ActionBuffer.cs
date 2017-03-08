using UnityEngine;
using System.Collections;

public class ActionBuffer : MonoBehaviour {
	private int[] actionBuffer;
	public int Action{ get { return actionBuffer[0]; }}
	private Quaternion[] rotationBuffer;
	public Quaternion RotationBuffer {
		get{ return rotationBuffer[0];}
	}

	public float bufferTime;
	void Awake () {
		actionBuffer = new int[1];
		for (int i =0; i<actionBuffer.Length; ++i) {
			actionBuffer [i] = 0;
		}
		rotationBuffer = new Quaternion[1];
		for (int i =0; i<actionBuffer.Length; ++i) {
			rotationBuffer [i] = Quaternion.identity;
		}
	}
	private IEnumerator BufferTimer(){
		yield return new WaitForSeconds(bufferTime);
		for (int i =0; i<actionBuffer.Length; ++i) {
			actionBuffer [i] = 0;
			rotationBuffer [i] = Quaternion.identity;
		}

	}

	public void ConsumeAction(){
		for (int i =0; i<actionBuffer.Length-1; ++i) {
			actionBuffer[i]=actionBuffer[i+1];
			rotationBuffer[i]=rotationBuffer[i+1];
		}
		actionBuffer[actionBuffer.Length-1]=0;
		rotationBuffer[actionBuffer.Length-1]= Quaternion.identity;
	}
	private void DebugBuffer(){
		for (int i =0; i<actionBuffer.Length; ++i) {
			Debug.Log (i + "   " + actionBuffer[i]);
		}
	}
	public void AddToBuffer(int action, Vector3 direction){
		StopAllCoroutines ();

		for (int i =actionBuffer.Length-1; i>0; --i) {
			actionBuffer[i]=actionBuffer[i-1];
			rotationBuffer[i]=rotationBuffer[i-1];
		}
		actionBuffer[0]=action;
		rotationBuffer[0] = Quaternion.LookRotation(direction);
		StartCoroutine(BufferTimer());
		return;
	}
}
