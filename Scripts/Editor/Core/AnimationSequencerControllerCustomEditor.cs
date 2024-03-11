#if DOTWEEN_ENABLED
using System;
using DG.DOTweenEditor;
using DG.Tweening;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [CustomEditor(typeof(AnimationSequencerController), true)]
    public class AnimationSequencerControllerCustomEditor : Editor
    {
        private static readonly GUIContent CollapseAllAnimationStepsContent = new GUIContent("▸◂", "Collapse all animation steps");
        private static readonly GUIContent ExpandAllAnimationStepsContent   = new GUIContent("◂▸", "Expand all animation steps");

        private ReorderableList reorderableList;
        
        private AnimationSequencerController sequencerController;

        private static AnimationStepAdvancedDropdown cachedAnimationStepsDropdown;
        private static AnimationStepAdvancedDropdown AnimationStepAdvancedDropdown
        {
            get
            {
                if (cachedAnimationStepsDropdown == null)
                    cachedAnimationStepsDropdown = new AnimationStepAdvancedDropdown(new AdvancedDropdownState());
                return cachedAnimationStepsDropdown;
            }
        }

        private bool showPreviewPanel = true;
        private bool showSettingsPanel;
        private bool showCallbacksPanel;
        private bool showSequenceSettingsPanel;
        private bool showStepsPanel = true;
        private float tweenTimeScale = 1f;
        private bool wasShowingStepsPanel;
        private bool justStartPreviewing;

        private (float start, float end)[] previewingTimings;

        private void OnEnable()
        {
            sequencerController = target as AnimationSequencerController;
            reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("animationSteps"), true, false, true, true);
            reorderableList.drawElementCallback += OnDrawAnimationStep;
            reorderableList.drawElementBackgroundCallback += OnDrawAnimationStepBackground;
            reorderableList.elementHeightCallback += GetAnimationStepHeight;
            reorderableList.onAddDropdownCallback += OnClickToAddNew;
            reorderableList.onRemoveCallback += OnClickToRemove;
            reorderableList.onReorderCallback += OnListOrderChanged;
            reorderableList.drawHeaderCallback += OnDrawerHeader;
            EditorApplication.update += EditorUpdate;
            EditorApplication.playModeStateChanged += OnEditorPlayModeChanged;
            
#if UNITY_2021_1_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage.prefabSaving += PrefabSaving;
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabSaving += PrefabSaving;
#endif
            
            Repaint();
        }

       

        public override bool RequiresConstantRepaint()
        {
            return DOTweenEditorPreview.isPreviewing;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        private void OnDisable()
        {
            reorderableList.drawElementCallback -= OnDrawAnimationStep;
            reorderableList.drawElementBackgroundCallback -= OnDrawAnimationStepBackground;
            reorderableList.elementHeightCallback -= GetAnimationStepHeight;
            reorderableList.onAddDropdownCallback -= OnClickToAddNew;
            reorderableList.onRemoveCallback -= OnClickToRemove;
            reorderableList.onReorderCallback -= OnListOrderChanged;
            reorderableList.drawHeaderCallback -= OnDrawerHeader;
            EditorApplication.playModeStateChanged -= OnEditorPlayModeChanged;
            EditorApplication.update -= EditorUpdate;

#if UNITY_2021_1_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage.prefabSaving -= PrefabSaving;
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabSaving -= PrefabSaving;
#endif

            if (!Application.isPlaying)
            {
                if (DOTweenEditorPreview.isPreviewing)
                {
                    sequencerController.ResetToInitialState();
                    DOTweenEditorPreview.Stop();            
                }
            }
            
            tweenTimeScale = 1f;
        }

        private void EditorUpdate()
        {
            if (Application.isPlaying)
                return;

            SerializedProperty progressSP = serializedObject.FindProperty("progress");
            if (progressSP == null || Mathf.Approximately(progressSP.floatValue, -1))
                return;
            
            SetProgress(progressSP.floatValue);
        }

        private void OnEditorPlayModeChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.ExitingEditMode)
            {
                if (DOTweenEditorPreview.isPreviewing)
                {
                    sequencerController.ResetToInitialState();
                    DOTweenEditorPreview.Stop();            
                }
            }
        }
        
        private void PrefabSaving(GameObject gameObject)
        {
            if (DOTweenEditorPreview.isPreviewing)
            {
                sequencerController.ResetToInitialState();
                DOTweenEditorPreview.Stop();            
            }
        }
        
        private void OnDrawerHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Animation Steps");
        }
        
        private void AddNewAnimationStepOfType(Type targetAnimationType)
        {
            SerializedProperty animationStepsProperty = reorderableList.serializedProperty;
            int targetIndex = animationStepsProperty.arraySize;
            animationStepsProperty.InsertArrayElementAtIndex(targetIndex);
            SerializedProperty arrayElementAtIndex = animationStepsProperty.GetArrayElementAtIndex(targetIndex);
            object managedReferenceValue = Activator.CreateInstance(targetAnimationType);
            arrayElementAtIndex.managedReferenceValue = managedReferenceValue;
        
            //TODO copy from last step would be better here.
            SerializedProperty targetSerializedProperty = arrayElementAtIndex.FindPropertyRelative("target");
            if (targetSerializedProperty != null)
                targetSerializedProperty.objectReferenceValue = (serializedObject.targetObject as AnimationSequencerController)?.gameObject;
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnClickToRemove(ReorderableList list)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(list.index);
            SerializedPropertyExtensions.ClearPropertyCache(element.propertyPath);
            reorderableList.serializedProperty.DeleteArrayElementAtIndex(list.index);
            reorderableList.serializedProperty.serializedObject.ApplyModifiedProperties();
        }
        
        private void OnListOrderChanged(ReorderableList list)
        {
            SerializedPropertyExtensions.ClearPropertyCache(list.serializedProperty.propertyPath);
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
        }
        
        private void OnClickToAddNew(Rect buttonRect, ReorderableList list)
        {
            AnimationStepAdvancedDropdown.Show(buttonRect, OnNewAnimationStepTypeSelected);
        }

        private void OnNewAnimationStepTypeSelected(AnimationStepAdvancedDropdownItem animationStepAdvancedDropdownItem)
        {
            AddNewAnimationStepOfType(animationStepAdvancedDropdownItem.AnimationStepType);
        }

        public override void OnInspectorGUI()
        {
            if (sequencerController.IsResetRequired())
            {
                SetDefaults();
            }

            DrawFoldoutArea("Settings", ref showSettingsPanel, DrawSettings, DrawSettingsHeader);
            DrawFoldoutArea("Callback", ref showCallbacksPanel, DrawCallbacks);
            DrawFoldoutArea("Preview", ref showPreviewPanel, DrawPreviewControls);
            DrawFoldoutArea("Steps", ref showStepsPanel, DrawAnimationSteps, DrawAnimationStepsHeader, 50);
        }

        private void DrawAnimationStepsHeader(Rect rect, bool foldout)
        {
            if (!foldout)
                return;
            
            var collapseAllRect = new Rect(rect)
            {
                xMin = rect.xMax - 50,
                xMax = rect.xMax - 25,
            };

            var expandAllRect = new Rect(rect)
            {
                xMin = rect.xMax - 25,
                xMax = rect.xMax - 0,
            };

            if (GUI.Button(collapseAllRect, CollapseAllAnimationStepsContent, EditorStyles.miniButtonLeft))
            {
                SetStepsExpanded(false);
            }

            if (GUI.Button(expandAllRect, ExpandAllAnimationStepsContent, EditorStyles.miniButtonRight))
            {
                SetStepsExpanded(true);
            }
        }

        private void DrawAnimationSteps()
        {
            bool wasGUIEnabled = GUI.enabled;
            if (DOTweenEditorPreview.isPreviewing)
                GUI.enabled = false;

            reorderableList.DoLayoutList();
                        
            GUI.enabled = wasGUIEnabled;
        }

        protected virtual void DrawCallbacks()
        {
            bool wasGUIEnabled = GUI.enabled;
            if (DOTweenEditorPreview.isPreviewing)
                GUI.enabled = false;
            SerializedProperty onStartEventSerializedProperty = serializedObject.FindProperty("onStartEvent");
            SerializedProperty onFinishedEventSerializedProperty = serializedObject.FindProperty("onFinishedEvent");
            SerializedProperty onProgressEventSerializedProperty = serializedObject.FindProperty("onProgressEvent");

            
            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(onStartEventSerializedProperty);
                EditorGUILayout.PropertyField(onFinishedEventSerializedProperty);
                EditorGUILayout.PropertyField(onProgressEventSerializedProperty);
                
                if (changedCheck.changed)
                    serializedObject.ApplyModifiedProperties();
            }
            
            GUI.enabled = wasGUIEnabled;
        }

        private void DrawSettingsHeader(Rect rect, bool foldout)
        {
            var autoPlayModeSerializedProperty = serializedObject.FindProperty("autoplayMode");
            var autoKillSerializedProperty = serializedObject.FindProperty("autoKill");

            var autoplayMode = (AnimationSequencerController.AutoplayType) autoPlayModeSerializedProperty.enumValueIndex;
            var autoKill = autoKillSerializedProperty.boolValue;

            if (autoKill)
                rect = DrawAutoSizedBadgeRight(rect, "Auto Kill", new Color(1f, 0.2f, 0f, 0.6f));

            if (autoplayMode == AnimationSequencerController.AutoplayType.Awake)
                rect = DrawAutoSizedBadgeRight(rect, "AutoPlay on Awake", new Color(1f, 0.7f, 0f, 0.6f));
            else if (autoplayMode == AnimationSequencerController.AutoplayType.OnEnable)
                rect = DrawAutoSizedBadgeRight(rect, "AutoPlay on Enable", new Color(1f, 0.7f, 0f, 0.6f));
        }

        private void DrawSettings()
        {
            SerializedProperty autoPlayModeSerializedProperty = serializedObject.FindProperty("autoplayMode");
            SerializedProperty pauseOnAwakeSerializedProperty = serializedObject.FindProperty("startPaused");

            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                AnimationSequencerController.AutoplayType autoplayMode = (AnimationSequencerController.AutoplayType)autoPlayModeSerializedProperty.enumValueIndex;
                EditorGUILayout.PropertyField(autoPlayModeSerializedProperty);

                if (autoplayMode != AnimationSequencerController.AutoplayType.Nothing)
                    EditorGUILayout.PropertyField(pauseOnAwakeSerializedProperty);
				
                DrawPlaybackSpeedSlider();
                
                if (changedCheck.changed)
                    serializedObject.ApplyModifiedProperties();
            }
            
            bool wasEnabled = GUI.enabled; 
            if (DOTweenEditorPreview.isPreviewing)
                GUI.enabled = false;
            
            SerializedProperty updateTypeSerializedProperty = serializedObject.FindProperty("updateType");
            SerializedProperty timeScaleIndependentSerializedProperty = serializedObject.FindProperty("timeScaleIndependent");
            SerializedProperty sequenceDirectionSerializedProperty = serializedObject.FindProperty("playType");
            SerializedProperty loopsSerializedProperty = serializedObject.FindProperty("loops");
            SerializedProperty loopTypeSerializedProperty = serializedObject.FindProperty("loopType");
            SerializedProperty autoKillSerializedProperty = serializedObject.FindProperty("autoKill");

            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(timeScaleIndependentSerializedProperty);
                EditorGUILayout.PropertyField(sequenceDirectionSerializedProperty);
                EditorGUILayout.PropertyField(updateTypeSerializedProperty);
                EditorGUILayout.PropertyField(autoKillSerializedProperty);

                EditorGUILayout.PropertyField(loopsSerializedProperty);

                if (loopsSerializedProperty.intValue != 0)
                {
                    EditorGUILayout.PropertyField(loopTypeSerializedProperty);
                }
 
                if (changedCheck.changed)
                {
                    loopsSerializedProperty.intValue = Mathf.Clamp(loopsSerializedProperty.intValue, -1, int.MaxValue);
                    serializedObject.ApplyModifiedProperties();
                }
                
            }
            GUI.enabled = wasEnabled;
        }
		
        private void DrawPlaybackSpeedSlider()
        {
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            
            var playbackSpeedProperty = serializedObject.FindProperty("playbackSpeed");
            playbackSpeedProperty.floatValue = EditorGUILayout.Slider("Playback Speed", playbackSpeedProperty.floatValue, 0, 2);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                UpdateSequenceTimeScale();
            }

            GUILayout.FlexibleSpace();
        }
        
        private void UpdateSequenceTimeScale()
        {
            if (sequencerController.PlayingSequence == null)
                return;
            
            sequencerController.PlayingSequence.timeScale = sequencerController.PlaybackSpeed * tweenTimeScale;
        }
        

        private void DrawPreviewControls()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            
            bool guiEnabled = GUI.enabled;

            GUIStyle previewButtonStyle = new GUIStyle(GUI.skin.button);
            previewButtonStyle.fixedWidth = previewButtonStyle.fixedHeight = 40;
            if (GUILayout.Button(AnimationSequenceEditorGUIUtility.BackButtonGUIContent, previewButtonStyle))
            {
                if (!sequencerController.IsPlaying)
                    PlaySequence();

                sequencerController.Rewind();
            }

            if (GUILayout.Button(AnimationSequenceEditorGUIUtility.StepBackGUIContent, previewButtonStyle))
            {
                if(!sequencerController.IsPlaying)
                    PlaySequence();

                StepBack();
            }

            if (sequencerController.IsPlaying)
            {
                if (GUILayout.Button(AnimationSequenceEditorGUIUtility.PauseButtonGUIContent, previewButtonStyle))
                {
                    sequencerController.Pause();
                }
            }
            else
            {
                if (GUILayout.Button(AnimationSequenceEditorGUIUtility.PlayButtonGUIContent, previewButtonStyle))
                {
                    PlaySequence();
                }
            }

            
            if (GUILayout.Button(AnimationSequenceEditorGUIUtility.StepNextGUIContent, previewButtonStyle))
            {
                if(!sequencerController.IsPlaying)
                    PlaySequence();

                StepNext();
            }
            
            if (GUILayout.Button(AnimationSequenceEditorGUIUtility.ForwardButtonGUIContent, previewButtonStyle))
            {
                if (!sequencerController.IsPlaying)
                    PlaySequence();

                sequencerController.Complete();
            }

            if (!Application.isPlaying)
            {
                GUI.enabled = DOTweenEditorPreview.isPreviewing;
                if (GUILayout.Button(AnimationSequenceEditorGUIUtility.StopButtonGUIContent, previewButtonStyle))
                {
                    sequencerController.Rewind();
                    DOTween.Kill(sequencerController.PlayingSequence);
                    DOTweenEditorPreview.Stop();
                    sequencerController.ResetToInitialState();
                    sequencerController.ClearPlayingSequence();
                    if (AnimationSequencerSettings.GetInstance().AutoHideStepsWhenPreviewing)
                        showStepsPanel = wasShowingStepsPanel;
                }
            }

            GUI.enabled = guiEnabled;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            DrawTimeScaleSlider();
            DrawProgressSlider();
        }

        private void StepBack()
        {
            if (!sequencerController.IsPlaying)
                PlaySequence();
            
            sequencerController.PlayingSequence.Goto((sequencerController.PlayingSequence.ElapsedPercentage() -
                                                      0.01f) * sequencerController.PlayingSequence.Duration());
        }

        private void StepNext()
        {
            if (!sequencerController.IsPlaying)
                PlaySequence();

            sequencerController.PlayingSequence.Goto((sequencerController.PlayingSequence.ElapsedPercentage() +
                                                      0.01f) * sequencerController.PlayingSequence.Duration());
        }

        private void PlaySequence()
        {
            justStartPreviewing = false;
            if (!Application.isPlaying)
            {
                if (!DOTweenEditorPreview.isPreviewing)
                {
                    justStartPreviewing = true;
                    DOTweenEditorPreview.Start();

                    sequencerController.Play();
                    
                    DOTweenEditorPreview.PrepareTweenForPreview(sequencerController.PlayingSequence);

                    if (AnimationSequencerSettings.GetInstance().DrawTimingsWhenPreviewing)
                        previewingTimings = DOTweenProxy.GetTimings(sequencerController.PlayingSequence,
                            sequencerController.AnimationSteps);
                    else
                        previewingTimings = null;
                }
                else
                {
                    if (sequencerController.PlayingSequence == null)
                    {
                        sequencerController.Play();
                    }
                    else
                    {
                        if (!sequencerController.PlayingSequence.IsBackwards() &&
                            sequencerController.PlayingSequence.fullPosition >= sequencerController.PlayingSequence.Duration())
                        {
                            sequencerController.Rewind();
                        }
                        else if (sequencerController.PlayingSequence.IsBackwards() &&
                                 sequencerController.PlayingSequence.fullPosition <= 0f)
                        {
                            sequencerController.Complete();
                        }

                        sequencerController.TogglePause();
                    }
                }
            }
            else
            {
                if (sequencerController.PlayingSequence == null)
                    sequencerController.Play();
                else
                {
                    if (sequencerController.PlayingSequence.IsActive())
                        sequencerController.TogglePause();
                    else
                        sequencerController.Play();
                }
            }

            if (justStartPreviewing)
                wasShowingStepsPanel = showStepsPanel;
            
            showStepsPanel = !AnimationSequencerSettings.GetInstance().AutoHideStepsWhenPreviewing;
        }

        private void DrawProgressSlider()
        {
            GUILayout.FlexibleSpace();

            EditorGUI.BeginChangeCheck();
            float tweenProgress = 0;

            tweenProgress = GetCurrentSequencerProgress();

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 65;
            tweenProgress = EditorGUILayout.Slider("Progress", tweenProgress, 0, 1);
            EditorGUIUtility.labelWidth = oldLabelWidth;

            if (EditorGUI.EndChangeCheck())
            {
                SetProgress(tweenProgress);

                if (!Application.isPlaying)
                {
                    serializedObject.FindProperty("progress").floatValue = tweenProgress;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.FlexibleSpace();
        }

        private void SetProgress(float tweenProgress)
        {
            if (!sequencerController.IsPlaying)
                PlaySequence();

            sequencerController.PlayingSequence.Goto(tweenProgress *
                                                     sequencerController.PlayingSequence.Duration());
        }

        private float GetCurrentSequencerProgress()
        {
            float tweenProgress;
            if (sequencerController.PlayingSequence != null && sequencerController.PlayingSequence.IsActive())
                tweenProgress = sequencerController.PlayingSequence.ElapsedPercentage();
            else
                tweenProgress = 0;
            return tweenProgress;
        }

        private void SetCurrentSequenceProgress(float progress)
        {
            sequencerController.PlayingSequence.Goto(progress *
                                                     sequencerController.PlayingSequence.Duration());
        }

        private void DrawTimeScaleSlider()
        {
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 65;
            tweenTimeScale = EditorGUILayout.Slider("TimeScale", tweenTimeScale, 0, 2);
            EditorGUIUtility.labelWidth = oldLabelWidth;
			
            UpdateSequenceTimeScale();

            GUILayout.FlexibleSpace();
        }

        private void DrawFoldoutArea(string title, ref bool foldout, Action additionalInspectorGUI,
            Action<Rect, bool> additionalHeaderGUI = null, float additionalHeaderWidth = 0)
        {
            Rect rect = EditorGUILayout.GetControlRect();

            if (Event.current.type == EventType.Repaint)
            {
                GUI.skin.box.Draw(rect, false, false, false, false);
            }

            using (new EditorGUILayout.VerticalScope(AnimationSequencerStyles.InspectorSideMargins))
            {
                Rect rectWithMargins = new Rect(rect)
                {
                    xMin = rect.xMin + AnimationSequencerStyles.InspectorSideMargins.padding.left,
                    xMax = rect.xMax - AnimationSequencerStyles.InspectorSideMargins.padding.right,
                };

                var foldoutRect = new Rect(rectWithMargins)
                {
                    xMax = rectWithMargins.xMax - additionalHeaderWidth,
                };

                var additionalHeaderRect = new Rect(rectWithMargins)
                {
                    xMin = foldoutRect.xMax,
                };

                foldout = EditorGUI.Foldout(foldoutRect, foldout, title, true);

                additionalHeaderGUI?.Invoke(additionalHeaderRect, foldout);

                if (foldout)
                {
                    additionalInspectorGUI.Invoke();
                    GUILayout.Space(10);
                }
            }
        }

        private void OnDrawAnimationStepBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var titlebarRect = new Rect(rect)
                {
                    height = EditorGUIUtility.singleLineHeight,
                };

                if (isActive)
                    ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, true, isFocused, false);
                else
                    AnimationSequencerStyles.InspectorTitlebar.Draw(titlebarRect, false, false, false, false);
            }

            if (Event.current.type == EventType.Repaint &&
                DOTweenEditorPreview.isPreviewing &&
                previewingTimings != null &&
                index >= 0 && index < previewingTimings.Length)
            {
                var (start, end) = previewingTimings[index];

                var progress = GetCurrentSequencerProgress();

                var progressRect = new Rect(rect)
                {
                    xMin = Mathf.Lerp(rect.xMin, rect.xMax, start) - 1,
                    xMax = Mathf.Lerp(rect.xMin, rect.xMax, end) + 1,
                    height = EditorGUIUtility.singleLineHeight,
                };

                var markerRect = new Rect(rect)
                {
                    xMin = Mathf.Lerp(rect.xMin, rect.xMax, progress) - 1,
                    xMax = Mathf.Lerp(rect.xMin, rect.xMax, progress) + 1,
                    height = EditorGUIUtility.singleLineHeight,
                };

                var oldColor = GUI.color;

                GUI.color = new Color(0f, 0.5f, 0f, 0.45f);
                GUI.DrawTexture(progressRect, EditorGUIUtility.whiteTexture);

                GUI.color = Color.black;
                GUI.DrawTexture(markerRect, EditorGUIUtility.whiteTexture);

                GUI.color = oldColor;
            }
        }

        private void OnDrawAnimationStep(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty flowTypeSerializedProperty = element.FindPropertyRelative("flowType");

            if (!element.TryGetTargetObjectOfProperty(out AnimationStepBase animationStepBase))
                return;

            FlowType flowType = (FlowType)flowTypeSerializedProperty.enumValueIndex;

            int baseIdentLevel = EditorGUI.indentLevel;
            
            GUIContent guiContent = new GUIContent(element.displayName);
            if (animationStepBase != null)
                guiContent = new GUIContent(animationStepBase.GetDisplayNameForEditor(index + 1));

            if (flowType == FlowType.Join)
                EditorGUI.indentLevel = baseIdentLevel + 1;
            
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.x += 10;
            rect.width -= 20;

            EditorGUI.PropertyField(
                rect,
                element,
                guiContent,
                false
            );

            EditorGUI.indentLevel = baseIdentLevel;
            // DrawContextInputOnItem(element, index, rect);
        }

        private float GetAnimationStepHeight(int index)
        {
            if (index > reorderableList.serializedProperty.arraySize - 1)
                return EditorGUIUtility.singleLineHeight;
            
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            return element.GetPropertyDrawerHeight();
        }

        private void SetStepsExpanded(bool expanded)
        {
            SerializedProperty animationStepsProperty = reorderableList.serializedProperty;
            for (int i = 0; i < animationStepsProperty.arraySize; i++)
            {
                animationStepsProperty.GetArrayElementAtIndex(i).isExpanded = expanded;
            }
        }

        private void SetDefaults()
        {
            sequencerController = target as AnimationSequencerController;
            if (sequencerController != null)
            {
                sequencerController.SetAutoplayMode(AnimationControllerDefaults.Instance.AutoplayMode);
                sequencerController.SetPlayOnAwake(AnimationControllerDefaults.Instance.PlayOnAwake);
                sequencerController.SetPauseOnAwake(AnimationControllerDefaults.Instance.PauseOnAwake);
                sequencerController.SetTimeScaleIndependent(AnimationControllerDefaults.Instance.TimeScaleIndependent);
                sequencerController.SetPlayType(AnimationControllerDefaults.Instance.PlayType);
                sequencerController.SetUpdateType(AnimationControllerDefaults.Instance.UpdateType);
                sequencerController.SetAutoKill(AnimationControllerDefaults.Instance.AutoKill);
                sequencerController.SetLoops(AnimationControllerDefaults.Instance.Loops);
                sequencerController.ResetComplete();
            }
        }

        private static Rect DrawAutoSizedBadgeRight(Rect rect, string text, Color color)
        {
            var style = AnimationSequencerStyles.Badge;
            var size = style.CalcSize(EditorGUIUtility.TrTempContent(text));
            var buttonRect = new Rect(rect)
            {
                xMin = rect.xMax - size.x,
            };

            if (Event.current.type == EventType.Repaint)
            {
                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = color;
                style.Draw(buttonRect, text, false, false, true, false);
                GUI.backgroundColor = oldColor;
            }

            return new Rect(rect)
            {
                xMax = rect.xMax - size.x - style.margin.left,
            };
        }
    }
}
#endif
