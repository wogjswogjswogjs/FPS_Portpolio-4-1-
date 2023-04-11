using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundTool : EditorWindow
{
    public int uiWidthLarge = 300;
    public int uiWidthMiddle = 200;
    private int selection = 0;
    private Vector2 scrollPosition1 = Vector2.zero;
    private Vector2 scrollPosition2 = Vector2.zero;

    private static SoundData soundData;
    private AudioClip source;

    [MenuItem("Tools/Sound Tool")]
    static void Init()
    {
        soundData = ScriptableObject.CreateInstance<SoundData>();
        soundData.LoadData();
        
        SoundTool window = GetWindow<SoundTool>(false, "SoundTool");
        ((EditorWindow)window).Show();
    }

    private void OnGUI()
    {
        // --------------------------------------------------------------------------
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add", GUILayout.Width(uiWidthLarge)))
                {
                    soundData.AddData();
                }

                if (soundData.GetDataCount() > 1)
                {
                    if (GUILayout.Button("Remove", GUILayout.Width(uiWidthLarge)))
                    {
                        soundData.RemoveData(selection);
                        if (selection == soundData.dataNameList.Count)
                        {
                            selection -= 1;
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            // --------------------------------------------------------------------------
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(uiWidthLarge));
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginVertical("box");
                    {
                        scrollPosition1 = EditorGUILayout.BeginScrollView(scrollPosition1);
                        {
                            selection = GUILayout.SelectionGrid(selection, soundData.GetNameList().ToArray(), 1);
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                {
                    scrollPosition2 = EditorGUILayout.BeginScrollView(scrollPosition2);
                    {
                        if (soundData.GetDataCount() > 0)
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                SoundClip sound = soundData.GetSound(selection);
                                
                                EditorGUILayout.Separator();
                                sound.soundPrefab = (AudioClip)EditorGUILayout.ObjectField("Sound Source",
                                    sound.soundPrefab, typeof(AudioClip), false,GUILayout.Width(uiWidthLarge));
                                EditorGUILayout.Separator();
                                
                                if (sound.soundPrefab != null)
                                {

                                    sound.soundID = EditorGUILayout.IntField("Sound ID",
                                        sound.soundID,GUILayout.Width(uiWidthLarge));
                                    sound.SOUNDTYPE = (SOUNDPLAYTYPE)EditorGUILayout.EnumPopup("Sound Type",
                                        sound.SOUNDTYPE, GUILayout.Width(uiWidthLarge));

                                    
                                    sound.soundPath = EditorGUILayout.TextField("Effect Path",
                                        EditorHelper.GetPath(sound.soundPrefab), GUILayout.Width(uiWidthLarge));
                                    soundData.dataNameList[selection] = EditorGUILayout.TextField("Effect Name",
                                        sound.soundPrefab.name, GUILayout.Width(uiWidthLarge));
                                    sound.soundName = soundData.dataNameList[selection];
                                    EditorGUILayout.Separator();

                                    EditorGUILayout.Separator();
                                    sound.hasLoop = EditorGUILayout.Toggle("hasLoop",
                                        sound.hasLoop, GUILayout.Width(uiWidthLarge));
                                    sound.maxVolume = EditorGUILayout.FloatField("Max Volume",
                                        sound.maxVolume, GUILayout.Width(uiWidthLarge));
                                    sound.pitch = EditorGUILayout.Slider("Pitch",
                                        sound.pitch, -3.0f, 3.0f, GUILayout.Width(uiWidthLarge));

                                    sound.dopplerLevel = EditorGUILayout.FloatField("Doppler Level",
                                        sound.dopplerLevel, GUILayout.Width(uiWidthLarge));
                                    sound.rolloffMode = (AudioRolloffMode)EditorGUILayout.EnumPopup("Volume Rolloff",
                                        sound.rolloffMode, GUILayout.Width(uiWidthLarge));
                                    sound.minDistance = EditorGUILayout.FloatField("min Distance",
                                        sound.minDistance, GUILayout.Width(uiWidthLarge));
                                    sound.maxDistance = EditorGUILayout.FloatField("max Distance",
                                        sound.maxDistance, GUILayout.Width(uiWidthLarge));
                                    sound.spartialBlend = EditorGUILayout.Slider("PanLevel",
                                        sound.spartialBlend, 0.0f, 1.0f, GUILayout.Width(uiWidthLarge)); 
                                   
                                    EditorGUILayout.Separator();
                                    /*if (sound.hasLoop == true)
                                    {
                                        if (GUILayout.Button("Add Loop", GUILayout.Width(uiWidthMiddle)))
                                        {

                                            soundData.soundClips[selection].AddLoop();
                                        }
                                        for (int i = 0; i < soundData.soundClips[selection].checkTime.Count; i++)
                                        {
                                            EditorGUILayout.BeginVertical("box");
                                            {
                                                GUILayout.Label("Loop Step" + i, EditorStyles.boldLabel);
                                                if (GUILayout.Button("Remove", GUILayout.Width(uiWidthMiddle)))
                                                {
                                                    soundData.soundClips[selection].RemoveLoop(i);
                                                }

                                                if (sound.checkTime.Count > 0)
                                                {
                                                    sound.checkTime[i] = EditorGUILayout.FloatField("check Time",
                                                        sound.checkTime[i], GUILayout.Width(uiWidthMiddle));
                                                    sound.setTime[i] = EditorGUILayout.FloatField("Set Time",
                                                        sound.setTime[i], GUILayout.Width(uiWidthMiddle));
                                                }
                                            }
                                            EditorGUILayout.EndVertical();

                                            soundData.soundClips[selection].RemoveLoop(i);
                                           

                                        }
                                       
                                        SoundClip sound1 = soundData.soundClips[selection];
                                    }*/
                                    EditorGUILayout.Separator();
                                }
                                EditorGUILayout.Separator();
                            }
                            EditorGUILayout.EndVertical();
                        }

                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
                
            }
            EditorGUILayout.EndHorizontal();
            //---------------------------------------------------------------------------
            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("SaveData", GUILayout.Width(uiWidthMiddle)))
                {
                    soundData.SaveData();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
}
