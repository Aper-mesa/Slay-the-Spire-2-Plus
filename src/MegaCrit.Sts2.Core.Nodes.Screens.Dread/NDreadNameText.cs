using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.RichTextTags;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Dread;

[ScriptPath("res://src/Core/Nodes/Screens/Dread/NDreadNameText.cs")]
public class NDreadNameText : Control
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
		/// Cached name for the 'GetTextCenterGlyphIndex' method.
		/// </summary>
		public static readonly StringName GetTextCenterGlyphIndex = "GetTextCenterGlyphIndex";

		/// <summary>
		/// Cached name for the 'UpdateTransform' method.
		/// </summary>
		public static readonly StringName UpdateTransform = "UpdateTransform";

		/// <summary>
		/// Cached name for the 'UpdateGlyphSpace' method.
		/// </summary>
		public static readonly StringName UpdateGlyphSpace = "UpdateGlyphSpace";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Control.PropertyName
	{
		/// <summary>
		/// Cached name for the '_nameLabel' field.
		/// </summary>
		public static readonly StringName _nameLabel = "_nameLabel";

		/// <summary>
		/// Cached name for the '_shadowLabel' field.
		/// </summary>
		public static readonly StringName _shadowLabel = "_shadowLabel";

		/// <summary>
		/// Cached name for the '_nameEffect' field.
		/// </summary>
		public static readonly StringName _nameEffect = "_nameEffect";

		/// <summary>
		/// Cached name for the '_tween' field.
		/// </summary>
		public static readonly StringName _tween = "_tween";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
	}

	private MegaRichTextLabel _nameLabel;

	private MegaRichTextLabel _shadowLabel;

	private RichTextAncientBanner _nameEffect;

	private Tween? _tween;

	public override void _Ready()
	{
		_nameLabel = GetNode<MegaRichTextLabel>("%Name");
		_shadowLabel = GetNode<MegaRichTextLabel>("%Shadow");
		_nameEffect = new RichTextAncientBanner();
		_nameLabel.InstallEffect(_nameEffect);
		_shadowLabel.InstallEffect(_nameEffect);
	}

	public void SetNameText(CharacterModel characterModel)
	{
		SetNameText(characterModel.ShortTitle, characterModel.DreadNameColor);
	}

	public void SetNameText(LocString locString, Color color)
	{
		string formattedText = locString.GetFormattedText();
		string text = formattedText.ToUpper();
		_nameEffect.CenterCharacter = GetTextCenterGlyphIndex(text, _nameLabel.GetThemeFont(ThemeConstants.RichTextLabel.NormalFont, "RichTextLabel"), _nameLabel.GetThemeFontSize(ThemeConstants.RichTextLabel.NormalFontSize, "RichTextLabel"));
		_nameLabel.SelfModulate = color;
		_nameLabel.Text = "[ancient_banner]" + text + "[/ancient_banner]";
		_shadowLabel.Text = _nameLabel.Text;
		TaskHelper.RunSafely(AnimateVfx());
	}

	/// <summary>
	/// Copy of what's seen in NAncientNameBanner.cs
	/// </summary>
	private float GetTextCenterGlyphIndex(string text, Font font, int fontSize)
	{
		using TextParagraph textParagraph = new TextParagraph();
		textParagraph.AddString(text, font, fontSize);
		TextServer primaryInterface = TextServerManager.Singleton.GetPrimaryInterface();
		Array<Dictionary> array = primaryInterface.ShapedTextGetGlyphs(textParagraph.GetLineRid(0));
		float num = array.Sum((Dictionary glyph) => glyph.GetValueOrDefault("advance").AsSingle());
		float num2 = 0f;
		int num3 = 0;
		foreach (Dictionary item in array)
		{
			float num4 = item.GetValueOrDefault("advance").AsSingle();
			num2 += num4;
			if (num2 > num * 0.5f)
			{
				return (float)num3 + (num * 0.5f - (num2 - num4)) / num4;
			}
			num3++;
		}
		return 0f;
	}

	private async Task AnimateVfx()
	{
		UpdateGlyphSpace(0f);
		UpdateTransform(1f);
		_tween?.Kill();
		_tween = CreateTween().SetParallel();
		_tween.TweenMethod(Callable.From<float>(UpdateGlyphSpace), 0f, 1f, 1.0).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
		if (await _tween.AwaitFinished(this) == TweenOutcome.Finished)
		{
			this.IsValid();
		}
	}

	private void UpdateTransform(float obj)
	{
		_nameEffect.Rotation = obj;
	}

	private void UpdateGlyphSpace(float spacing)
	{
		_nameEffect.Spacing = 40f + spacing * 5f;
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.GetTextCenterGlyphIndex, new PropertyInfo(Variant.Type.Float, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
			new PropertyInfo(Variant.Type.Object, "font", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Font"), exported: false),
			new PropertyInfo(Variant.Type.Int, "fontSize", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.UpdateTransform, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "obj", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.UpdateGlyphSpace, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "spacing", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
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
		if (method == MethodName.GetTextCenterGlyphIndex && args.Count == 3)
		{
			ret = VariantUtils.CreateFrom<float>(GetTextCenterGlyphIndex(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<Font>(in args[1]), VariantUtils.ConvertTo<int>(in args[2])));
			return true;
		}
		if (method == MethodName.UpdateTransform && args.Count == 1)
		{
			UpdateTransform(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateGlyphSpace && args.Count == 1)
		{
			UpdateGlyphSpace(VariantUtils.ConvertTo<float>(in args[0]));
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
		if (method == MethodName.GetTextCenterGlyphIndex)
		{
			return true;
		}
		if (method == MethodName.UpdateTransform)
		{
			return true;
		}
		if (method == MethodName.UpdateGlyphSpace)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._nameLabel)
		{
			_nameLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._shadowLabel)
		{
			_shadowLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._nameEffect)
		{
			_nameEffect = VariantUtils.ConvertTo<RichTextAncientBanner>(in value);
			return true;
		}
		if (name == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._nameLabel)
		{
			value = VariantUtils.CreateFrom(in _nameLabel);
			return true;
		}
		if (name == PropertyName._shadowLabel)
		{
			value = VariantUtils.CreateFrom(in _shadowLabel);
			return true;
		}
		if (name == PropertyName._nameEffect)
		{
			value = VariantUtils.CreateFrom(in _nameEffect);
			return true;
		}
		if (name == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom(in _tween);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._nameLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._shadowLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._nameEffect, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._tween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._nameLabel, Variant.From(in _nameLabel));
		info.AddProperty(PropertyName._shadowLabel, Variant.From(in _shadowLabel));
		info.AddProperty(PropertyName._nameEffect, Variant.From(in _nameEffect));
		info.AddProperty(PropertyName._tween, Variant.From(in _tween));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._nameLabel, out var value))
		{
			_nameLabel = value.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._shadowLabel, out var value2))
		{
			_shadowLabel = value2.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._nameEffect, out var value3))
		{
			_nameEffect = value3.As<RichTextAncientBanner>();
		}
		if (info.TryGetProperty(PropertyName._tween, out var value4))
		{
			_tween = value4.As<Tween>();
		}
	}
}
