using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundThing : MonoBehaviour {

	const int size = 64;

	float[] fftSamples = new float[size * size];
	float[] lowPass = new float[size * size];

	void Start () {
		string micName = Microphone.devices [0];
		Debug.Log (micName);

		var clip = Microphone.Start (micName, true, 10, 44100);
		var audioSource = GetComponent<AudioSource> ();
		audioSource.clip = clip;
		audioSource.time = 9f;
		audioSource.Play ();


		Mesh mesh = new Mesh ();
		var verts = new Vector3[size * size];
		var tris = new int[(size - 1) * (size - 1) * 6];
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				Vector3 vert = new Vector3 (x, y, 0);
				int vIndex = x + y * size;
				verts [vIndex] = vert;

				if (x < size - 1 && y < size - 1) {
					var offset = x * 6 + y * 6 * (size - 1);

					// wind triangle indices clockwise because that's the goddamned way of thigs.  (facing direction)
					tris [offset] = vIndex;
					tris [offset + 1] = vIndex + 1;
					tris [offset + 2] = vIndex + size;

					tris [offset + 3] = vIndex + 1;
					tris [offset + 4] = vIndex + size + 1;
					tris [offset + 5] = vIndex + size;
				}
			}
		}

		mesh.vertices = verts;
		mesh.triangles = tris;
		GetComponent<MeshFilter> ().mesh = mesh;
	}
	
	void Update () {
		var audioSrc = GetComponent<AudioSource> ();
		var micPos = Microphone.GetPosition (null);

		// Set the play head 64 samples behind the microphone
		audioSrc.timeSamples = micPos;

		audioSrc.GetSpectrumData (fftSamples, 0, FFTWindow.Rectangular);
		for (int i = 0; i < fftSamples.Length; i++) {
			lowPass [i] = lowPass [i] * 0.5f + fftSamples [i] * 0.5f;
		}

		var mesh = GetComponent<MeshFilter> ().sharedMesh;
		var verts = mesh.vertices;
		for (int i = 0; i < size * size; i++) {
			verts [i].z = 5 * Mathf.Log(lowPass [i] * 100000000f);
		}
		mesh.vertices = verts;

		Debug.Log (lowPass [0]);
	}
}
