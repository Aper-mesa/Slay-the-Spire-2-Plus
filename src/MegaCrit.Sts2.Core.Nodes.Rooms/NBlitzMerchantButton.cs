using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace MegaCrit.Sts2.Core.Nodes.Rooms;

[ScriptPath("res://src/Core/Nodes/Rooms/NBlitzMerchantButton.cs")]
public class NBlitzMerchantButton : NMerchantButton
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : NMerchantButton.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'Teardown' method.
		/// </summary>
		public static readonly StringName Teardown = "Teardown";

		/// <summary>
		/// Cached name for the 'UpdateTokens' method.
		/// </summary>
		public static readonly StringName UpdateTokens = "UpdateTokens";

		/// <summary>
		/// Cached name for the 'CompleteShop' method.
		/// </summary>
		public static readonly StringName CompleteShop = "CompleteShop";

		/// <summary>
		/// Cached name for the 'ShowGreen' method.
		/// </summary>
		public static readonly StringName ShowGreen = "ShowGreen";

		/// <summary>
		/// Cached name for the 'StartGreenSparks' method.
		/// </summary>
		public static readonly StringName StartGreenSparks = "StartGreenSparks";

		/// <summary>
		/// Cached name for the 'ShowRed' method.
		/// </summary>
		public static readonly StringName ShowRed = "ShowRed";

		/// <summary>
		/// Cached name for the '_ExitTree' method.
		/// </summary>
		public new static readonly StringName _ExitTree = "_ExitTree";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : NMerchantButton.PropertyName
	{
		/// <summary>
		/// Cached name for the '_greenBlast' field.
		/// </summary>
		public static readonly StringName _greenBlast = "_greenBlast";

		/// <summary>
		/// Cached name for the '_greenEmbers' field.
		/// </summary>
		public static readonly StringName _greenEmbers = "_greenEmbers";

		/// <summary>
		/// Cached name for the '_redEmbers' field.
		/// </summary>
		public static readonly StringName _redEmbers = "_redEmbers";

		/// <summary>
		/// Cached name for the '_greenGlow' field.
		/// </summary>
		public static readonly StringName _greenGlow = "_greenGlow";

		/// <summary>
		/// Cached name for the '_redGlow' field.
		/// </summary>
		public static readonly StringName _redGlow = "_redGlow";

		/// <summary>
		/// Cached name for the '_greenFire' field.
		/// </summary>
		public static readonly StringName _greenFire = "_greenFire";

		/// <summary>
		/// Cached name for the '_redFire' field.
		/// </summary>
		public static readonly StringName _redFire = "_redFire";

		/// <summary>
		/// Cached name for the 'TokenSlots' field.
		/// </summary>
		public static readonly StringName TokenSlots = "TokenSlots";

		/// <summary>
		/// Cached name for the '_redFireScale' field.
		/// </summary>
		public static readonly StringName _redFireScale = "_redFireScale";

		/// <summary>
		/// Cached name for the '_greenFireScale' field.
		/// </summary>
		public static readonly StringName _greenFireScale = "_greenFireScale";

		/// <summary>
		/// Cached name for the '_redTween' field.
		/// </summary>
		public static readonly StringName _redTween = "_redTween";

		/// <summary>
		/// Cached name for the '_greenTween' field.
		/// </summary>
		public static readonly StringName _greenTween = "_greenTween";

		/// <summary>
		/// Cached name for the '_hasSwitchedToGreen' field.
		/// </summary>
		public static readonly StringName _hasSwitchedToGreen = "_hasSwitchedToGreen";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NMerchantButton.SignalName
	{
	}

	private MerchantInventory? _inventory;

	private MegaSprite _megaSprite;

	private GpuParticles2D _greenBlast;

	private GpuParticles2D _greenEmbers;

	private GpuParticles2D _redEmbers;

	private GpuParticles2D _greenGlow;

	private GpuParticles2D _redGlow;

	private Sprite2D _greenFire;

	private Sprite2D _redFire;

	private Sprite2D[] TokenSlots;

	private Vector2 _redFireScale;

	private Vector2 _greenFireScale;

	private Tween? _redTween;

	private Tween? _greenTween;

	private bool _hasSwitchedToGreen;

	public override void _Ready()
	{
		ConnectSignals();
		_greenBlast = GetNode<GpuParticles2D>("BackVfx/GreenBlast");
		_greenEmbers = GetNode<GpuParticles2D>("BackVfx/GreenEmbers");
		_redEmbers = GetNode<GpuParticles2D>("BackVfx/RedEmbers");
		_greenGlow = GetNode<GpuParticles2D>("FrontVfx/GreenGlow");
		_redGlow = GetNode<GpuParticles2D>("FrontVfx/RedGlow");
		_greenFire = GetNode<Sprite2D>("BackVfx/GreenFire");
		_redFire = GetNode<Sprite2D>("BackVfx/RedFire");
		_merchantSelectionReticle = GetNode<NSelectionReticle>("%MerchantSelectionReticle");
		_megaSprite = new MegaSprite(GetNode("%MerchantVisual"));
		TokenSlots = new Sprite2D[6];
		for (int i = 0; i < 6; i++)
		{
			TokenSlots[i] = GetNode<Sprite2D>($"%Token{i + 1}");
		}
		this.RunWhenSpineReady(_megaSprite, (MegaAnimationState animState) =>
		{
			_merchantSkeleton = _megaSprite.GetSkeleton();
			animState.SetAnimation("animation");
			if (IsFocused && !_focusedWhileTargeting)
			{
				_merchantSkeleton.SetSkinByName("outline");
				_merchantSkeleton.SetSlotsToSetupPose();
			}
		});
		_redFireScale = _redFire.Scale;
		_greenFireScale = _greenFire.Scale;
		_greenBlast.OneShot = true;
		ShowRed();
	}

	public void Setup(MerchantInventory inventory)
	{
		_inventory = inventory;
		IReadOnlyList<BlitzMerchantEntry> blitzEntries = inventory.BlitzEntries;
		foreach (BlitzMerchantEntry item in blitzEntries)
		{
			item.PurchaseCompleted += UpdateTokens;
		}
		UpdateTokens();
	}

	private void Teardown()
	{
		if (_inventory == null)
		{
			return;
		}
		IReadOnlyList<BlitzMerchantEntry> blitzEntries = _inventory.BlitzEntries;
		foreach (BlitzMerchantEntry item in blitzEntries)
		{
			item.PurchaseCompleted -= UpdateTokens;
		}
		_inventory = null;
	}

	private void UpdateTokens(PurchaseStatus _, MerchantEntry __)
	{
		UpdateTokens();
	}

	private void UpdateTokens()
	{
		if (_inventory != null)
		{
			IReadOnlyList<BlitzMerchantEntry> blitzEntries = _inventory.BlitzEntries;
			for (int i = 0; i < blitzEntries.Count; i++)
			{
				TokenSlots[i].Texture = blitzEntries[i].SmallIcon;
				TokenSlots[i].SetVisible(blitzEntries[i].IsStocked);
			}
		}
	}

	public void CompleteShop()
	{
		if (!_hasSwitchedToGreen)
		{
			_hasSwitchedToGreen = true;
			ShowGreen();
		}
	}

	private void ShowGreen()
	{
		_greenGlow.Restart();
		_greenFire.Visible = true;
		_greenTween = CreateTween();
		_greenTween.TweenInterval(0.5);
		_greenFire.Scale = Vector2.Zero;
		Vector2 vector = new Vector2(1.1f, 1.3f);
		_greenTween.TweenCallback(Callable.From(() =>
		{
			MegaAnimationState animationState = _megaSprite.GetAnimationState();
			animationState.SetAnimation("done", loop: false);
			animationState.AddAnimation("done_loop");
			_redGlow.Restart();
			_redGlow.Emitting = false;
		}));
		_greenTween.TweenProperty(_greenFire, "scale", _greenFireScale * vector, 0.10000000149011612).SetDelay(0.15000000596046448).SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Quad);
		_greenTween.TweenProperty(_greenFire, "scale", _greenFireScale, 0.10000000149011612).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
		_redTween = CreateTween();
		_redTween.TweenInterval(0.5);
		_redTween.TweenProperty(_redFire, "scale", Vector2.Zero, 0.15000000596046448).SetEase(Tween.EaseType.In);
		_redTween.TweenCallback(Callable.From(StartGreenSparks));
	}

	private void StartGreenSparks()
	{
		_redEmbers.Restart();
		_redEmbers.Emitting = false;
		_greenBlast.Restart();
		_greenEmbers.Restart();
	}

	private void ShowRed()
	{
		_redEmbers.Restart();
		_redGlow.Restart();
		_redFire.Visible = true;
		_redFire.Scale = _redFireScale;
		_greenBlast.Restart();
		_greenEmbers.Restart();
		_greenGlow.Restart();
		_greenBlast.Emitting = false;
		_greenEmbers.Emitting = false;
		_greenGlow.Emitting = false;
		_greenFire.Visible = false;
		_greenFire.Scale = _greenFireScale;
		MegaAnimationState animationState = _megaSprite.GetAnimationState();
		animationState.SetAnimation("animation");
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_redTween?.Kill();
		_greenTween?.Kill();
		Teardown();
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Teardown, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateTokens, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.CompleteShop, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ShowGreen, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.StartGreenSparks, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ShowRed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.Teardown && args.Count == 0)
		{
			Teardown();
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateTokens && args.Count == 0)
		{
			UpdateTokens();
			ret = default;
			return true;
		}
		if (method == MethodName.CompleteShop && args.Count == 0)
		{
			CompleteShop();
			ret = default;
			return true;
		}
		if (method == MethodName.ShowGreen && args.Count == 0)
		{
			ShowGreen();
			ret = default;
			return true;
		}
		if (method == MethodName.StartGreenSparks && args.Count == 0)
		{
			StartGreenSparks();
			ret = default;
			return true;
		}
		if (method == MethodName.ShowRed && args.Count == 0)
		{
			ShowRed();
			ret = default;
			return true;
		}
		if (method == MethodName._ExitTree && args.Count == 0)
		{
			_ExitTree();
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
		if (method == MethodName.Teardown)
		{
			return true;
		}
		if (method == MethodName.UpdateTokens)
		{
			return true;
		}
		if (method == MethodName.CompleteShop)
		{
			return true;
		}
		if (method == MethodName.ShowGreen)
		{
			return true;
		}
		if (method == MethodName.StartGreenSparks)
		{
			return true;
		}
		if (method == MethodName.ShowRed)
		{
			return true;
		}
		if (method == MethodName._ExitTree)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._greenBlast)
		{
			_greenBlast = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._greenEmbers)
		{
			_greenEmbers = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._redEmbers)
		{
			_redEmbers = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._greenGlow)
		{
			_greenGlow = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._redGlow)
		{
			_redGlow = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._greenFire)
		{
			_greenFire = VariantUtils.ConvertTo<Sprite2D>(in value);
			return true;
		}
		if (name == PropertyName._redFire)
		{
			_redFire = VariantUtils.ConvertTo<Sprite2D>(in value);
			return true;
		}
		if (name == PropertyName.TokenSlots)
		{
			TokenSlots = VariantUtils.ConvertToSystemArrayOfGodotObject<Sprite2D>(in value);
			return true;
		}
		if (name == PropertyName._redFireScale)
		{
			_redFireScale = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._greenFireScale)
		{
			_greenFireScale = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._redTween)
		{
			_redTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._greenTween)
		{
			_greenTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._hasSwitchedToGreen)
		{
			_hasSwitchedToGreen = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._greenBlast)
		{
			value = VariantUtils.CreateFrom(in _greenBlast);
			return true;
		}
		if (name == PropertyName._greenEmbers)
		{
			value = VariantUtils.CreateFrom(in _greenEmbers);
			return true;
		}
		if (name == PropertyName._redEmbers)
		{
			value = VariantUtils.CreateFrom(in _redEmbers);
			return true;
		}
		if (name == PropertyName._greenGlow)
		{
			value = VariantUtils.CreateFrom(in _greenGlow);
			return true;
		}
		if (name == PropertyName._redGlow)
		{
			value = VariantUtils.CreateFrom(in _redGlow);
			return true;
		}
		if (name == PropertyName._greenFire)
		{
			value = VariantUtils.CreateFrom(in _greenFire);
			return true;
		}
		if (name == PropertyName._redFire)
		{
			value = VariantUtils.CreateFrom(in _redFire);
			return true;
		}
		if (name == PropertyName.TokenSlots)
		{
			GodotObject[] tokenSlots = TokenSlots;
			value = VariantUtils.CreateFromSystemArrayOfGodotObject(tokenSlots);
			return true;
		}
		if (name == PropertyName._redFireScale)
		{
			value = VariantUtils.CreateFrom(in _redFireScale);
			return true;
		}
		if (name == PropertyName._greenFireScale)
		{
			value = VariantUtils.CreateFrom(in _greenFireScale);
			return true;
		}
		if (name == PropertyName._redTween)
		{
			value = VariantUtils.CreateFrom(in _redTween);
			return true;
		}
		if (name == PropertyName._greenTween)
		{
			value = VariantUtils.CreateFrom(in _greenTween);
			return true;
		}
		if (name == PropertyName._hasSwitchedToGreen)
		{
			value = VariantUtils.CreateFrom(in _hasSwitchedToGreen);
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
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._greenBlast, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._greenEmbers, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._redEmbers, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._greenGlow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._redGlow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._greenFire, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._redFire, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Array, PropertyName.TokenSlots, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._redFireScale, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._greenFireScale, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._redTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._greenTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._hasSwitchedToGreen, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._greenBlast, Variant.From(in _greenBlast));
		info.AddProperty(PropertyName._greenEmbers, Variant.From(in _greenEmbers));
		info.AddProperty(PropertyName._redEmbers, Variant.From(in _redEmbers));
		info.AddProperty(PropertyName._greenGlow, Variant.From(in _greenGlow));
		info.AddProperty(PropertyName._redGlow, Variant.From(in _redGlow));
		info.AddProperty(PropertyName._greenFire, Variant.From(in _greenFire));
		info.AddProperty(PropertyName._redFire, Variant.From(in _redFire));
		StringName tokenSlots = PropertyName.TokenSlots;
		GodotObject[] tokenSlots2 = TokenSlots;
		info.AddProperty(tokenSlots, Variant.CreateFrom(tokenSlots2));
		info.AddProperty(PropertyName._redFireScale, Variant.From(in _redFireScale));
		info.AddProperty(PropertyName._greenFireScale, Variant.From(in _greenFireScale));
		info.AddProperty(PropertyName._redTween, Variant.From(in _redTween));
		info.AddProperty(PropertyName._greenTween, Variant.From(in _greenTween));
		info.AddProperty(PropertyName._hasSwitchedToGreen, Variant.From(in _hasSwitchedToGreen));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._greenBlast, out var value))
		{
			_greenBlast = value.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._greenEmbers, out var value2))
		{
			_greenEmbers = value2.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._redEmbers, out var value3))
		{
			_redEmbers = value3.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._greenGlow, out var value4))
		{
			_greenGlow = value4.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._redGlow, out var value5))
		{
			_redGlow = value5.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._greenFire, out var value6))
		{
			_greenFire = value6.As<Sprite2D>();
		}
		if (info.TryGetProperty(PropertyName._redFire, out var value7))
		{
			_redFire = value7.As<Sprite2D>();
		}
		if (info.TryGetProperty(PropertyName.TokenSlots, out var value8))
		{
			TokenSlots = value8.AsGodotObjectArray<Sprite2D>();
		}
		if (info.TryGetProperty(PropertyName._redFireScale, out var value9))
		{
			_redFireScale = value9.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._greenFireScale, out var value10))
		{
			_greenFireScale = value10.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._redTween, out var value11))
		{
			_redTween = value11.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._greenTween, out var value12))
		{
			_greenTween = value12.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._hasSwitchedToGreen, out var value13))
		{
			_hasSwitchedToGreen = value13.As<bool>();
		}
	}
}
