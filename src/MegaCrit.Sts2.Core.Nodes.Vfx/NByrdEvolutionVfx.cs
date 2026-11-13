using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NByrdEvolutionVfx.cs")]
public class NByrdEvolutionVfx : Control, IOverlayScreen, IScreenContext
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : Control.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'SetSkins' method.
		/// </summary>
		public static readonly StringName SetSkins = "SetSkins";

		/// <summary>
		/// Cached name for the '_Input' method.
		/// </summary>
		public new static readonly StringName _Input = "_Input";

		/// <summary>
		/// Cached name for the 'SkipToFinale' method.
		/// </summary>
		public static readonly StringName SkipToFinale = "SkipToFinale";

		/// <summary>
		/// Cached name for the 'StartEvolution' method.
		/// </summary>
		public static readonly StringName StartEvolution = "StartEvolution";

		/// <summary>
		/// Cached name for the 'ShowByrdine' method.
		/// </summary>
		public static readonly StringName ShowByrdine = "ShowByrdine";

		/// <summary>
		/// Cached name for the 'ShowByrdpip' method.
		/// </summary>
		public static readonly StringName ShowByrdpip = "ShowByrdpip";

		/// <summary>
		/// Cached name for the 'Accelerate' method.
		/// </summary>
		public static readonly StringName Accelerate = "Accelerate";

		/// <summary>
		/// Cached name for the 'FireFinale' method.
		/// </summary>
		public static readonly StringName FireFinale = "FireFinale";

		/// <summary>
		/// Cached name for the 'CompleteAnimation' method.
		/// </summary>
		public static readonly StringName CompleteAnimation = "CompleteAnimation";

		/// <summary>
		/// Cached name for the 'StopAnimation' method.
		/// </summary>
		public static readonly StringName StopAnimation = "StopAnimation";

		/// <summary>
		/// Cached name for the 'StopAudio' method.
		/// </summary>
		public static readonly StringName StopAudio = "StopAudio";

		/// <summary>
		/// Cached name for the '_ExitTree' method.
		/// </summary>
		public new static readonly StringName _ExitTree = "_ExitTree";

		/// <summary>
		/// Cached name for the 'AfterOverlayOpened' method.
		/// </summary>
		public static readonly StringName AfterOverlayOpened = "AfterOverlayOpened";

		/// <summary>
		/// Cached name for the 'AfterOverlayClosed' method.
		/// </summary>
		public static readonly StringName AfterOverlayClosed = "AfterOverlayClosed";

		/// <summary>
		/// Cached name for the 'AfterOverlayShown' method.
		/// </summary>
		public static readonly StringName AfterOverlayShown = "AfterOverlayShown";

		/// <summary>
		/// Cached name for the 'AfterOverlayHidden' method.
		/// </summary>
		public static readonly StringName AfterOverlayHidden = "AfterOverlayHidden";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Control.PropertyName
	{
		/// <summary>
		/// Cached name for the 'DefaultFocusedControl' property.
		/// </summary>
		public static readonly StringName DefaultFocusedControl = "DefaultFocusedControl";

		/// <summary>
		/// Cached name for the 'ScreenType' property.
		/// </summary>
		public static readonly StringName ScreenType = "ScreenType";

		/// <summary>
		/// Cached name for the 'UseSharedBackstop' property.
		/// </summary>
		public static readonly StringName UseSharedBackstop = "UseSharedBackstop";

		/// <summary>
		/// Cached name for the '_finaleParticles' field.
		/// </summary>
		public static readonly StringName _finaleParticles = "_finaleParticles";

		/// <summary>
		/// Cached name for the '_powerParticles' field.
		/// </summary>
		public static readonly StringName _powerParticles = "_powerParticles";

		/// <summary>
		/// Cached name for the '_byrdpipContainer' field.
		/// </summary>
		public static readonly StringName _byrdpipContainer = "_byrdpipContainer";

		/// <summary>
		/// Cached name for the '_byrdineContainer' field.
		/// </summary>
		public static readonly StringName _byrdineContainer = "_byrdineContainer";

		/// <summary>
		/// Cached name for the '_curTweenDuration' field.
		/// </summary>
		public static readonly StringName _curTweenDuration = "_curTweenDuration";

		/// <summary>
		/// Cached name for the '_minTweenDuration' field.
		/// </summary>
		public static readonly StringName _minTweenDuration = "_minTweenDuration";

		/// <summary>
		/// Cached name for the '_accelerationMultiplier' field.
		/// </summary>
		public static readonly StringName _accelerationMultiplier = "_accelerationMultiplier";

		/// <summary>
		/// Cached name for the '_shrinkReps' field.
		/// </summary>
		public static readonly StringName _shrinkReps = "_shrinkReps";

		/// <summary>
		/// Cached name for the '_repsToShowPowerAt' field.
		/// </summary>
		public static readonly StringName _repsToShowPowerAt = "_repsToShowPowerAt";

		/// <summary>
		/// Cached name for the '_repsToStopPowerAt' field.
		/// </summary>
		public static readonly StringName _repsToStopPowerAt = "_repsToStopPowerAt";

		/// <summary>
		/// Cached name for the '_powerParticlesSpeedIncrement' field.
		/// </summary>
		public static readonly StringName _powerParticlesSpeedIncrement = "_powerParticlesSpeedIncrement";

		/// <summary>
		/// Cached name for the '_powerParticlesBaseSpeed' field.
		/// </summary>
		public static readonly StringName _powerParticlesBaseSpeed = "_powerParticlesBaseSpeed";

		/// <summary>
		/// Cached name for the '_pipTranslateTime' field.
		/// </summary>
		public static readonly StringName _pipTranslateTime = "_pipTranslateTime";

		/// <summary>
		/// Cached name for the '_pipHoldTime' field.
		/// </summary>
		public static readonly StringName _pipHoldTime = "_pipHoldTime";

		/// <summary>
		/// Cached name for the '_byrdpipTween' field.
		/// </summary>
		public static readonly StringName _byrdpipTween = "_byrdpipTween";

		/// <summary>
		/// Cached name for the '_byrdineTween' field.
		/// </summary>
		public static readonly StringName _byrdineTween = "_byrdineTween";

		/// <summary>
		/// Cached name for the '_particleDelayTween' field.
		/// </summary>
		public static readonly StringName _particleDelayTween = "_particleDelayTween";

		/// <summary>
		/// Cached name for the '_lifetimeTween' field.
		/// </summary>
		public static readonly StringName _lifetimeTween = "_lifetimeTween";

		/// <summary>
		/// Cached name for the '_startTween' field.
		/// </summary>
		public static readonly StringName _startTween = "_startTween";

		/// <summary>
		/// Cached name for the '_audioStarted' field.
		/// </summary>
		public static readonly StringName _audioStarted = "_audioStarted";

		/// <summary>
		/// Cached name for the '_finaleStarted' field.
		/// </summary>
		public static readonly StringName _finaleStarted = "_finaleStarted";

		/// <summary>
		/// Cached name for the '_basePipPosition' field.
		/// </summary>
		public static readonly StringName _basePipPosition = "_basePipPosition";

		/// <summary>
		/// Cached name for the '_fadeTween' field.
		/// </summary>
		public static readonly StringName _fadeTween = "_fadeTween";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
	}

	private const string _byrdEvolutionMusic = "event:/music/byrdlett_evolution_music";

	private const string _byrdEvolutionSfx = "event:/sfx/byrdpip/byrdlett_evolution";

	private GpuParticles2D _finaleParticles;

	private GpuParticles2D _powerParticles;

	private MegaSprite _byrdpipSprite;

	private MegaSprite _byrdineSprite;

	private Node2D _byrdpipContainer;

	private Node2D _byrdineContainer;

	private float _curTweenDuration = 0.5f;

	private float _minTweenDuration = 0.05f;

	private float _accelerationMultiplier = 0.85f;

	private int _shrinkReps = 10;

	private int _repsToShowPowerAt = 10;

	private int _repsToStopPowerAt;

	private float _powerParticlesSpeedIncrement = 1.11f;

	private float _powerParticlesBaseSpeed = 3f;

	private float _pipTranslateTime = 0.5f;

	private float _pipHoldTime = 0.25f;

	private Tween? _byrdpipTween;

	private Tween? _byrdineTween;

	private Tween? _particleDelayTween;

	private Tween? _lifetimeTween;

	private Tween? _startTween;

	private bool _audioStarted;

	private bool _finaleStarted;

	private Vector2 _basePipPosition;

	private Tween? _fadeTween;

	private Byrdpip _byrdpip;

	private Byrdine _byrdine;

	protected readonly TaskCompletionSource _completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

	private static string ScenePath => SceneHelper.GetScenePath("vfx/byrd_evolution_vfx");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public Control DefaultFocusedControl => this;

	public NetScreenType ScreenType => NetScreenType.None;

	public bool UseSharedBackstop => true;

	public static NByrdEvolutionVfx ShowScreen(Byrdpip byrdpip, Byrdine byrdine)
	{
		NByrdEvolutionVfx nByrdEvolutionVfx = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NByrdEvolutionVfx>(PackedScene.GenEditState.Disabled);
		nByrdEvolutionVfx._byrdpip = byrdpip;
		nByrdEvolutionVfx._byrdine = byrdine;
		NOverlayStack.Instance.Push(nByrdEvolutionVfx);
		return nByrdEvolutionVfx;
	}

	public override void _Ready()
	{
		_finaleParticles = GetNode<GpuParticles2D>("%FinaleParticles");
		_powerParticles = GetNode<GpuParticles2D>("%PowerParticles");
		_byrdpipContainer = GetNode<Node2D>("%ByrdpipContainer");
		_byrdineContainer = GetNode<Node2D>("%ByrdineContainer");
		Node2D node = GetNode<Node2D>("%ByrdpipSprite");
		Node2D node2 = GetNode<Node2D>("%ByrdineSprite");
		_byrdpipSprite = new MegaSprite(node);
		_byrdineSprite = new MegaSprite(node2);
		_byrdineContainer.Scale = Vector2.Zero;
		this.RunWhenSpineReady(_byrdpipSprite, (MegaAnimationState animState) =>
		{
			animState.SetAnimation("stand_still");
		});
		this.RunWhenSpineReady(_byrdineSprite, (MegaAnimationState animState) =>
		{
			animState.SetAnimation("stand_still");
		});
		_byrdpipContainer.Modulate = new Color(10f, 10f, 10f);
		_byrdineContainer.Modulate = new Color(10f, 10f, 10f);
		_finaleParticles.Emitting = false;
		_finaleParticles.OneShot = true;
		_powerParticles.Emitting = false;
		_powerParticles.SpeedScale = _powerParticlesBaseSpeed;
		_basePipPosition = _byrdpipContainer.Position;
		_byrdpipContainer.Translate(Vector2.Down * 800f);
		SetSkins();
	}

	private void SetSkins()
	{
		MegaSkeleton skeleton = _byrdpipSprite.GetSkeleton();
		MegaSkeletonDataResource data = skeleton.GetData();
		MegaSkin megaSkin = _byrdpipSprite.NewSkin("custom-skin");
		megaSkin.AddSkin(data.FindSkin(_byrdpip.Skin));
		skeleton.SetSkin(megaSkin);
		skeleton.SetSlotsToSetupPose();
		MegaSkeleton skeleton2 = _byrdineSprite.GetSkeleton();
		MegaSkeletonDataResource data2 = skeleton2.GetData();
		CharacterModel character = _byrdpip.Owner.Character;
		string text = ((character is Necrobinder) ? "necro" : character.Id.Entry.ToLowerInvariant());
		MegaSkin megaSkin2 = data2.FindSkin("eye/" + text);
		if (megaSkin2 == null)
		{
			megaSkin2 = data2.FindSkin("eye/ironclad");
		}
		megaSkin = _byrdineSprite.NewSkin("custom-skin");
		megaSkin.AddSkin(data2.FindSkin(_byrdine.Hair));
		megaSkin.AddSkin(megaSkin2);
		skeleton2.SetSkin(megaSkin);
		skeleton2.SetSlotsToSetupPose();
	}

	public async Task PlayAnimation()
	{
		SfxCmd.PlayLoop("event:/sfx/byrdpip/byrdlett_evolution", usesLoopParam: false);
		SfxCmd.PlayLoop("event:/music/byrdlett_evolution_music", usesLoopParam: false);
		_audioStarted = true;
		_startTween = CreateTween();
		_startTween.TweenCallback(Callable.From(StartEvolution)).SetDelay(0.75);
		await _completionSource.Task;
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (!_completionSource.Task.IsCompleted && ActiveScreenContext.Instance.GetCurrentScreen() == this && ((inputEvent is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Left && inputEventMouseButton.Pressed) || inputEvent.IsActionPressed(MegaInput.select) || inputEvent.IsActionPressed(MegaInput.cancel)))
		{
			GetViewport().SetInputAsHandled();
			SkipToFinale();
		}
	}

	private void SkipToFinale()
	{
		if (!_finaleStarted)
		{
			StopAnimation();
			_fadeTween?.FastForwardToCompletion();
			FireFinale();
		}
	}

	private void StartEvolution()
	{
		_byrdpipTween = CreateTween();
		_byrdpipTween.TweenProperty(_byrdpipContainer, "position", _basePipPosition, _pipTranslateTime).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
		_byrdpipTween.TweenCallback(Callable.From(ShowByrdine)).SetDelay(_pipHoldTime);
	}

	private void ShowByrdine()
	{
		Accelerate();
		_byrdpipTween = CreateTween();
		_byrdpipTween.TweenProperty(_byrdpipContainer, "scale", Vector2.Zero, _curTweenDuration).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
		_byrdpipTween.TweenInterval(_curTweenDuration);
		_byrdineTween = CreateTween();
		_byrdineTween.TweenInterval(_curTweenDuration);
		if (_shrinkReps > 0)
		{
			_byrdineTween.TweenProperty(_byrdineContainer, "scale", Vector2.One, _curTweenDuration).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
		}
		else
		{
			_byrdineTween.TweenProperty(_byrdineContainer, "scale", Vector2.One, _curTweenDuration * 0.6f).SetTrans(Tween.TransitionType.Linear);
		}
		_byrdineTween.TweenCallback(Callable.From(ShowByrdpip));
	}

	private void ShowByrdpip()
	{
		Accelerate();
		if (_shrinkReps > 0)
		{
			_byrdpipTween = CreateTween();
			_byrdpipTween?.TweenInterval(_curTweenDuration);
			_byrdpipTween?.TweenProperty(_byrdpipContainer, "scale", Vector2.One, _curTweenDuration).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
			_byrdineTween = CreateTween();
			_byrdineTween?.TweenProperty(_byrdineContainer, "scale", Vector2.Zero, _curTweenDuration).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
			_byrdineTween?.TweenInterval(_curTweenDuration);
			_byrdineTween?.TweenCallback(Callable.From(ShowByrdine));
		}
		else
		{
			_particleDelayTween = CreateTween();
			_particleDelayTween.TweenInterval(_curTweenDuration);
			_particleDelayTween.TweenCallback(Callable.From(FireFinale));
		}
		if (_shrinkReps == _repsToShowPowerAt)
		{
			_powerParticles.Restart();
		}
		else if (_shrinkReps == _repsToStopPowerAt)
		{
			_powerParticles.Emitting = false;
		}
		_shrinkReps--;
	}

	private void Accelerate()
	{
		if (_curTweenDuration * _accelerationMultiplier >= _minTweenDuration)
		{
			_curTweenDuration *= _accelerationMultiplier;
		}
		_powerParticles.SpeedScale *= (double)_powerParticlesSpeedIncrement;
	}

	private void FireFinale()
	{
		if (!_finaleStarted)
		{
			_finaleStarted = true;
			_byrdpipContainer.Scale = Vector2.Zero;
			_byrdineContainer.Scale = Vector2.One;
			_finaleParticles.Restart();
			_powerParticles.Restart();
			_powerParticles.Emitting = false;
			_byrdineContainer.Modulate = new Color(1f, 1f, 1f);
			_byrdineSprite.GetAnimationState().SetAnimation("be_born", loop: false);
			_lifetimeTween = CreateTween();
			_lifetimeTween.TweenCallback(Callable.From(CompleteAnimation)).SetDelay(3.0);
		}
	}

	private void CompleteAnimation()
	{
		if (_completionSource.TrySetResult())
		{
			StopAnimation();
			NOverlayStack.Instance?.Remove(this);
		}
	}

	private void StopAnimation()
	{
		_startTween?.Kill();
		_byrdpipTween?.Kill();
		_byrdineTween?.Kill();
		_particleDelayTween?.Kill();
		_lifetimeTween?.Kill();
		StopAudio();
	}

	private void StopAudio()
	{
		if (_audioStarted)
		{
			SfxCmd.StopLoop("event:/sfx/byrdpip/byrdlett_evolution");
			SfxCmd.StopLoop("event:/music/byrdlett_evolution_music");
			_audioStarted = false;
		}
	}

	public override void _ExitTree()
	{
		_startTween?.Kill();
		_byrdpipTween?.Kill();
		_byrdineTween?.Kill();
		_particleDelayTween?.Kill();
		_lifetimeTween?.Kill();
		_fadeTween?.Kill();
		StopAudio();
		_completionSource.TrySetResult();
	}

	public void AfterOverlayOpened()
	{
	}

	public void AfterOverlayClosed()
	{
		_fadeTween?.FastForwardToCompletion();
		_fadeTween = CreateTween();
		_fadeTween.TweenProperty(this, "modulate:a", 0, 0.25);
		_fadeTween.TweenCallback(Callable.From(this.QueueFreeSafely));
	}

	public void AfterOverlayShown()
	{
		_fadeTween?.FastForwardToCompletion();
		_fadeTween = CreateTween();
		_fadeTween.TweenProperty(this, "modulate:a", 1.0, 0.5).From(0f);
	}

	public void AfterOverlayHidden()
	{
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(17);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetSkins, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "inputEvent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SkipToFinale, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.StartEvolution, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ShowByrdine, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ShowByrdpip, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Accelerate, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.FireFinale, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.CompleteAnimation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.StopAnimation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.StopAudio, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AfterOverlayOpened, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AfterOverlayClosed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AfterOverlayShown, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AfterOverlayHidden, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default;
			return true;
		}
		if (method == MethodName.SetSkins && args.Count == 0)
		{
			SetSkins();
			ret = default;
			return true;
		}
		if (method == MethodName._Input && args.Count == 1)
		{
			_Input(VariantUtils.ConvertTo<InputEvent>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.SkipToFinale && args.Count == 0)
		{
			SkipToFinale();
			ret = default;
			return true;
		}
		if (method == MethodName.StartEvolution && args.Count == 0)
		{
			StartEvolution();
			ret = default;
			return true;
		}
		if (method == MethodName.ShowByrdine && args.Count == 0)
		{
			ShowByrdine();
			ret = default;
			return true;
		}
		if (method == MethodName.ShowByrdpip && args.Count == 0)
		{
			ShowByrdpip();
			ret = default;
			return true;
		}
		if (method == MethodName.Accelerate && args.Count == 0)
		{
			Accelerate();
			ret = default;
			return true;
		}
		if (method == MethodName.FireFinale && args.Count == 0)
		{
			FireFinale();
			ret = default;
			return true;
		}
		if (method == MethodName.CompleteAnimation && args.Count == 0)
		{
			CompleteAnimation();
			ret = default;
			return true;
		}
		if (method == MethodName.StopAnimation && args.Count == 0)
		{
			StopAnimation();
			ret = default;
			return true;
		}
		if (method == MethodName.StopAudio && args.Count == 0)
		{
			StopAudio();
			ret = default;
			return true;
		}
		if (method == MethodName._ExitTree && args.Count == 0)
		{
			_ExitTree();
			ret = default;
			return true;
		}
		if (method == MethodName.AfterOverlayOpened && args.Count == 0)
		{
			AfterOverlayOpened();
			ret = default;
			return true;
		}
		if (method == MethodName.AfterOverlayClosed && args.Count == 0)
		{
			AfterOverlayClosed();
			ret = default;
			return true;
		}
		if (method == MethodName.AfterOverlayShown && args.Count == 0)
		{
			AfterOverlayShown();
			ret = default;
			return true;
		}
		if (method == MethodName.AfterOverlayHidden && args.Count == 0)
		{
			AfterOverlayHidden();
			ret = default;
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.SetSkins)
		{
			return true;
		}
		if (method == MethodName._Input)
		{
			return true;
		}
		if (method == MethodName.SkipToFinale)
		{
			return true;
		}
		if (method == MethodName.StartEvolution)
		{
			return true;
		}
		if (method == MethodName.ShowByrdine)
		{
			return true;
		}
		if (method == MethodName.ShowByrdpip)
		{
			return true;
		}
		if (method == MethodName.Accelerate)
		{
			return true;
		}
		if (method == MethodName.FireFinale)
		{
			return true;
		}
		if (method == MethodName.CompleteAnimation)
		{
			return true;
		}
		if (method == MethodName.StopAnimation)
		{
			return true;
		}
		if (method == MethodName.StopAudio)
		{
			return true;
		}
		if (method == MethodName._ExitTree)
		{
			return true;
		}
		if (method == MethodName.AfterOverlayOpened)
		{
			return true;
		}
		if (method == MethodName.AfterOverlayClosed)
		{
			return true;
		}
		if (method == MethodName.AfterOverlayShown)
		{
			return true;
		}
		if (method == MethodName.AfterOverlayHidden)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._finaleParticles)
		{
			_finaleParticles = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._powerParticles)
		{
			_powerParticles = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._byrdpipContainer)
		{
			_byrdpipContainer = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._byrdineContainer)
		{
			_byrdineContainer = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._curTweenDuration)
		{
			_curTweenDuration = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._minTweenDuration)
		{
			_minTweenDuration = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._accelerationMultiplier)
		{
			_accelerationMultiplier = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._shrinkReps)
		{
			_shrinkReps = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._repsToShowPowerAt)
		{
			_repsToShowPowerAt = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._repsToStopPowerAt)
		{
			_repsToStopPowerAt = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._powerParticlesSpeedIncrement)
		{
			_powerParticlesSpeedIncrement = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._powerParticlesBaseSpeed)
		{
			_powerParticlesBaseSpeed = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._pipTranslateTime)
		{
			_pipTranslateTime = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._pipHoldTime)
		{
			_pipHoldTime = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._byrdpipTween)
		{
			_byrdpipTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._byrdineTween)
		{
			_byrdineTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._particleDelayTween)
		{
			_particleDelayTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._lifetimeTween)
		{
			_lifetimeTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._startTween)
		{
			_startTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._audioStarted)
		{
			_audioStarted = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._finaleStarted)
		{
			_finaleStarted = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._basePipPosition)
		{
			_basePipPosition = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._fadeTween)
		{
			_fadeTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.DefaultFocusedControl)
		{
			value = VariantUtils.CreateFrom<Control>(DefaultFocusedControl);
			return true;
		}
		if (name == PropertyName.ScreenType)
		{
			value = VariantUtils.CreateFrom<NetScreenType>(ScreenType);
			return true;
		}
		if (name == PropertyName.UseSharedBackstop)
		{
			value = VariantUtils.CreateFrom<bool>(UseSharedBackstop);
			return true;
		}
		if (name == PropertyName._finaleParticles)
		{
			value = VariantUtils.CreateFrom(in _finaleParticles);
			return true;
		}
		if (name == PropertyName._powerParticles)
		{
			value = VariantUtils.CreateFrom(in _powerParticles);
			return true;
		}
		if (name == PropertyName._byrdpipContainer)
		{
			value = VariantUtils.CreateFrom(in _byrdpipContainer);
			return true;
		}
		if (name == PropertyName._byrdineContainer)
		{
			value = VariantUtils.CreateFrom(in _byrdineContainer);
			return true;
		}
		if (name == PropertyName._curTweenDuration)
		{
			value = VariantUtils.CreateFrom(in _curTweenDuration);
			return true;
		}
		if (name == PropertyName._minTweenDuration)
		{
			value = VariantUtils.CreateFrom(in _minTweenDuration);
			return true;
		}
		if (name == PropertyName._accelerationMultiplier)
		{
			value = VariantUtils.CreateFrom(in _accelerationMultiplier);
			return true;
		}
		if (name == PropertyName._shrinkReps)
		{
			value = VariantUtils.CreateFrom(in _shrinkReps);
			return true;
		}
		if (name == PropertyName._repsToShowPowerAt)
		{
			value = VariantUtils.CreateFrom(in _repsToShowPowerAt);
			return true;
		}
		if (name == PropertyName._repsToStopPowerAt)
		{
			value = VariantUtils.CreateFrom(in _repsToStopPowerAt);
			return true;
		}
		if (name == PropertyName._powerParticlesSpeedIncrement)
		{
			value = VariantUtils.CreateFrom(in _powerParticlesSpeedIncrement);
			return true;
		}
		if (name == PropertyName._powerParticlesBaseSpeed)
		{
			value = VariantUtils.CreateFrom(in _powerParticlesBaseSpeed);
			return true;
		}
		if (name == PropertyName._pipTranslateTime)
		{
			value = VariantUtils.CreateFrom(in _pipTranslateTime);
			return true;
		}
		if (name == PropertyName._pipHoldTime)
		{
			value = VariantUtils.CreateFrom(in _pipHoldTime);
			return true;
		}
		if (name == PropertyName._byrdpipTween)
		{
			value = VariantUtils.CreateFrom(in _byrdpipTween);
			return true;
		}
		if (name == PropertyName._byrdineTween)
		{
			value = VariantUtils.CreateFrom(in _byrdineTween);
			return true;
		}
		if (name == PropertyName._particleDelayTween)
		{
			value = VariantUtils.CreateFrom(in _particleDelayTween);
			return true;
		}
		if (name == PropertyName._lifetimeTween)
		{
			value = VariantUtils.CreateFrom(in _lifetimeTween);
			return true;
		}
		if (name == PropertyName._startTween)
		{
			value = VariantUtils.CreateFrom(in _startTween);
			return true;
		}
		if (name == PropertyName._audioStarted)
		{
			value = VariantUtils.CreateFrom(in _audioStarted);
			return true;
		}
		if (name == PropertyName._finaleStarted)
		{
			value = VariantUtils.CreateFrom(in _finaleStarted);
			return true;
		}
		if (name == PropertyName._basePipPosition)
		{
			value = VariantUtils.CreateFrom(in _basePipPosition);
			return true;
		}
		if (name == PropertyName._fadeTween)
		{
			value = VariantUtils.CreateFrom(in _fadeTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	/// <summary>
	/// Get the property information for all the properties declared in this class.
	/// This method is used by Godot to register the available properties in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._finaleParticles, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._powerParticles, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._byrdpipContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._byrdineContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._curTweenDuration, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._minTweenDuration, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._accelerationMultiplier, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._shrinkReps, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._repsToShowPowerAt, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._repsToStopPowerAt, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._powerParticlesSpeedIncrement, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._powerParticlesBaseSpeed, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._pipTranslateTime, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._pipHoldTime, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._byrdpipTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._byrdineTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._particleDelayTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._lifetimeTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._startTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._audioStarted, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._finaleStarted, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._basePipPosition, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._fadeTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.DefaultFocusedControl, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName.ScreenType, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName.UseSharedBackstop, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._finaleParticles, Variant.From(in _finaleParticles));
		info.AddProperty(PropertyName._powerParticles, Variant.From(in _powerParticles));
		info.AddProperty(PropertyName._byrdpipContainer, Variant.From(in _byrdpipContainer));
		info.AddProperty(PropertyName._byrdineContainer, Variant.From(in _byrdineContainer));
		info.AddProperty(PropertyName._curTweenDuration, Variant.From(in _curTweenDuration));
		info.AddProperty(PropertyName._minTweenDuration, Variant.From(in _minTweenDuration));
		info.AddProperty(PropertyName._accelerationMultiplier, Variant.From(in _accelerationMultiplier));
		info.AddProperty(PropertyName._shrinkReps, Variant.From(in _shrinkReps));
		info.AddProperty(PropertyName._repsToShowPowerAt, Variant.From(in _repsToShowPowerAt));
		info.AddProperty(PropertyName._repsToStopPowerAt, Variant.From(in _repsToStopPowerAt));
		info.AddProperty(PropertyName._powerParticlesSpeedIncrement, Variant.From(in _powerParticlesSpeedIncrement));
		info.AddProperty(PropertyName._powerParticlesBaseSpeed, Variant.From(in _powerParticlesBaseSpeed));
		info.AddProperty(PropertyName._pipTranslateTime, Variant.From(in _pipTranslateTime));
		info.AddProperty(PropertyName._pipHoldTime, Variant.From(in _pipHoldTime));
		info.AddProperty(PropertyName._byrdpipTween, Variant.From(in _byrdpipTween));
		info.AddProperty(PropertyName._byrdineTween, Variant.From(in _byrdineTween));
		info.AddProperty(PropertyName._particleDelayTween, Variant.From(in _particleDelayTween));
		info.AddProperty(PropertyName._lifetimeTween, Variant.From(in _lifetimeTween));
		info.AddProperty(PropertyName._startTween, Variant.From(in _startTween));
		info.AddProperty(PropertyName._audioStarted, Variant.From(in _audioStarted));
		info.AddProperty(PropertyName._finaleStarted, Variant.From(in _finaleStarted));
		info.AddProperty(PropertyName._basePipPosition, Variant.From(in _basePipPosition));
		info.AddProperty(PropertyName._fadeTween, Variant.From(in _fadeTween));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._finaleParticles, out var value))
		{
			_finaleParticles = value.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._powerParticles, out var value2))
		{
			_powerParticles = value2.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._byrdpipContainer, out var value3))
		{
			_byrdpipContainer = value3.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._byrdineContainer, out var value4))
		{
			_byrdineContainer = value4.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._curTweenDuration, out var value5))
		{
			_curTweenDuration = value5.As<float>();
		}
		if (info.TryGetProperty(PropertyName._minTweenDuration, out var value6))
		{
			_minTweenDuration = value6.As<float>();
		}
		if (info.TryGetProperty(PropertyName._accelerationMultiplier, out var value7))
		{
			_accelerationMultiplier = value7.As<float>();
		}
		if (info.TryGetProperty(PropertyName._shrinkReps, out var value8))
		{
			_shrinkReps = value8.As<int>();
		}
		if (info.TryGetProperty(PropertyName._repsToShowPowerAt, out var value9))
		{
			_repsToShowPowerAt = value9.As<int>();
		}
		if (info.TryGetProperty(PropertyName._repsToStopPowerAt, out var value10))
		{
			_repsToStopPowerAt = value10.As<int>();
		}
		if (info.TryGetProperty(PropertyName._powerParticlesSpeedIncrement, out var value11))
		{
			_powerParticlesSpeedIncrement = value11.As<float>();
		}
		if (info.TryGetProperty(PropertyName._powerParticlesBaseSpeed, out var value12))
		{
			_powerParticlesBaseSpeed = value12.As<float>();
		}
		if (info.TryGetProperty(PropertyName._pipTranslateTime, out var value13))
		{
			_pipTranslateTime = value13.As<float>();
		}
		if (info.TryGetProperty(PropertyName._pipHoldTime, out var value14))
		{
			_pipHoldTime = value14.As<float>();
		}
		if (info.TryGetProperty(PropertyName._byrdpipTween, out var value15))
		{
			_byrdpipTween = value15.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._byrdineTween, out var value16))
		{
			_byrdineTween = value16.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._particleDelayTween, out var value17))
		{
			_particleDelayTween = value17.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._lifetimeTween, out var value18))
		{
			_lifetimeTween = value18.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._startTween, out var value19))
		{
			_startTween = value19.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._audioStarted, out var value20))
		{
			_audioStarted = value20.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._finaleStarted, out var value21))
		{
			_finaleStarted = value21.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._basePipPosition, out var value22))
		{
			_basePipPosition = value22.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._fadeTween, out var value23))
		{
			_fadeTween = value23.As<Tween>();
		}
	}
}
