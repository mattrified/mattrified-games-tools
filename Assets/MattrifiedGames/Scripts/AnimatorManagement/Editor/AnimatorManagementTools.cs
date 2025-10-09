using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;
using System.Collections.Generic;

namespace MattrifiedGames.ManagedAnimation.Edit
{
    public class AnimatorManagementToolWindow : EditorWindow
    {
        const string LAST_CONTROLLER_PREF_KEY = "AMTW - Last Controller GUID";
        AnimatorController controller;

        Vector2 scrolling;

        Hashtable helperHash;

        List<AnimatorStateTransition> copiedTransitions;

        [MenuItem("Animator Management/Open Editor")]
        public static void OpenWindow()
        {
            AnimatorManagementToolWindow amtw = GetWindow<AnimatorManagementToolWindow>();

            string lastGUID = EditorPrefs.GetString(LAST_CONTROLLER_PREF_KEY, string.Empty);
            string path = AssetDatabase.GUIDToAssetPath(lastGUID);
            amtw.controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            amtw.helperHash = new Hashtable();
            amtw.copiedTransitions = new List<AnimatorStateTransition>();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            AnimatorController newController = (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", controller,
                typeof(AnimatorController), false);
            if (newController != controller)
            {
                if (newController != null)
                    EditorPrefs.SetString(LAST_CONTROLLER_PREF_KEY, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newController)));
                controller = newController;
            }

            if (GUILayout.Button("Focus on Controller"))
                Selection.activeObject = controller;
            EditorGUILayout.EndHorizontal();

            scrolling = EditorGUILayout.BeginScrollView(scrolling);

            if (FoldoutSection("Copied Transitions", "SHOW COPIED TRANSITIONS"))
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < copiedTransitions.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(copiedTransitions[i].name, GUILayout.Width(200f));

                    if (GUILayout.Button("Remove", GUILayout.Width(200f)))
                    {
                        copiedTransitions.RemoveAt(i);
                        i--;
                    }

                    GUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel = 0;

            var objects = Selection.objects;
            for (int i = 0, len = objects.Length; i < len; i++)
            {
                if (EditController(objects[i] as AnimatorController))
                    continue;

                if (EditStateMachine(objects[i] as AnimatorStateMachine))
                    continue;

                if (EditState(objects[i] as AnimatorState))
                    continue;

                if (EditStateTransition(objects[i] as AnimatorStateTransition))
                    continue;
            }

            EditorGUILayout.EndScrollView();
        }

        private bool EditStateTransition(AnimatorStateTransition animatorStateTransition)
        {
            if ((object)animatorStateTransition == null)
                return false;

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Transition:  " + animatorStateTransition.name, GUILayout.Width(300f));
            if (!copiedTransitions.Contains(animatorStateTransition) && GUILayout.Button("Copy", GUILayout.Width(200f)))
            {
                copiedTransitions.Add(animatorStateTransition);
            }

            if (copiedTransitions.Count > 0 && GUILayout.Button("Paste Transition Info"))
            {
                animatorStateTransition.conditions = (AnimatorCondition[])copiedTransitions[0].conditions.Clone();
                animatorStateTransition.duration = copiedTransitions[0].duration;
                animatorStateTransition.exitTime = copiedTransitions[0].exitTime;
                animatorStateTransition.hasExitTime = copiedTransitions[0].hasExitTime;
                animatorStateTransition.mute = true;
                animatorStateTransition.offset = copiedTransitions[0].offset;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;

            if (animatorStateTransition.destinationState != null)
                EditorGUILayout.LabelField(animatorStateTransition.destinationState.ToString());

            if (animatorStateTransition.destinationStateMachine != null)
                EditorGUILayout.LabelField(animatorStateTransition.destinationStateMachine.ToString());

            string conditionKey = animatorStateTransition.name + " SHOW CONDITIIONS";
            if (FoldoutSection("Conditions", conditionKey))
            {
                EditorGUI.indentLevel++;
                animatorStateTransition.hasExitTime = EditorGUILayout.Toggle("Has Exit Time", animatorStateTransition.hasExitTime);
                if (animatorStateTransition.hasExitTime)
                {
                    EditorGUI.indentLevel++;
                    animatorStateTransition.exitTime = EditorGUILayout.FloatField("Exit Time", animatorStateTransition.exitTime);
                    EditorGUI.indentLevel--;
                }

                for (int i = 0; i < animatorStateTransition.conditions.Length; i++)
                {
                    Color cc = GUI.contentColor;
                    EditorGUILayout.LabelField("Condition " + i, CreateConditionString(animatorStateTransition.conditions[i]));
                    GUI.contentColor = cc;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;

            return true;
        }

        private string CreateConditionString(AnimatorCondition animatorCondition)
        {
            switch (animatorCondition.mode)
            {
                case AnimatorConditionMode.If: return animatorCondition.parameter + " is true.";
                case AnimatorConditionMode.IfNot: return animatorCondition.parameter + " is false.";
                case AnimatorConditionMode.Greater: return animatorCondition.parameter + " > " + animatorCondition.threshold;
                case AnimatorConditionMode.Less: return animatorCondition.parameter + " < " + animatorCondition.threshold;
                case AnimatorConditionMode.Equals: return animatorCondition.parameter + " == " + animatorCondition.threshold;
                case AnimatorConditionMode.NotEqual: return animatorCondition.parameter + " != " + animatorCondition.threshold;
                default: return animatorCondition.parameter + ", " + animatorCondition.mode + ", " + animatorCondition.threshold;
            }
        }

        private bool IndentedButton(string label, int indentValue = 24)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * indentValue);
            bool result = GUILayout.Button(label);
            EditorGUILayout.EndHorizontal();
            return result;
        }

        private bool EditState(AnimatorState animatorState)
        {
            if ((object)animatorState == null)
                return false;

            EditorGUILayout.LabelField("State:  " + animatorState.name);

            EditorGUI.indentLevel++;
            string transitionKey = animatorState.name + " Show Transitions";
            if (FoldoutSection("Transitions", transitionKey))
            {
                if (IndentedButton("Paste Transitions"))
                {
                    for (int i = 0; i < copiedTransitions.Count; i++)
                    {
                        AnimatorStateTransition newTransition = Instantiate(copiedTransitions[i]);
                        AssetDatabase.AddObjectToAsset(newTransition, controller);
                        animatorState.AddTransition(newTransition);
                    }

                    NameAllStateTransitions(animatorState.transitions, animatorState);
                }

                if (IndentedButton("Name All Transitions"))
                {
                    NameAllStateTransitions(animatorState.transitions, animatorState);
                }

                for (int i = 0; i < animatorState.transitions.Length; i++)
                {
                    EditStateTransition(animatorState.transitions[i]);
                }
            }
            EditorGUI.indentLevel--;

            EditorGUI.indentLevel++;
            string showStateBehaviours = animatorState.name + " Show State Behavours";
            if (FoldoutSection("State Behaviours", showStateBehaviours))
            {
                for (int i = 0; i < animatorState.behaviours.Length; i++)
                {
                    EditorGUI.indentLevel++;

                    string sb = animatorState.name + " Show State Behavours " + i;
                    if (FoldoutSection(animatorState.behaviours[i].name, sb))
                    {
                        EditorGUI.indentLevel++;
                        if (helperHash.ContainsKey(animatorState.behaviours[i]))
                        {
                            ((Editor)helperHash[animatorState.behaviours[i]]).OnInspectorGUI();
                        }
                        else
                        {
                            helperHash.Add(animatorState.behaviours[i], Editor.CreateEditor(animatorState.behaviours[i]));
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUI.indentLevel--;

            return true;
        }

        private bool EditStateMachine(AnimatorStateMachine animatorStateMachine)
        {
            if ((object)animatorStateMachine == null)
                return false;

            EditorGUILayout.LabelField("State Machine:  " + animatorStateMachine.name);

            if (animatorStateMachine.stateMachines.Length > 0)
            {
                string showStateMachinesKey = animatorStateMachine.name + " Show State Machines";
                if (FoldoutSection("State Machines", showStateMachinesKey))
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < animatorStateMachine.stateMachines.Length; i++)
                    {
                        EditStateMachine(animatorStateMachine.stateMachines[i].stateMachine);
                    }
                    EditorGUI.indentLevel--;
                }
            }

            if (animatorStateMachine.states.Length > 0)
            {
                string showStatesKey = animatorStateMachine.name + " Show States";
                EditorGUI.indentLevel++;
                if (FoldoutSection("Show States", showStatesKey))
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < animatorStateMachine.states.Length; i++)
                    {
                        EditState(animatorStateMachine.states[i].state);
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }

            return true;
        }

        private bool EditController(AnimatorController animatorController)
        {
            if ((object)animatorController == null)
                return false;

            EditorGUILayout.LabelField("Animator Controller:  " + animatorController.name);

            if (GUILayout.Button("Paste Transition to Any States"))
            {
                for (int i = 0; i < copiedTransitions.Count; i++)
                {
                    AnimatorStateTransition newTransition =
                        animatorController.layers[0].stateMachine.AddAnyStateTransition(copiedTransitions[i].destinationState);

                    newTransition.hasExitTime = copiedTransitions[i].hasExitTime;
                    newTransition.exitTime = copiedTransitions[i].exitTime;
                    newTransition.mute = copiedTransitions[i].mute;
                    newTransition.offset = copiedTransitions[i].offset;
                    newTransition.duration = copiedTransitions[i].duration;
                    newTransition.conditions = (AnimatorCondition[])copiedTransitions[i].conditions.Clone();
                }

                NameAllStateTransitions(animatorController);
            }

            if (GUILayout.Button("Name All Transitions"))
                NameAllStateTransitions(animatorController);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Mute All Transitions"))
            {
                MuteAllStateTransitions(animatorController, true);
            }
            if (GUILayout.Button("Unmute All Transitions"))
            {
                MuteAllStateTransitions(animatorController, false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Turn On Foot IK"))
            {
                SetFootIKAllStates(animatorController, true);
            }
            if (GUILayout.Button("Turn Off Foot IK"))
            {
                SetFootIKAllStates(animatorController, false);
            }
            EditorGUILayout.EndHorizontal();

            string showTKey = animatorController.name + " Show Any Transitions";
            if (FoldoutSection("Any State Transitions", showTKey))
            {
                for (int i = 0; i < animatorController.layers.Length; i++)
                {
                    for (int j = 0; j < animatorController.layers[i].stateMachine.anyStateTransitions.Length; j++)
                    {
                        EditStateTransition(animatorController.layers[i].stateMachine.anyStateTransitions[j]);
                    }
                }
            }

            string showPKey = animatorController.name + " Show Parameters";
            if (FoldoutSection("Parameters", showPKey))
            {
                EditorGUI.indentLevel++;
                // List all parameters
                if (IndentedButton("Convert All Triggers to Bools"))
                {
                    for (int i = 0; i < animatorController.parameters.Length; i++)
                    {
                        var parameter = animatorController.parameters[i];
                        if (parameter.type == AnimatorControllerParameterType.Trigger)
                        {
                            parameter.type = AnimatorControllerParameterType.Bool;
                            animatorController.RemoveParameter(i);
                            animatorController.AddParameter(parameter);
                            i--;
                        }
                    }
                }

                for (int i = 0; i < animatorController.parameters.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    var parameter = animatorController.parameters[i];
                    EditorGUILayout.LabelField(parameter.name, GUILayout.Width(200f));

                    AnimatorControllerParameterType currentType = parameter.type;
                    parameter.type = (AnimatorControllerParameterType)EditorGUILayout.EnumPopup(parameter.type, GUILayout.Width(150f));
                    if (currentType != parameter.type)
                    {
                        animatorController.RemoveParameter(i);
                        animatorController.AddParameter(parameter);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }

            string showStateMachineKey = animatorController.name + " Show State Machines";
            if (FoldoutSection("State Machines", showStateMachineKey))
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < animatorController.layers.Length; i++)
                {
                    EditStateMachine(animatorController.layers[i].stateMachine);
                }
                EditorGUI.indentLevel--;
            }

            return true;
        }

        private bool FoldoutSection(string label, string key)
        {
            SetHashValue(key, EditorGUILayout.Foldout(GetHashValue(key, true), label));
            return GetHashValue(key, true);
        }

        private T GetHashValue<T>(string key, T defaultValue)
        {
            if (helperHash == null)
                helperHash = new Hashtable();

            if (!helperHash.ContainsKey(key))
            {
                helperHash.Add(key, defaultValue);
            }
            return (T)helperHash[key];
        }

        private void SetHashValue<T>(string key, T value)
        {
            if (helperHash == null)
                helperHash = new Hashtable();

            if (!helperHash.ContainsKey(key))
            {
                helperHash.Add(key, value);
            }
            else
            {
                helperHash[key] = value;
            }
        }

        private void NameAllStateTransitions(AnimatorController animatorController)
        {
            for (int i = 0; i < animatorController.layers.Length; i++)
            {
                var layer = animatorController.layers[i];
                var anyStateTransitions = layer.stateMachine.anyStateTransitions;

                NameAllStateTransitions(anyStateTransitions, null);

                NameAllStateTransitions(layer.stateMachine);
                NameAllStateTransitions(layer.stateMachine.states);
            }
        }

        private void NameAllStateTransitions(AnimatorStateMachine stateMachine)
        {
            NameAllStateTransitions(stateMachine.states);

            for (int i = 0; i < stateMachine.stateMachines.Length; i++)
                NameAllStateTransitions(stateMachine.stateMachines[i].stateMachine);
        }

        private void NameAllStateTransitions(ChildAnimatorState[] states)
        {
            for (int i = 0; i < states.Length; i++)
                NameAllStateTransitions(states[i].state.transitions, states[i].state);
        }

        private void NameAllStateTransitions(AnimatorStateTransition[] anyStateTransitions, AnimatorState parentState)
        {
            for (int i = 0; i < anyStateTransitions.Length; i++)
            {
                anyStateTransitions[i].name = i.ToString() + ":  " + (parentState == null ? "Any State" : parentState.name) + " -> " +
                    anyStateTransitions[i].destinationState.name;
            }
        }


        private void MuteAllStateTransitions(AnimatorController animatorController, bool mute)
        {
            for (int i = 0; i < animatorController.layers.Length; i++)
            {
                var layer = animatorController.layers[i];
                var anyStateTransitions = layer.stateMachine.anyStateTransitions;

                MuteAllStateTransitions(anyStateTransitions, mute);

                MuteAllStateTransitions(layer.stateMachine, mute);
                MuteAllStateTransitions(layer.stateMachine.states, mute);
            }
        }

        private void MuteAllStateTransitions(AnimatorStateMachine stateMachine, bool mute)
        {
            MuteAllStateTransitions(stateMachine.states, mute);

            for (int i = 0; i < stateMachine.stateMachines.Length; i++)
                MuteAllStateTransitions(stateMachine.stateMachines[i].stateMachine, mute);
        }

        private void MuteAllStateTransitions(ChildAnimatorState[] states, bool mute)
        {
            for (int i = 0; i < states.Length; i++)
                MuteAllStateTransitions(states[i].state.transitions, mute);
        }

        private void MuteAllStateTransitions(AnimatorStateTransition[] anyStateTransitions, bool mute)
        {
            for (int i = 0; i < anyStateTransitions.Length; i++)
            {
                anyStateTransitions[i].mute = mute;
            }
        }

        private void SetFootIKAllStates(AnimatorController animatorController, bool footIK)
        {
            for (int i = 0; i < animatorController.layers.Length; i++)
            {
                var layer = animatorController.layers[i];

                SetFootIKAllStates(layer.stateMachine, footIK);
                SetFootIKAllStates(layer.stateMachine.states, footIK);
            }
        }

        private void SetFootIKAllStates(AnimatorStateMachine stateMachine, bool footIK)
        {
            SetFootIKAllStates(stateMachine.states, footIK);

            for (int i = 0; i < stateMachine.stateMachines.Length; i++)
                SetFootIKAllStates(stateMachine.stateMachines[i].stateMachine, footIK);
        }

        private void SetFootIKAllStates(ChildAnimatorState[] states, bool footIK)
        {
            for (int i = 0; i < states.Length; i++)
                states[i].state.iKOnFeet = footIK;
        }
    }
}