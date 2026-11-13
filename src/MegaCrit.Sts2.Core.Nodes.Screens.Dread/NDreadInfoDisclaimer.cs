using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Dread;

/// <summary>
/// Disclaimer which explains how Dread works.
/// Appears the first time you enter the Dread mode screen but can also be
/// accessed by selecting the "?" button in the top right corner.
/// </summary>
[ScriptPath("res://src/Core/Nodes/Screens/Dread/NDreadInfoDisclaimer.cs")]
public class NDreadInfoDisclaimer : Control, IScreenContext
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : Control.MethodName
	{
		/// <summary>
		/// Cached name for the 'Create' method.
		/// </summary>
		public static readonly StringName Create = "Create";

		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'OnConfirmPressed' method.
		/// </summary>
		public static readonly StringName OnConfirmPressed = "OnConfirmPressed";

		/// <summary>
		/// Cached name for the 'AnimIn' method.
		/// </summary>
		public static readonly StringName AnimIn = "AnimIn";

		/// <summary>
		/// Cached name for the 'UpdateDisclaimerDescription' method.
		/// </summary>
		public static readonly StringName UpdateDisclaimerDescription = "UpdateDisclaimerDescription";
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
		/// Cached name for the '_openTween' field.
		/// </summary>
		public static readonly StringName _openTween = "_openTween";

		/// <summary>
		/// Cached name for the '_image' field.
		/// </summary>
		public static readonly StringName _image = "_image";

		/// <summary>
		/// Cached name for the '_bgRect' field.
		/// </summary>
		public static readonly StringName _bgRect = "_bgRect";

		/// <summary>
		/// Cached name for the '_popup' field.
		/// </summary>
		public static readonly StringName _popup = "_popup";

		/// <summary>
		/// Cached name for the '_confirmButton' field.
		/// </summary>
		public static readonly StringName _confirmButton = "_confirmButton";

		/// <summary>
		/// Cached name for the '_isDelayedOpen' field.
		/// </summary>
		public static readonly StringName _isDelayedOpen = "_isDelayedOpen";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
	}

	public const string id = "dread_disclaimer";

	private const string _disclaimerScenePath = "screens/dread/dread_info_disclaimer";

	private Tween? _openTween;

	private readonly OwnedTween _fadeOut;

	private Control _image;

	private Control _bgRect;

	private Control _popup;

	private NButton _confirmButton;

	private bool _isDelayedOpen;

	public Control? DefaultFocusedControl => null;

	public NDreadInfoDisclaimer()
	{
		_fadeOut = new OwnedTween(this);
	}

	/// <summary>
	/// Created as needed (not preloaded) because it shows up rarely.
	/// </summary>
	public static NDreadInfoDisclaimer Create(bool isDelayedOpen)
	{
		NDreadInfoDisclaimer nDreadInfoDisclaimer = SceneHelper.Instantiate<NDreadInfoDisclaimer>("screens/dread/dread_info_disclaimer");
		NHotkeyManager.Instance?.AddBlockingScreen(nDreadInfoDisclaimer);
		nDreadInfoDisclaimer._isDelayedOpen = isDelayedOpen;
		return nDreadInfoDisclaimer;
	}

	public override void _Ready()
	{
		string formattedText = new LocString("main_menu_ui", "DREAD_DISCLAIMER.title").GetFormattedText();
		_confirmButton = GetNode<NDreadDisclaimerConfirmButton>("%ConfirmButton");
		_confirmButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnConfirmPressed));
		GetNode<MegaLabel>("%Header").SetTextAutoSize(formattedText);
		_image = GetNode<TextureRect>("Image");
		UpdateDisclaimerDescription();
		AnimIn();
	}

	private void OnConfirmPressed(NButton obj)
	{
		TaskHelper.RunSafely(CloseScreen());
	}

	private void AnimIn()
	{
		_bgRect = GetNode<Control>("%Bg");
		Control bgRect = _bgRect;
		Color modulate = _bgRect.Modulate;
		modulate.A = (_isDelayedOpen ? 0.8f : 0f);
		bgRect.Modulate = modulate;
		_popup = GetNode<Control>("%Image");
		Control popup = _popup;
		modulate = _popup.Modulate;
		modulate.A = 0f;
		popup.Modulate = modulate;
		_openTween?.Kill();
		_openTween = CreateTween().SetParallel();
		if (_isDelayedOpen)
		{
			_openTween.Chain();
			_openTween.TweenInterval(0.8);
			_openTween.Chain();
		}
		_openTween.TweenProperty(_bgRect, "modulate:a", 0.8f, 0.25);
		_openTween.TweenProperty(_popup, "modulate:a", 1f, 0.5);
		_openTween.TweenProperty(_popup, "position:y", _popup.Position.Y, 0.5).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back)
			.From(_popup.Position.Y + 100f);
		double num = 1.0;
		int num2 = 1;
		foreach (MegaRichTextLabel item in GetNode<Control>("%LeftList").GetChildren().OfType<MegaRichTextLabel>())
		{
			string rawText = new LocString("ascension", $"LEVEL_{num2:D2}.title").GetRawText();
			string formattedText = new LocString("ascension", $"LEVEL_{num2:D2}.description").GetFormattedText();
			item.SetTextAutoSize("[gold]" + rawText + "[/gold]\n" + formattedText);
			modulate = item.Modulate;
			modulate.A = 0f;
			item.Modulate = modulate;
			_openTween.TweenProperty(item, "modulate:a", 1f, 0.5).SetDelay(num);
			num += 0.2;
			num2++;
		}
		num += 0.5;
		foreach (MegaRichTextLabel item2 in GetNode<Control>("%RightList").GetChildren().OfType<MegaRichTextLabel>())
		{
			string rawText2 = new LocString("ascension", $"LEVEL_{num2:D2}.title").GetRawText();
			string formattedText2 = new LocString("ascension", $"LEVEL_{num2:D2}.description").GetFormattedText();
			item2.SetTextAutoSize("[gold]" + rawText2 + "[/gold]\n" + formattedText2);
			modulate = item2.Modulate;
			modulate.A = 0f;
			item2.Modulate = modulate;
			_openTween.TweenProperty(item2, "modulate:a", 1f, 0.5).SetDelay(num);
			num += 0.2;
			num2++;
		}
	}

	private async Task CloseScreen()
	{
		SaveManager.Instance.MarkFtueAsComplete("dread_disclaimer");
		_openTween?.Kill();
		if (await _fadeOut.Play((Tween tween) =>
		{
			tween.TweenProperty(this, "modulate:a", 0f, 0.2);
		}) != TweenOutcome.Replaced)
		{
			this.QueueFreeSafely();
			NHotkeyManager.Instance?.RemoveBlockingScreen(this);
		}
	}

	private void UpdateDisclaimerDescription()
	{
		LocString locString = new LocString("main_menu_ui", "DREAD_DISCLAIMER.description");
		locString.Add("rule1", new LocString("ascension", "DREAD_RULE_01").GetRawText());
		locString.Add("rule2", new LocString("ascension", "DREAD_RULE_02").GetRawText());
		locString.Add("rule3", new LocString("ascension", "DREAD_RULE_03").GetRawText());
		locString.Add("rule4", new LocString("ascension", "DREAD_RULE_04").GetRawText());
		GetNode<MegaRichTextLabel>("%Description").SetTextAutoSize(locString.GetFormattedText());
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "isDelayedOpen", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnConfirmPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "obj", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateDisclaimerDescription, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 1)
		{
			ret = VariantUtils.CreateFrom<NDreadInfoDisclaimer>(Create(VariantUtils.ConvertTo<bool>(in args[0])));
			return true;
		}
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default;
			return true;
		}
		if (method == MethodName.OnConfirmPressed && args.Count == 1)
		{
			OnConfirmPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.AnimIn && args.Count == 0)
		{
			AnimIn();
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateDisclaimerDescription && args.Count == 0)
		{
			UpdateDisclaimerDescription();
			ret = default;
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 1)
		{
			ret = VariantUtils.CreateFrom<NDreadInfoDisclaimer>(Create(VariantUtils.ConvertTo<bool>(in args[0])));
			return true;
		}
		ret = default;
		return false;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.Create)
		{
			return true;
		}
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.OnConfirmPressed)
		{
			return true;
		}
		if (method == MethodName.AnimIn)
		{
			return true;
		}
		if (method == MethodName.UpdateDisclaimerDescription)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._openTween)
		{
			_openTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._image)
		{
			_image = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._bgRect)
		{
			_bgRect = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._popup)
		{
			_popup = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._confirmButton)
		{
			_confirmButton = VariantUtils.ConvertTo<NButton>(in value);
			return true;
		}
		if (name == PropertyName._isDelayedOpen)
		{
			_isDelayedOpen = VariantUtils.ConvertTo<bool>(in value);
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
		if (name == PropertyName._openTween)
		{
			value = VariantUtils.CreateFrom(in _openTween);
			return true;
		}
		if (name == PropertyName._image)
		{
			value = VariantUtils.CreateFrom(in _image);
			return true;
		}
		if (name == PropertyName._bgRect)
		{
			value = VariantUtils.CreateFrom(in _bgRect);
			return true;
		}
		if (name == PropertyName._popup)
		{
			value = VariantUtils.CreateFrom(in _popup);
			return true;
		}
		if (name == PropertyName._confirmButton)
		{
			value = VariantUtils.CreateFrom(in _confirmButton);
			return true;
		}
		if (name == PropertyName._isDelayedOpen)
		{
			value = VariantUtils.CreateFrom(in _isDelayedOpen);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._openTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._image, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._bgRect, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._popup, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._confirmButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._isDelayedOpen, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.DefaultFocusedControl, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._openTween, Variant.From(in _openTween));
		info.AddProperty(PropertyName._image, Variant.From(in _image));
		info.AddProperty(PropertyName._bgRect, Variant.From(in _bgRect));
		info.AddProperty(PropertyName._popup, Variant.From(in _popup));
		info.AddProperty(PropertyName._confirmButton, Variant.From(in _confirmButton));
		info.AddProperty(PropertyName._isDelayedOpen, Variant.From(in _isDelayedOpen));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._openTween, out var value))
		{
			_openTween = value.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._image, out var value2))
		{
			_image = value2.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._bgRect, out var value3))
		{
			_bgRect = value3.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._popup, out var value4))
		{
			_popup = value4.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._confirmButton, out var value5))
		{
			_confirmButton = value5.As<NButton>();
		}
		if (info.TryGetProperty(PropertyName._isDelayedOpen, out var value6))
		{
			_isDelayedOpen = value6.As<bool>();
		}
	}
}
