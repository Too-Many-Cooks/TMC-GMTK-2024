using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioLooper))]
public class AudioLooperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AudioLooper myAudioLooper = target as AudioLooper;


        #region Graph

        GUILayout.Space(10);

        // First, we need to make sure that our Audio Source has a clip in it. If it doesn't, we don't show a graph.
        if (myAudioLooper.AudioSource1.clip == null)
        {
            // Message to Select an Audio clip.
            GUIStyle headStyle = new GUIStyle();
            headStyle.fontSize = 15;
            headStyle.fontStyle = FontStyle.Bold;
            headStyle.normal.textColor = Color.red;
            headStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label("Place a Clip to Loop in the Audio Source", headStyle);

            return;
        }

        // Then, we check that our Fade In & Out durations are not too big. If they are, we don't show a graph.
        if (myAudioLooper.AudioSource1.clip.length < myAudioLooper.fadeInDuration + myAudioLooper.fadeOutDuration)
        {
            // Message to Select an Audio clip.
            GUIStyle headStyle = new GUIStyle();
            headStyle.fontSize = 15;
            headStyle.fontStyle = FontStyle.Bold;
            headStyle.normal.textColor = Color.red;
            headStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label("Reduce the Fade In & Fade Out durations", headStyle);

            return;
        }


        TLP.Editor.EditorGraph graph;

        const float cutGraphExtraLenghtMultiplier = 0.1f;


        float maxXValue1 = myAudioLooper.fadeInDuration + myAudioLooper.fadeOutDuration 
            + myAudioLooper.AudioSource1.clip.length * cutGraphExtraLenghtMultiplier;

        float maxXValue2 = myAudioLooper.AudioSource1.clip.length;


        float finalMaxXValue;
        bool cutGraph;  // Whether our Graph has a discontinuity.


        if (maxXValue1 < maxXValue2)
        {
            cutGraph = true;
            finalMaxXValue = maxXValue1;
        }
        else
        {
            cutGraph = false;
            finalMaxXValue = maxXValue2;
        }


        graph = new TLP.Editor.EditorGraph(0, -0.1f, finalMaxXValue, 1.1f, "", 100);


        // Horizontal lines.
        graph.AddLineY(0, Color.white);
        graph.AddLineY(0.5f, new Color(0.2f, 0.2f, 0.2f));
        graph.AddLineY(1, new Color(0.4f, 0.4f, 0.4f));


        // Vertical lines.
        for (int i = 0; i < finalMaxXValue; i++)
        {
            if (i == 0)
                graph.AddLineX(i, Color.grey);
            else
                graph.AddLineX(i, new Color(0.25f, 0.25f, 0.25f));
        }


        // Cut Graph
        float dividerXPosition = myAudioLooper.fadeInDuration + myAudioLooper.AudioSource1.clip.length * cutGraphExtraLenghtMultiplier / 2;

        if (cutGraph)
            graph.AddLineX(dividerXPosition, Color.red);



        // Graphs.

        Color mainGraphColor = new Color(0.8f, 0.8f, 0.8f, 1), secondaryGraphColor = new Color(0.5f, 0.5f, 0.5f, 1);

        float EvaluateMainGraphVisuals(float x)
        {
            // Uncut Graph.
            if (!cutGraph)
                return myAudioLooper.EvaluateLoopAtTime(x);


            // Cut Graph.

            // First part of the cut graph.
            if (x < dividerXPosition)
                return myAudioLooper.EvaluateLoopAtTime(x);

            // Second part of the graph.
            float timeToEvaluate = myAudioLooper.AudioSource1.clip.length - (finalMaxXValue - x);
            return myAudioLooper.EvaluateLoopAtTime(timeToEvaluate);
        }

        float EvaluateSecondaryGraphVisuals(float x)
        {
            // Checking if x is whithin the range of the secondary fade out visuals.
            if (x < myAudioLooper.blendTime)
                return myAudioLooper.EvaluateLoopAtTime(myAudioLooper.AudioSource1.clip.length - myAudioLooper.blendTime + x);


            // Checking if x is whithin the range of the secondary fade in visuals.
            if (finalMaxXValue - x < myAudioLooper.blendTime)
                return myAudioLooper.EvaluateLoopAtTime(x - (finalMaxXValue - myAudioLooper.blendTime));

            return 0;
        }


        graph.AddFunction(x => EvaluateMainGraphVisuals(x), mainGraphColor);
        graph.AddFunction(x => EvaluateSecondaryGraphVisuals(x), secondaryGraphColor);


        graph.Draw();

        #endregion
    }
}
