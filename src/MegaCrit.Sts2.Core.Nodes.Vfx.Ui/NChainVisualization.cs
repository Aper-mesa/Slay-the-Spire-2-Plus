using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Ui;

[ScriptPath("res://src/Core/Nodes/Vfx/Ui/NChainVisualization.cs")]
public class NChainVisualization : Control
{
	private enum State
	{
		None,
		Default,
		Breaking,
		Broken
	}

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
		/// Cached name for the 'Initialize' method.
		/// </summary>
		public static readonly StringName Initialize = "Initialize";

		/// <summary>
		/// Cached name for the 'CancelPendingBreak' method.
		/// </summary>
		public static readonly StringName CancelPendingBreak = "CancelPendingBreak";

		/// <summary>
		/// Cached name for the 'StopWindupSound' method.
		/// </summary>
		public static readonly StringName StopWindupSound = "StopWindupSound";

		/// <summary>
		/// Cached name for the 'SetXOffset' method.
		/// </summary>
		public static readonly StringName SetXOffset = "SetXOffset";

		/// <summary>
		/// Cached name for the 'Reset' method.
		/// </summary>
		public static readonly StringName Reset = "Reset";

		/// <summary>
		/// Cached name for the '_Process' method.
		/// </summary>
		public new static readonly StringName _Process = "_Process";

		/// <summary>
		/// Cached name for the 'Update' method.
		/// </summary>
		public static readonly StringName Update = "Update";

		/// <summary>
		/// Cached name for the 'BreakingUpdate' method.
		/// </summary>
		public static readonly StringName BreakingUpdate = "BreakingUpdate";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Control.PropertyName
	{
		/// <summary>
		/// Cached name for the 'IsBreaking' property.
		/// </summary>
		public static readonly StringName IsBreaking = "IsBreaking";

		/// <summary>
		/// Cached name for the '_originalLinkImage' field.
		/// </summary>
		public static readonly StringName _originalLinkImage = "_originalLinkImage";

		/// <summary>
		/// Cached name for the '_blendWeightSum' field.
		/// </summary>
		public static readonly StringName _blendWeightSum = "_blendWeightSum";

		/// <summary>
		/// Cached name for the '_time' field.
		/// </summary>
		public static readonly StringName _time = "_time";

		/// <summary>
		/// Cached name for the '_breakStartTime' field.
		/// </summary>
		public static readonly StringName _breakStartTime = "_breakStartTime";

		/// <summary>
		/// Cached name for the '_noise' field.
		/// </summary>
		public static readonly StringName _noise = "_noise";

		/// <summary>
		/// Cached name for the '_lowFreqNoise' field.
		/// </summary>
		public static readonly StringName _lowFreqNoise = "_lowFreqNoise";

		/// <summary>
		/// Cached name for the '_xOffset' field.
		/// </summary>
		public static readonly StringName _xOffset = "_xOffset";

		/// <summary>
		/// Cached name for the '_windupSoundId' field.
		/// </summary>
		public static readonly StringName _windupSoundId = "_windupSoundId";

		/// <summary>
		/// Cached name for the '_state' field.
		/// </summary>
		public static readonly StringName _state = "_state";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
	}

	private static readonly string _linkPath = SceneHelper.GetScenePath("screens/dread/chain_link");

	private static readonly string _breakSparkPath = SceneHelper.GetScenePath("screens/dread/chain_break_vfx");

	private static readonly string _linkCreateVfxPath = SceneHelper.GetScenePath("screens/dread/chain_link_create_vfx");

	private static readonly string[] _brokenLinkPaths = new string[4]
	{
		ImageHelper.GetImagePath("ui/chain_break_1.png"),
		ImageHelper.GetImagePath("ui/chain_break_2.png"),
		ImageHelper.GetImagePath("ui/chain_break_3.png"),
		ImageHelper.GetImagePath("ui/chain_break_4.png")
	};

	private static readonly float[] _blendWeights = new float[7] { 1f, 1f, 2f, 6f, 2f, 1f, 1f };

	private const float _stretchedLinkSeparation = 85f;

	private const float _defaultLinkSeparation = 70f;

	private const float _maxPerturb = 40f;

	private const float _maxLinkYOffset = 15f;

	private const float _linkMinScale = 0.8f;

	private const float _linkMaxScaleChange = 0.1f;

	private const float _xPosSpeed = 0.12f;

	private const float _yPosSpeed = 0.08f;

	private const float _scaleSpeed = 0.2f;

	private const float _minBreakStretchTime = 0.1f;

	private const float _maxBreakStretchTime = 1f;

	private const int _maxBreakStretchLinks = 5;

	private Texture2D _originalLinkImage;

	private float _blendWeightSum;

	private readonly List<Control> _links = new List<Control>();

	private double _time;

	private double _breakStartTime = -1.0;

	private FastNoiseLite _noise = new FastNoiseLite();

	private FastNoiseLite _lowFreqNoise = new FastNoiseLite();

	private float _xOffset;

	private readonly List<float> _samples = new List<float>();

	private readonly List<float> _blendedSamples = new List<float>();

	private readonly List<Vector2> _points = new List<Vector2>();

	private int _windupSoundId = -1;

	private State _state = State.Default;

	private TaskCompletionSource? _breakCompletedSource;

	private TaskCompletionSource? _breakFadeOutCompletedSource;

	private OwnedTween _tween;

	public bool IsBreaking
	{
		get
		{
			if (_state != State.Breaking)
			{
				return _state == State.Broken;
			}
			return true;
		}
	}

	public override void _Ready()
	{
		_blendWeightSum = _blendWeights.Sum();
		_noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		_noise.Frequency = 1f;
		_lowFreqNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		_lowFreqNoise.Frequency = 1f;
		_lowFreqNoise.FractalOctaves = 8;
		_tween = new OwnedTween(this);
	}

	public void Initialize(int count)
	{
		CancelPendingBreak();
		_state = State.Default;
		_breakStartTime = -1.0;
		_points.Clear();
		foreach (Control link in _links)
		{
			link.QueueFreeSafely();
		}
		_links.Clear();
		for (int i = 0; i < count; i++)
		{
			Control control = PreloadManager.Cache.GetScene(_linkPath).Instantiate<Control>(PackedScene.GenEditState.Disabled);
			this.AddChildSafely(control);
			_links.Add(control);
			_originalLinkImage = control.GetNode<TextureRect>("TextureRect").Texture;
		}
		_xOffset = (float)(-_links.Count) * 70f * 0.5f;
	}

	public Task Break(float delay)
	{
		CancelPendingBreak();
		_state = State.Default;
		Update();
		_breakStartTime = _time + (double)delay;
		_breakCompletedSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
		_breakFadeOutCompletedSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
		return _breakCompletedSource.Task;
	}

	/// <summary>
	/// Abandons a break that has not finished. The caller awaiting it sees an
	/// <see cref="T:System.OperationCanceledException" />.
	/// </summary>
	private void CancelPendingBreak()
	{
		_breakCompletedSource?.TrySetCanceled();
		_breakFadeOutCompletedSource?.TrySetCanceled();
		_tween.Kill();
		StopWindupSound();
	}

	/// <summary>
	/// <see cref="M:MegaCrit.Sts2.Core.Audio.Debug.NDebugAudioManager.Stop(System.Int32,System.Single)" /> warns about ids it cannot find, so only stop a live windup.
	/// </summary>
	private void StopWindupSound()
	{
		if (_windupSoundId >= 0)
		{
			NDebugAudioManager.Instance.Stop(_windupSoundId, 0.1f);
			_windupSoundId = -1;
		}
	}

	public async Task AnimateInLastLink(float delay, TaskCompletionSource? animateInCompleteSource)
	{
		if (_links.Count == 0)
		{
			animateInCompleteSource?.SetResult();
			return;
		}
		List<Control> links = _links;
		Control link = links[links.Count - 1];
		Color originalColor = link.Modulate;
		link.Modulate = Colors.Transparent;
		Control control = PreloadManager.Cache.GetScene(_linkCreateVfxPath).Instantiate<Control>(PackedScene.GenEditState.Disabled);
		link.AddChildSafely(control);
		control.Position = Vector2.Zero;
		control.Size = link.Size;
		TextureRect additive = control.GetNode<TextureRect>("%Additive");
		GpuParticles2D shineVfx = control.GetNode<GpuParticles2D>("%ShineVfx");
		GpuParticles2D[] sparkVfx = control.GetNode("%Spark").GetChildren().OfType<GpuParticles2D>()
			.ToArray();
		additive.Modulate = Colors.Transparent;
		await _tween.Play((Tween tween) =>
		{
			tween.TweenInterval(delay);
			if (_links.Count > 1)
			{
				float xOffset = _xOffset;
				SetXOffset((float)(-(_links.Count - 1)) * 70f * 0.5f);
				tween.TweenMethod(Callable.From<float>(SetXOffset), _xOffset, xOffset, 0.5).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Expo);
				tween.Parallel().TweenCallback(Callable.From(() => NDebugAudioManager.Instance.Play("chain_get.wav"))).SetDelay(0.30000001192092896);
			}
			else
			{
				tween.TweenCallback(Callable.From(() => NDebugAudioManager.Instance.Play("chain_get.wav")));
				tween.TweenInterval(0.20000000298023224);
			}
			tween.TweenCallback(Callable.From(() =>
			{
				link.Modulate = originalColor;
				additive.Modulate = Colors.White;
				GpuParticles2D[] array = sparkVfx;
				foreach (GpuParticles2D gpuParticles2D in array)
				{
					gpuParticles2D.Emitting = true;
				}
				animateInCompleteSource?.SetResult();
			}));
			tween.TweenProperty(additive, "self_modulate:a", 0f, 0.3).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Expo);
			tween.TweenCallback(Callable.From(() =>
			{
				if (shineVfx.IsValid())
				{
					shineVfx.Emitting = true;
				}
			}));
		});
		animateInCompleteSource?.TrySetResult();
	}

	private void SetXOffset(float xOffset)
	{
		_xOffset = xOffset;
	}

	public void Reset()
	{
		_state = State.Default;
		foreach (Control link in _links)
		{
			link.GetNode<TextureRect>("TextureRect").Texture = _originalLinkImage;
			Color modulate = link.Modulate;
			modulate.A = 1f;
			link.Modulate = modulate;
		}
	}

	public override void _Process(double delta)
	{
		_time += delta;
		if (_state == State.Default && _breakStartTime >= 0.0 && _time >= _breakStartTime)
		{
			_state = State.Breaking;
			_windupSoundId = NDebugAudioManager.Instance.Play("chain_break_windup.wav");
		}
		if (_state == State.Breaking)
		{
			BreakingUpdate();
		}
		else if (_state == State.Default)
		{
			Update();
		}
	}

	private void Update()
	{
		_samples.Clear();
		_blendedSamples.Clear();
		_points.Clear();
		if (_links.Count == 0)
		{
			return;
		}
		float item = (_lowFreqNoise.GetNoise3D((float)_time * 0.12f, 0f, 0f) + 1f) * 0.5f;
		_samples.Add(item);
		for (int i = 0; i < _links.Count - 1; i++)
		{
			float noise3D = _lowFreqNoise.GetNoise3D((float)_time * 0.12f, (i + 1) * 7, i + 1);
			_samples.Add(noise3D);
		}
		float item2 = (0f - (_lowFreqNoise.GetNoise3D((float)_time * 0.12f, (_links.Count + 1) * 7, _links.Count + 1) + 1f)) * 0.5f;
		_samples.Add(item2);
		for (int j = 0; j < _samples.Count; j++)
		{
			float num = 0f;
			float num2 = 0f;
			int num3 = _blendWeights.Length / 2;
			for (int k = Mathf.Max(j - num3, 0); k <= Mathf.Min(j + num3, _samples.Count - 1); k++)
			{
				int num4 = k - j + num3;
				float num5 = _blendWeights[num4] / _blendWeightSum;
				num += num5 * _samples[k];
				num2 += num5;
			}
			_blendedSamples.Add(num / num2);
		}
		for (int l = 0; l < _blendedSamples.Count; l++)
		{
			float num6 = _blendedSamples[l];
			float num7 = (float)l * 70f;
			float noise3D2 = _noise.GetNoise3D((float)_time * 0.08f, l * 41, l * 9);
			_points.Add(new Vector2(num7 + 40f * num6, noise3D2 * 15f));
		}
		for (int m = 0; m < _points.Count - 1; m++)
		{
			Control control = _links[m];
			Vector2 vector = _points[m];
			Vector2 vector2 = _points[m + 1];
			control.Position = _xOffset * Vector2.Right + (vector + vector2) * 0.5f - control.Size * 0.5f;
			Vector2 vector3 = vector2 - vector;
			control.Rotation = Mathf.Atan2(vector3.Y, vector3.X);
			float num8 = _noise.GetNoise3D((float)_time * 0.2f, m * 63, m * 13) * 0.5f;
			if (m % 2 == 0)
			{
				control.Scale = new Vector2(1f, 0.8f + num8 * 0.1f);
			}
		}
	}

	private void BreakingUpdate()
	{
		double num = _time - _breakStartTime;
		float num2 = Ease.SineOut(Mathf.Clamp(Mathf.Lerp(0.1f, 1f, (float)_links.Count / 5f), 0f, 1f));
		if (num >= (double)num2)
		{
			_breakCompletedSource?.TrySetResult();
			TaskHelper.RunSafely(BrokenTween());
			_state = State.Broken;
			return;
		}
		float weight = Ease.SineOut((float)num / num2);
		float num3 = Ease.QuadIn(Mathf.Clamp((float)num / 0.7f, 0f, 1f));
		Vector2 vector = -new Vector2((float)_links.Count * 85f, 0f) * 0.5f;
		for (int i = 0; i < _points.Count - 1; i++)
		{
			Control control = _links[i];
			Vector2 vector2 = _points[i];
			Vector2 vector3 = _points[i + 1];
			Vector2 vector4 = _xOffset * Vector2.Right + (vector2 + vector3) * 0.5f - control.Size * 0.5f;
			Vector2 vector5 = (float)i * 85f * Vector2.Right;
			Vector2 vector6 = (float)(i + 1) * 85f * Vector2.Right;
			Vector2 to = vector + (vector5 + vector6) * 0.5f - control.Size * 0.5f;
			Vector2 vector7 = num3 * Vector2.Right.Rotated((float)Math.PI * 2f * Rng.Chaotic.NextFloat()) * 2f;
			Vector2 position = vector4.Lerp(to, weight) + vector7;
			Vector2 vector8 = vector3 - vector2;
			float num4 = Mathf.Atan2(vector8.Y, vector8.X);
			float to2 = 0f;
			control.Position = position;
			control.Rotation = Mathf.Lerp(num4, to2, weight);
		}
	}

	private async Task BrokenTween()
	{
		PackedScene sparkScene = PreloadManager.Cache.GetScene(_breakSparkPath);
		StopWindupSound();
		NDebugAudioManager.Instance.Play("chain_break.wav");
		await _tween.Play((Tween tween) =>
		{
			tween.SetParallel();
			for (int i = 0; i < _links.Count; i++)
			{
				Control control = _links[i];
				float num = ((float)i - (float)_links.Count * 0.5f) * 0.2f;
				Vector2 vector = Vector2.Right.Rotated(Rng.Chaotic.NextFloat((float)Math.PI * 2f));
				vector = (vector + Vector2.Right * num).Normalized();
				Node2D node2D = sparkScene.Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
				this.AddChildSafely(node2D);
				node2D.Position = control.Position;
				node2D.Rotation = Mathf.Atan2(vector.Y, vector.X) + (float)Math.PI / 2f;
				node2D.GetNode<GpuParticles2D>("Vfx").Emitting = true;
				float num2 = Rng.Chaotic.NextFloat((float)Math.PI * 2f);
				Vector2 vector2 = control.Position + vector * Rng.Chaotic.NextFloat(200f, 300f);
				control.GetNode<TextureRect>("TextureRect").Texture = PreloadManager.Cache.GetTexture2D(Rng.Chaotic.NextItem(_brokenLinkPaths));
				tween.TweenProperty(control, "rotation", num2, 1.4).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
				tween.TweenProperty(control, "position", vector2, 1.4).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
				tween.TweenProperty(control, "modulate:a", 0f, 0.3).SetDelay(Rng.Chaotic.NextFloat(0.5f, 0.7f));
			}
		});
		_breakFadeOutCompletedSource?.TrySetResult();
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Initialize, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "count", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.CancelPendingBreak, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.StopWindupSound, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetXOffset, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "xOffset", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.Reset, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.Update, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.BreakingUpdate, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.Initialize && args.Count == 1)
		{
			Initialize(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.CancelPendingBreak && args.Count == 0)
		{
			CancelPendingBreak();
			ret = default;
			return true;
		}
		if (method == MethodName.StopWindupSound && args.Count == 0)
		{
			StopWindupSound();
			ret = default;
			return true;
		}
		if (method == MethodName.SetXOffset && args.Count == 1)
		{
			SetXOffset(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.Reset && args.Count == 0)
		{
			Reset();
			ret = default;
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.Update && args.Count == 0)
		{
			Update();
			ret = default;
			return true;
		}
		if (method == MethodName.BreakingUpdate && args.Count == 0)
		{
			BreakingUpdate();
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
		if (method == MethodName.Initialize)
		{
			return true;
		}
		if (method == MethodName.CancelPendingBreak)
		{
			return true;
		}
		if (method == MethodName.StopWindupSound)
		{
			return true;
		}
		if (method == MethodName.SetXOffset)
		{
			return true;
		}
		if (method == MethodName.Reset)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.Update)
		{
			return true;
		}
		if (method == MethodName.BreakingUpdate)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._originalLinkImage)
		{
			_originalLinkImage = VariantUtils.ConvertTo<Texture2D>(in value);
			return true;
		}
		if (name == PropertyName._blendWeightSum)
		{
			_blendWeightSum = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._time)
		{
			_time = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName._breakStartTime)
		{
			_breakStartTime = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName._noise)
		{
			_noise = VariantUtils.ConvertTo<FastNoiseLite>(in value);
			return true;
		}
		if (name == PropertyName._lowFreqNoise)
		{
			_lowFreqNoise = VariantUtils.ConvertTo<FastNoiseLite>(in value);
			return true;
		}
		if (name == PropertyName._xOffset)
		{
			_xOffset = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._windupSoundId)
		{
			_windupSoundId = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._state)
		{
			_state = VariantUtils.ConvertTo<State>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.IsBreaking)
		{
			value = VariantUtils.CreateFrom<bool>(IsBreaking);
			return true;
		}
		if (name == PropertyName._originalLinkImage)
		{
			value = VariantUtils.CreateFrom(in _originalLinkImage);
			return true;
		}
		if (name == PropertyName._blendWeightSum)
		{
			value = VariantUtils.CreateFrom(in _blendWeightSum);
			return true;
		}
		if (name == PropertyName._time)
		{
			value = VariantUtils.CreateFrom(in _time);
			return true;
		}
		if (name == PropertyName._breakStartTime)
		{
			value = VariantUtils.CreateFrom(in _breakStartTime);
			return true;
		}
		if (name == PropertyName._noise)
		{
			value = VariantUtils.CreateFrom(in _noise);
			return true;
		}
		if (name == PropertyName._lowFreqNoise)
		{
			value = VariantUtils.CreateFrom(in _lowFreqNoise);
			return true;
		}
		if (name == PropertyName._xOffset)
		{
			value = VariantUtils.CreateFrom(in _xOffset);
			return true;
		}
		if (name == PropertyName._windupSoundId)
		{
			value = VariantUtils.CreateFrom(in _windupSoundId);
			return true;
		}
		if (name == PropertyName._state)
		{
			value = VariantUtils.CreateFrom(in _state);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._originalLinkImage, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._blendWeightSum, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._time, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._breakStartTime, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._noise, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._lowFreqNoise, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._xOffset, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._windupSoundId, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._state, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName.IsBreaking, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._originalLinkImage, Variant.From(in _originalLinkImage));
		info.AddProperty(PropertyName._blendWeightSum, Variant.From(in _blendWeightSum));
		info.AddProperty(PropertyName._time, Variant.From(in _time));
		info.AddProperty(PropertyName._breakStartTime, Variant.From(in _breakStartTime));
		info.AddProperty(PropertyName._noise, Variant.From(in _noise));
		info.AddProperty(PropertyName._lowFreqNoise, Variant.From(in _lowFreqNoise));
		info.AddProperty(PropertyName._xOffset, Variant.From(in _xOffset));
		info.AddProperty(PropertyName._windupSoundId, Variant.From(in _windupSoundId));
		info.AddProperty(PropertyName._state, Variant.From(in _state));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._originalLinkImage, out var value))
		{
			_originalLinkImage = value.As<Texture2D>();
		}
		if (info.TryGetProperty(PropertyName._blendWeightSum, out var value2))
		{
			_blendWeightSum = value2.As<float>();
		}
		if (info.TryGetProperty(PropertyName._time, out var value3))
		{
			_time = value3.As<double>();
		}
		if (info.TryGetProperty(PropertyName._breakStartTime, out var value4))
		{
			_breakStartTime = value4.As<double>();
		}
		if (info.TryGetProperty(PropertyName._noise, out var value5))
		{
			_noise = value5.As<FastNoiseLite>();
		}
		if (info.TryGetProperty(PropertyName._lowFreqNoise, out var value6))
		{
			_lowFreqNoise = value6.As<FastNoiseLite>();
		}
		if (info.TryGetProperty(PropertyName._xOffset, out var value7))
		{
			_xOffset = value7.As<float>();
		}
		if (info.TryGetProperty(PropertyName._windupSoundId, out var value8))
		{
			_windupSoundId = value8.As<int>();
		}
		if (info.TryGetProperty(PropertyName._state, out var value9))
		{
			_state = value9.As<State>();
		}
	}
}
