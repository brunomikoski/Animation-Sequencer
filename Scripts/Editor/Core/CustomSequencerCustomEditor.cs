#if DOTWEEN_ENABLED
using System;
using DG.Tweening;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace BrunoMikoski.AnimationSequencer
{
    public class CustomSequencerCustomEditor : EditorWindow
    {
        private const string STYLE_SHEET_NAME = "sequencer";
        
        
        
        private static class UssClassNames
        {
            internal const string Root = "sequencer";
            internal const string Box = "box";
            public const string Foldout = "foldout";
            public const string ControlButton = "controlButton";
            public const string ControlButtonIcon = "controlButton-icon";
            public const string BackIcon = "backIcon";
            public const string StepBackIcon = "stepBackIcon";
            public const string PlayIcon = "playIcon";
            public const string StepForwardIcon = "stepForward";
            public const string ForwardIcon = "forward";
            public const string StopIcon = "stop";
            public const string ControlBox = "controlBox";
        }

        
        [SerializeField]
        private float tweenTimeScale = 1f;

        private SerializedObject sequencerSerializedObject;
        private AnimationSequence animationSequence;


        [MenuItem("Window/Animation Sequencer")]
        public static void ShowWindow()
        {
            CustomSequencerCustomEditor window = GetWindow<CustomSequencerCustomEditor>();
            window.titleContent = new GUIContent("UI Elements Sequencer");
        }
        //
        //
        private void OnEnable()
        {
            // if (Selection.activeObject == null)
            //     return;
            //
            // if (Selection.activeObject is GameObject gameObject)
            //     sequencer = gameObject.GetComponent<Sequencer>();
            // else if (Selection.activeObject is Sequencer customSequencerDrawer)
            //     sequencer = customSequencerDrawer;
            // else
            //     return;
            //
            // sequencerSerializedObject = new SerializedObject(sequencer);
            //
            //
            // CreateVisuals();
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is GameObject gameObject)
            {
                animationSequence = gameObject.GetComponent<AnimationSequence>();
            }
            else if (Selection.activeObject is AnimationSequence customSequencerDrawer)
            {
                animationSequence = customSequencerDrawer;
            }
            else
            {
                animationSequence = null;
                return;
            }

            if (animationSequence != null)
            {
                sequencerSerializedObject = new SerializedObject(animationSequence);
                CreateVisuals();
            }
        }

        private void CreateVisuals()
        {
            rootVisualElement.Clear();
            rootVisualElement.AddToClassList(UssClassNames.Root);
            rootVisualElement.Bind(sequencerSerializedObject);

            LoadAndApplyStyleSheet(rootVisualElement);
            AddSequenceSettings(rootVisualElement);
            AddCallbacks(rootVisualElement);
            AddPreviewControls(rootVisualElement);
            AddStepsList(rootVisualElement);
        }
        

        private void AddStepsList(VisualElement parentElement)
        {
            
        }

        private void AddPreviewControls(VisualElement parentElement)
        {
            Box box = new Box();
            box.AddToClassList(UssClassNames.Box);
            
            Foldout foldout = new Foldout
            {
                text = "Preview"
            };
            foldout.AddToClassList(UssClassNames.Foldout);
            box.Add(foldout);
            
           
            Box controlsBox = new Box();
            controlsBox.AddToClassList(UssClassNames.ControlBox);

            Button backButton = new Button();
            backButton.AddToClassList(UssClassNames.ControlButton);
            Box backIcon = new Box();
            backIcon.AddToClassList(UssClassNames.ControlButtonIcon);
            backIcon.AddToClassList(UssClassNames.BackIcon);
            backButton.Add(backIcon);
            
            controlsBox.Add(backButton);

            
            
            Button stepBackButton = new Button();
            stepBackButton.AddToClassList(UssClassNames.ControlButton);
            Box stepBackIcon = new Box();
            stepBackIcon.AddToClassList(UssClassNames.ControlButtonIcon);
            stepBackIcon.AddToClassList(UssClassNames.StepBackIcon);
            stepBackButton.Add(stepBackIcon);
            
            controlsBox.Add(stepBackButton);

            Button playButton = new Button();
            playButton.AddToClassList(UssClassNames.ControlButton);
            Box playButtonIcon = new Box();
            playButtonIcon.AddToClassList(UssClassNames.ControlButtonIcon);
            playButtonIcon.AddToClassList(UssClassNames.PlayIcon);
            playButton.Add(playButtonIcon);
            
            controlsBox.Add(playButton);
            
            
            Button stepForwardButton = new Button();
            stepForwardButton.AddToClassList(UssClassNames.ControlButton);
            Box stepForwardICon = new Box();
            stepForwardICon.AddToClassList(UssClassNames.ControlButtonIcon);
            stepForwardICon.AddToClassList(UssClassNames.StepForwardIcon);
            stepForwardButton.Add(stepForwardICon);
            
            controlsBox.Add(stepForwardButton);
            
            
            Button forwardButton = new Button();
            forwardButton.AddToClassList(UssClassNames.ControlButton);
            Box forwardIcon = new Box();
            forwardIcon.AddToClassList(UssClassNames.ControlButtonIcon);
            forwardIcon.AddToClassList(UssClassNames.ForwardIcon);
            forwardButton.Add(forwardIcon);
            
            controlsBox.Add(forwardButton);
            
            
            Button stopButton = new Button();
            stopButton.AddToClassList(UssClassNames.ControlButton);
            Box stopIcon = new Box();
            stopIcon.AddToClassList(UssClassNames.ControlButtonIcon);
            stopIcon.AddToClassList(UssClassNames.StopIcon);
            stopButton.Add(stopIcon);
            
            controlsBox.Add(stopButton);
            
            foldout.Add(controlsBox);




            Label timescaleLabel = new Label("Timescale");
            foldout.Add(timescaleLabel);
            Slider timescaleSlider = new Slider(0, 2)
            {
                showInputField = true
            };
            timescaleSlider.value = tweenTimeScale;
            timescaleSlider.RegisterValueChangedCallback(v =>
            {
                OnTimeScaleValueChanged(v.newValue);
            });
            foldout.Add(timescaleSlider);
            
            
            Label progressLabel = new Label("Progress");
            foldout.Add(progressLabel);
            Slider progressSlider = new Slider(0,1)
            {
                showInputField = true,
            };
            SerializedProperty progressSerializedProperty = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfProgress);
            progressSlider.value = progressSerializedProperty.floatValue;
            // progressSlider.RegisterValueChangedCallback(v =>
            // {
            //     OnProgressSliderValueChanged(v.newValue);
            // });
            foldout.BindProperty(progressSerializedProperty);
            foldout.Add(progressSlider);
            
            parentElement.Add(box);
        }

        private void OnProgressSliderValueChanged(float newProgressValue)
        {
        }

        private void OnTimeScaleValueChanged(float newTimescaleValue)
        {
            tweenTimeScale = newTimescaleValue;
        }

        private void AddCallbacks(VisualElement parentElement)
        {
            Box box = new Box();
            box.AddToClassList(UssClassNames.Box);
            
            
            Foldout foldout = new Foldout
            {
                text = "Callbacks"
            };
            foldout.value = false;
            foldout.AddToClassList(UssClassNames.Foldout);
            box.Add(foldout);
            
            SerializedProperty onStartEventSerializedProperty = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfOnStartEvent);
            SerializedProperty onFinishedEventSerializedProperty = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfOnFinishedEvent);
            SerializedProperty onProgressEventSerializedProperty = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfOnProgressEvent);

            
            foldout.Add(new PropertyField(onStartEventSerializedProperty));
            foldout.Add(new PropertyField(onFinishedEventSerializedProperty));
            foldout.Add(new PropertyField(onProgressEventSerializedProperty));
            
            parentElement.Add(box);
        }

        private void AddSequenceSettings(VisualElement parentElement)
        {
            Box foldoutBox = new Box();
            foldoutBox.AddToClassList(UssClassNames.Box);
            
            
            Foldout foldout = new Foldout
            {
                text = "Sequence Settings"
            };
            foldout.AddToClassList(UssClassNames.Foldout);


            SerializedProperty autoPlayModeSerializedProperty = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfAutoPlayMode);

            EnumField autoPlayModeEnum = new EnumField("Auto Play", AnimationSequence.AutoplayType.Awake);
            autoPlayModeEnum.BindProperty(autoPlayModeSerializedProperty);
            foldout.Add(autoPlayModeEnum);
            

            SerializedProperty pauseOnAwakeSerializedProperty = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfStartPaused);
            Toggle loopToggle = new Toggle("Start Paused");
            loopToggle.BindProperty(pauseOnAwakeSerializedProperty);
            foldout.Add(loopToggle);

            
            SerializedProperty playbackSpeedSerializedProperty = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfPlaybackSpeed);
            Slider playbackSpeedSlider = new Slider("Playback Speed", 0,2)
            {
                showInputField = true,
            };
            playbackSpeedSlider.BindProperty(playbackSpeedSerializedProperty);
            
            foldout.Add(playbackSpeedSlider);
            
            
            SerializedProperty timescaleIndependent = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfTimeScaleIndependent);
            Toggle timescaleIndependentToggle = new Toggle("Timescale Independent");
            timescaleIndependentToggle.BindProperty(timescaleIndependent);
            foldout.Add(timescaleIndependentToggle);
            
            
            SerializedProperty playTypeSerializedProperty = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfPlayType);
            EnumField playTypeEnumField = new EnumField("Play Type", (AnimationSequence.PlayType)playTypeSerializedProperty.enumValueIndex);
            playTypeEnumField.BindProperty(playTypeSerializedProperty);
            foldout.Add(playTypeEnumField);
            

            SerializedProperty updateTypeSerializedProperty = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfUpdateType);
            EnumField updateType = new EnumField("UpdateType Type",(UpdateType) updateTypeSerializedProperty.enumValueIndex);
            updateType.BindProperty(updateTypeSerializedProperty);
            foldout.Add(updateType);

            
            SerializedProperty autoKillSerializedProperty = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfAutoKill);
            Toggle autoKillToggle = new Toggle("Auto Kill");
            autoKillToggle.BindProperty(autoKillSerializedProperty);
            foldout.Add(autoKillToggle);
            
            SerializedProperty loopCountSerializedProperty = sequencerSerializedObject.FindProperty(AnimationSequenceEditorUtils.NameOfLoops);
            IntegerField loopCountField = new IntegerField("Loop Count");
            loopCountField.BindProperty(loopCountSerializedProperty);
            foldout.Add(loopCountField);
            
            
            foldoutBox.Add(foldout);
            parentElement.Add(foldoutBox);
        }


        private void LoadAndApplyStyleSheet(VisualElement rootElement)
        {
            string[] guids = AssetDatabase.FindAssets($"t:StyleSheet {STYLE_SHEET_NAME}");
            if (guids.Length == 0)
                throw new Exception($"Failed to find style sheet with name :{STYLE_SHEET_NAME}");
            
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guids[0]));
            rootElement.styleSheets.Add(styleSheet);
        }
    }
}
#endif