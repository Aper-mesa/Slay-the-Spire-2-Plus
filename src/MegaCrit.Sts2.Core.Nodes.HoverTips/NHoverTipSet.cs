using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.HoverTips;

[ScriptPath("res://src/Core/Nodes/HoverTips/NHoverTipSet.cs")]
public class NHoverTipSet : Control
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : Control.MethodName
	{
		/// <summary>
		/// Cached name for the 'SetFollowOwner' method.
		/// </summary>
		public static readonly StringName SetFollowOwner = "SetFollowOwner";

		/// <summary>
		/// Cached name for the 'ReplaceExistingTip' method.
		/// </summary>
		public static readonly StringName ReplaceExistingTip = "ReplaceExistingTip";

		/// <summary>
		/// Cached name for the 'ConnectTreeExiting' method.
		/// </summary>
		public static readonly StringName ConnectTreeExiting = "ConnectTreeExiting";

		/// <summary>
		/// Cached name for the 'CreateAndShowMapPointHistory' method.
		/// </summary>
		public static readonly StringName CreateAndShowMapPointHistory = "CreateAndShowMapPointHistory";

		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the '_Process' method.
		/// </summary>
		public new static readonly StringName _Process = "_Process";

		/// <summary>
		/// Cached name for the 'SetAlignment' method.
		/// </summary>
		public static readonly StringName SetAlignment = "SetAlignment";

		/// <summary>
		/// Cached name for the 'SetAlignmentForRelic' method.
		/// </summary>
		public static readonly StringName SetAlignmentForRelic = "SetAlignmentForRelic";

		/// <summary>
		/// Cached name for the 'SetAlignmentForCardHolder' method.
		/// </summary>
		public static readonly StringName SetAlignmentForCardHolder = "SetAlignmentForCardHolder";

		/// <summary>
		/// Cached name for the 'CorrectVerticalOverflow' method.
		/// </summary>
		public static readonly StringName CorrectVerticalOverflow = "CorrectVerticalOverflow";

		/// <summary>
		/// Cached name for the 'CorrectHorizontalOverflow' method.
		/// </summary>
		public static readonly StringName CorrectHorizontalOverflow = "CorrectHorizontalOverflow";

		/// <summary>
		/// Cached name for the 'Clear' method.
		/// </summary>
		public static readonly StringName Clear = "Clear";

		/// <summary>
		/// Cached name for the 'Remove' method.
		/// </summary>
		public static readonly StringName Remove = "Remove";

		/// <summary>
		/// Cached name for the 'SetExtraFollowOffset' method.
		/// </summary>
		public static readonly StringName SetExtraFollowOffset = "SetExtraFollowOffset";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Control.PropertyName
	{
		/// <summary>
		/// Cached name for the 'TextHoverTipDimensions' property.
		/// </summary>
		public static readonly StringName TextHoverTipDimensions = "TextHoverTipDimensions";

		/// <summary>
		/// Cached name for the 'CardHoverTipDimensions' property.
		/// </summary>
		public static readonly StringName CardHoverTipDimensions = "CardHoverTipDimensions";

		/// <summary>
		/// Cached name for the '_textHoverTipContainer' field.
		/// </summary>
		public static readonly StringName _textHoverTipContainer = "_textHoverTipContainer";

		/// <summary>
		/// Cached name for the '_cardHoverTipContainer' field.
		/// </summary>
		public static readonly StringName _cardHoverTipContainer = "_cardHoverTipContainer";

		/// <summary>
		/// Cached name for the '_owner' field.
		/// </summary>
		public static readonly StringName _owner = "_owner";

		/// <summary>
		/// Cached name for the '_followOwner' field.
		/// </summary>
		public static readonly StringName _followOwner = "_followOwner";

		/// <summary>
		/// Cached name for the '_followOffset' field.
		/// </summary>
		public static readonly StringName _followOffset = "_followOffset";

		/// <summary>
		/// Cached name for the '_extraOffset' field.
		/// </summary>
		public static readonly StringName _extraOffset = "_extraOffset";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
	}

	/// <summary>
	/// Blocks hover tips from displaying. Useful for situations such as while you are using card/potion targeting
	/// WARNING: Be very careful when modifying this!!! Make sure that if you set it to true, that you set it back to false!
	/// </summary>
	public static bool shouldBlockHoverTips = false;

	private static readonly StringName _cardHoverTipContainerStr = new StringName("cardHoverTipContainer");

	private static readonly StringName _textHoverTipContainerStr = new StringName("textHoverTipContainer");

	private const float _hoverTipSpacing = 5f;

	public const float hoverTipWidth = 360f;

	private const string _tipScenePath = "res://scenes/ui/hover_tip.tscn";

	private const string _tipSetScenePath = "res://scenes/ui/hover_tip_set.tscn";

	private const string _debuffMatPath = "res://materials/ui/hover_tip_debuff.tres";

	/// <summary>
	/// A replacement drops the tip entry and keeps the handler entry, which is how
	/// <see cref="M:MegaCrit.Sts2.Core.Nodes.HoverTips.NHoverTipSet.ConnectTreeExiting(Godot.Control)" /> knows the owner is already connected. So a handler with no tip is
	/// ordinary; a tip with no handler is not, and every path that adds to the first must call
	/// ConnectTreeExiting.
	/// </summary>
	private static readonly Dictionary<Control, NHoverTipSet> _activeHoverTips = new Dictionary<Control, NHoverTipSet>();

	private static readonly Dictionary<Control, Callable> _treeExitingHandlers = new Dictionary<Control, Callable>();

	private VFlowContainer _textHoverTipContainer;

	private NHoverTipCardContainer _cardHoverTipContainer;

	private Control _owner;

	private bool _followOwner;

	private Vector2 _followOffset;

	private Vector2 _extraOffset = Vector2.Zero;

	/// <summary>
	/// Tests have no <see cref="T:MegaCrit.Sts2.Core.Nodes.NGame" />, so they supply their own container. Null in the running game.
	/// </summary>
	internal static Node? ContainerForTests { get; set; }

	private static Node HoverTipsContainer => ContainerForTests ?? NGame.Instance.HoverTipsContainer;

	private Vector2 TextHoverTipDimensions => _textHoverTipContainer.Size;

	private Vector2 CardHoverTipDimensions => _cardHoverTipContainer.Size;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlyArray<string>(new string[3] { "res://scenes/ui/hover_tip.tscn", "res://scenes/ui/hover_tip_set.tscn", "res://materials/ui/hover_tip_debuff.tres" });

	/// <summary>
	/// Allows the HoverTip to follow the node it's tied to.
	/// </summary>
	public void SetFollowOwner()
	{
		_followOwner = true;
		_followOffset = _owner.GlobalPosition - GlobalPosition;
	}

	public static NHoverTipSet? CreateAndShow(Control owner, IHoverTip hoverTip, HoverTipAlignment alignment = HoverTipAlignment.None)
	{
		return CreateAndShow(owner, new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(hoverTip), alignment);
	}

	public static NHoverTipSet? CreateAndShow(Control owner, IEnumerable<IHoverTip> hoverTips, HoverTipAlignment alignment = HoverTipAlignment.None)
	{
		NHoverTipSet nHoverTipSet = CreateAndRegister(owner, hoverTips);
		nHoverTipSet?.SetAlignment(owner, alignment);
		return nHoverTipSet;
	}

	/// <summary>
	/// Builds the tip node and takes the owner's slot in <see cref="F:MegaCrit.Sts2.Core.Nodes.HoverTips.NHoverTipSet._activeHoverTips" />. Split out from
	/// CreateAndShow because aligning needs a live <see cref="T:MegaCrit.Sts2.Core.Nodes.NGame" /> and this does not.
	/// </summary>
	internal static NHoverTipSet? CreateAndRegister(Control owner, IEnumerable<IHoverTip> hoverTips)
	{
		if (shouldBlockHoverTips)
		{
			return null;
		}
		ReplaceExistingTip(owner);
		NHoverTipSet nHoverTipSet = PreloadManager.Cache.GetScene("res://scenes/ui/hover_tip_set.tscn").Instantiate<NHoverTipSet>(PackedScene.GenEditState.Disabled);
		HoverTipsContainer.AddChildSafely(nHoverTipSet);
		_activeHoverTips.Add(owner, nHoverTipSet);
		nHoverTipSet.Init(owner, hoverTips);
		if (NGame.IsDebugHidingHoverTips)
		{
			nHoverTipSet.Visible = false;
		}
		ConnectTreeExiting(owner);
		return nHoverTipSet;
	}

	/// <summary>
	/// Frees the tip an owner is already showing, if any, so the new one can take its place. A control may only
	/// have one tip, and callers reach this from both the mouse and the focus path, so the previous one is not
	/// always cleared first.
	/// </summary>
	private static void ReplaceExistingTip(Control owner)
	{
		if (_activeHoverTips.Remove(owner, out NHoverTipSet value))
		{
			value.QueueFreeSafely();
		}
	}

	/// <summary>
	/// The handler is stored so <see cref="M:MegaCrit.Sts2.Core.Nodes.HoverTips.NHoverTipSet.Remove(Godot.Control)" /> can disconnect it again, and is trusted only while the
	/// owner still reports it connected, so a disconnect from elsewhere costs a reconnect rather than leaving
	/// the owner with no handler at all.
	/// </summary>
	private static void ConnectTreeExiting(Control owner)
	{
		if (!_treeExitingHandlers.TryGetValue(owner, out var value) || !owner.IsConnected(Node.SignalName.TreeExiting, value))
		{
			Callable callable = Callable.From(() =>
			{
				Remove(owner);
			});
			_treeExitingHandlers[owner] = callable;
			owner.Connect(Node.SignalName.TreeExiting, callable);
		}
	}

	public static NHoverTipSet CreateAndShowMapPointHistory(Control owner, NMapPointHistoryHoverTip historyHoverTip)
	{
		ReplaceExistingTip(owner);
		NHoverTipSet nHoverTipSet = PreloadManager.Cache.GetScene("res://scenes/ui/hover_tip_set.tscn").Instantiate<NHoverTipSet>(PackedScene.GenEditState.Disabled);
		nHoverTipSet._owner = owner;
		HoverTipsContainer.AddChildSafely(nHoverTipSet);
		_activeHoverTips.Add(owner, nHoverTipSet);
		nHoverTipSet._textHoverTipContainer.AddChildSafely(historyHoverTip);
		historyHoverTip.ApplyRtlLayout();
		if (NGame.IsDebugHidingHoverTips)
		{
			nHoverTipSet.Visible = false;
		}
		ConnectTreeExiting(owner);
		return nHoverTipSet;
	}

	public override void _Ready()
	{
		_textHoverTipContainer = new VFlowContainer();
		_textHoverTipContainer.Name = _textHoverTipContainerStr;
		_textHoverTipContainer.MouseFilter = MouseFilterEnum.Ignore;
		this.AddChildSafely(_textHoverTipContainer);
		_cardHoverTipContainer = new NHoverTipCardContainer();
		_cardHoverTipContainer.Name = _cardHoverTipContainerStr;
		_cardHoverTipContainer.MouseFilter = MouseFilterEnum.Ignore;
		this.AddChildSafely(_cardHoverTipContainer);
	}

	public override void _Process(double delta)
	{
		if (_followOwner && _owner != null)
		{
			GlobalPosition = _owner.GlobalPosition - _followOffset + _extraOffset;
		}
	}

	private void Init(Control owner, IEnumerable<IHoverTip> hoverTips)
	{
		_owner = owner;
		foreach (IHoverTip item in IHoverTip.RemoveDupes(hoverTips))
		{
			if (item is HoverTip hoverTip)
			{
				Control control = PreloadManager.Cache.GetScene("res://scenes/ui/hover_tip.tscn").Instantiate<Control>(PackedScene.GenEditState.Disabled);
				_textHoverTipContainer.AddChildSafely(control);
				control.ApplyRtlLayout();
				MegaLabel node = control.GetNode<MegaLabel>("%Title");
				if (hoverTip.Title == null)
				{
					node.Visible = false;
				}
				else
				{
					node.SetTextAutoSize(hoverTip.Title);
				}
				control.GetNode<MegaRichTextLabel>("%Description").Text = hoverTip.Description;
				control.GetNode<MegaRichTextLabel>("%Description").AutowrapMode = (TextServer.AutowrapMode)(hoverTip.ShouldOverrideTextOverflow ? 0 : 3);
				control.GetNode<TextureRect>("%Icon").Texture = hoverTip.Icon;
				if (hoverTip.IsDebuff)
				{
					control.GetNode<CanvasItem>("%Bg").Material = PreloadManager.Cache.GetMaterial("res://materials/ui/hover_tip_debuff.tres");
				}
				control.ResetSize();
				if (_textHoverTipContainer.Size.Y + control.Size.Y + 5f < NGame.Instance.GetViewportRect().Size.Y - 50f)
				{
					_textHoverTipContainer.Size = new Vector2(360f, _textHoverTipContainer.Size.Y + control.Size.Y + 5f);
				}
				else
				{
					_textHoverTipContainer.Alignment = FlowContainer.AlignmentMode.Center;
				}
			}
			else
			{
				_cardHoverTipContainer.Add((CardHoverTip)item);
			}
			AbstractModel canonicalModel = item.CanonicalModel;
			if (!(canonicalModel is CardModel card))
			{
				if (!(canonicalModel is RelicModel relic))
				{
					if (canonicalModel is PotionModel potion)
					{
						SaveManager.Instance.MarkPotionAsSeen(potion);
					}
				}
				else
				{
					SaveManager.Instance.MarkRelicAsSeen(relic);
				}
			}
			else
			{
				SaveManager.Instance.MarkCardAsSeen(card);
			}
		}
	}

	/// <summary>
	/// Sets the position of the hover tip relative to the given node based on the alignment set.
	/// </summary>
	/// <param name="node">The control node that we are aligning the hover tip to</param>.
	/// <param name="alignment">Dictates the position of the hover tip relative to the given node.
	/// ie: top left side of the node, Top right side of the node</param>
	public void SetAlignment(Control node, HoverTipAlignment alignment)
	{
		switch (alignment)
		{
		case HoverTipAlignment.Center:
			_textHoverTipContainer.Position = Vector2.Zero;
			GlobalPosition = node.GlobalPosition + Vector2.Down * node.Size.Y * 1.5f;
			_cardHoverTipContainer.GlobalPosition += Vector2.Down * _textHoverTipContainer.Size.Y;
			_cardHoverTipContainer.LayoutResizeAndReposition(_cardHoverTipContainer.GlobalPosition, alignment);
			break;
		default:
			SetAlignment(new Rect2(node.GlobalPosition, node.Size * node.Scale), alignment);
			return;
		case HoverTipAlignment.None:
			break;
		}
		CorrectVerticalOverflow();
		CorrectHorizontalOverflow();
	}

	/// <summary>
	/// Left/Right alignment against an arbitrary global rect. Useful when the target is mid-tween and
	/// we want to align to where it will end up rather than where it currently is.
	/// </summary>
	public void SetAlignment(Rect2 globalRect, HoverTipAlignment alignment)
	{
		_textHoverTipContainer.Position = Vector2.Zero;
		switch (alignment)
		{
		case HoverTipAlignment.Left:
			_textHoverTipContainer.GlobalPosition = globalRect.Position;
			_textHoverTipContainer.Position += Vector2.Left * _textHoverTipContainer.Size.X;
			_textHoverTipContainer.ReverseFill = true;
			_cardHoverTipContainer.LayoutResizeAndReposition(globalRect.Position + new Vector2(globalRect.Size.X, 0f), HoverTipAlignment.Right);
			break;
		case HoverTipAlignment.Right:
			_cardHoverTipContainer.LayoutResizeAndReposition(globalRect.Position, HoverTipAlignment.Left);
			_textHoverTipContainer.GlobalPosition = globalRect.Position + new Vector2(globalRect.Size.X, 0f);
			break;
		}
		CorrectVerticalOverflow();
		CorrectHorizontalOverflow();
	}

	/// <summary>
	/// Align the hover tip for the given relic. We separate it because there are special positioning
	/// rules for relics.
	/// </summary>
	/// <param name="relic">The relic we are aligning too</param>
	public void SetAlignmentForRelic(NRelic relic)
	{
		HoverTipAlignment hoverTipAlignment = HoverTip.GetHoverTipAlignment(relic);
		Vector2 vector = relic.Icon.Size * relic.GetGlobalTransform().Scale;
		_textHoverTipContainer.GlobalPosition = relic.GlobalPosition + Vector2.Down * (vector.Y + 10f);
		if (hoverTipAlignment == HoverTipAlignment.Left)
		{
			_textHoverTipContainer.Position += Vector2.Left * (_textHoverTipContainer.Size.X - vector.X);
		}
		_cardHoverTipContainer.LayoutResizeAndReposition(_textHoverTipContainer.GlobalPosition + Vector2.Down * _textHoverTipContainer.Size.Y, hoverTipAlignment);
		if (hoverTipAlignment == HoverTipAlignment.Left)
		{
			_cardHoverTipContainer.GlobalPosition = new Vector2(_textHoverTipContainer.GlobalPosition.X, _cardHoverTipContainer.GlobalPosition.Y);
		}
		float y = NGame.Instance.GetViewportRect().Size.Y;
		if (relic.GlobalPosition.Y > y * 0.75f)
		{
			_textHoverTipContainer.GlobalPosition = relic.GlobalPosition + Vector2.Up * _textHoverTipContainer.Size.Y;
		}
		CorrectVerticalOverflow();
		CorrectHorizontalOverflow();
		if (_textHoverTipContainer.GetRect().Intersects(_cardHoverTipContainer.GetRect()))
		{
			if (hoverTipAlignment == HoverTipAlignment.Left)
			{
				_cardHoverTipContainer.GlobalPosition = _textHoverTipContainer.GlobalPosition + _textHoverTipContainer.Size.X * Vector2.Right;
			}
			else
			{
				_cardHoverTipContainer.GlobalPosition = _textHoverTipContainer.GlobalPosition + _cardHoverTipContainer.Size.X * Vector2.Left;
			}
			CorrectVerticalOverflow();
		}
	}

	/// <summary>
	/// Positions the hovertips given a Card. We separate it because there are special positioning
	/// rules for cards.
	/// </summary>
	/// <param name="holder">The holder that we are aligning to</param>
	public void SetAlignmentForCardHolder(NCardHolder holder)
	{
		HoverTipAlignment hoverTipAlignment = HoverTip.GetHoverTipAlignment(holder);
		_textHoverTipContainer.Position = Vector2.Zero;
		Control hitbox = holder.Hitbox;
		if (hoverTipAlignment == HoverTipAlignment.Left)
		{
			_textHoverTipContainer.GlobalPosition = hitbox.GlobalPosition;
			_textHoverTipContainer.Position += Vector2.Left * _textHoverTipContainer.Size.X - new Vector2(10f, 0f);
			_textHoverTipContainer.ReverseFill = true;
			_cardHoverTipContainer.LayoutResizeAndReposition(hitbox.GlobalPosition + new Vector2(hitbox.Size.X, 0f) * hitbox.Scale, HoverTipAlignment.Right);
		}
		else
		{
			Vector2 globalPosition = hitbox.GlobalPosition;
			if (holder.CardModel != null && (holder.CardModel.CurrentStarCost > 0 || holder.CardModel.HasStarCostX))
			{
				globalPosition += Vector2.Left * 15f;
			}
			_cardHoverTipContainer.LayoutResizeAndReposition(globalPosition, HoverTipAlignment.Left);
			_textHoverTipContainer.GlobalPosition = hitbox.GlobalPosition + new Vector2(hitbox.Size.X + 10f, 0f) * hitbox.Scale * holder.Scale;
		}
		CorrectVerticalOverflow();
		CorrectHorizontalOverflow();
		SetFollowOwner();
	}

	private void CorrectVerticalOverflow()
	{
		float y = NGame.Instance.GetViewportRect().Size.Y;
		if (_textHoverTipContainer.GlobalPosition.Y + _textHoverTipContainer.Size.Y > y)
		{
			_textHoverTipContainer.GlobalPosition = new Vector2(_textHoverTipContainer.GlobalPosition.X, y - _textHoverTipContainer.Size.Y);
		}
		if (_cardHoverTipContainer.GlobalPosition.Y + _cardHoverTipContainer.Size.Y > y)
		{
			_cardHoverTipContainer.GlobalPosition = new Vector2(_cardHoverTipContainer.GlobalPosition.X, y - _cardHoverTipContainer.Size.Y);
		}
	}

	private void CorrectHorizontalOverflow()
	{
		float x = NGame.Instance.GetViewportRect().Size.X;
		Vector2 globalPosition = _cardHoverTipContainer.GlobalPosition;
		float x2 = _cardHoverTipContainer.Size.X;
		Vector2 globalPosition2 = _textHoverTipContainer.GlobalPosition;
		float x3 = _textHoverTipContainer.Size.X;
		if (globalPosition.X + x2 <= x && globalPosition2.X + x3 > x)
		{
			float x4 = globalPosition.X - x3;
			_textHoverTipContainer.GlobalPosition = new Vector2(x4, globalPosition.Y);
		}
		else if (globalPosition.X + x2 > x || globalPosition2.X + x3 > x)
		{
			float x5 = globalPosition2.X + x3 - x2;
			_cardHoverTipContainer.GlobalPosition = new Vector2(x5, globalPosition.Y);
			_textHoverTipContainer.GlobalPosition += Vector2.Left * x2;
		}
		else if (globalPosition.X < 0f || globalPosition2.X < 0f)
		{
			float x6 = globalPosition2.X;
			_cardHoverTipContainer.GlobalPosition = new Vector2(x6, globalPosition.Y);
			_textHoverTipContainer.GlobalPosition += Vector2.Right * x2;
		}
	}

	public static void Clear()
	{
		foreach (Control key in _activeHoverTips.Keys)
		{
			Remove(key);
		}
	}

	public static void Remove(Control owner)
	{
		if (_activeHoverTips.Remove(owner, out NHoverTipSet value))
		{
			value.QueueFreeSafely();
		}
		if (_treeExitingHandlers.Remove(owner, out var value2) && GodotObject.IsInstanceValid(owner) && owner.IsConnected(Node.SignalName.TreeExiting, value2))
		{
			owner.Disconnect(Node.SignalName.TreeExiting, value2);
		}
	}

	/// <summary>
	/// Adds a static offset to a HoverTip when it's following a node.
	/// Used when the default offset doesn't have enough padding.
	/// See NLabPotionHolder for example.
	/// </summary>
	public void SetExtraFollowOffset(Vector2 offset)
	{
		_extraOffset = offset;
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(14);
		list.Add(new MethodInfo(MethodName.SetFollowOwner, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ReplaceExistingTip, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "owner", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.ConnectTreeExiting, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "owner", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.CreateAndShowMapPointHistory, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "owner", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false),
			new PropertyInfo(Variant.Type.Object, "historyHoverTip", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("MarginContainer"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SetAlignment, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "node", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false),
			new PropertyInfo(Variant.Type.Int, "alignment", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SetAlignmentForRelic, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "relic", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SetAlignmentForCardHolder, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "holder", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.CorrectVerticalOverflow, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.CorrectHorizontalOverflow, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Clear, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, null, null));
		list.Add(new MethodInfo(MethodName.Remove, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "owner", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SetExtraFollowOffset, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Vector2, "offset", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.SetFollowOwner && args.Count == 0)
		{
			SetFollowOwner();
			ret = default;
			return true;
		}
		if (method == MethodName.ReplaceExistingTip && args.Count == 1)
		{
			ReplaceExistingTip(VariantUtils.ConvertTo<Control>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.ConnectTreeExiting && args.Count == 1)
		{
			ConnectTreeExiting(VariantUtils.ConvertTo<Control>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.CreateAndShowMapPointHistory && args.Count == 2)
		{
			ret = VariantUtils.CreateFrom<NHoverTipSet>(CreateAndShowMapPointHistory(VariantUtils.ConvertTo<Control>(in args[0]), VariantUtils.ConvertTo<NMapPointHistoryHoverTip>(in args[1])));
			return true;
		}
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default;
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.SetAlignment && args.Count == 2)
		{
			SetAlignment(VariantUtils.ConvertTo<Control>(in args[0]), VariantUtils.ConvertTo<HoverTipAlignment>(in args[1]));
			ret = default;
			return true;
		}
		if (method == MethodName.SetAlignmentForRelic && args.Count == 1)
		{
			SetAlignmentForRelic(VariantUtils.ConvertTo<NRelic>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.SetAlignmentForCardHolder && args.Count == 1)
		{
			SetAlignmentForCardHolder(VariantUtils.ConvertTo<NCardHolder>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.CorrectVerticalOverflow && args.Count == 0)
		{
			CorrectVerticalOverflow();
			ret = default;
			return true;
		}
		if (method == MethodName.CorrectHorizontalOverflow && args.Count == 0)
		{
			CorrectHorizontalOverflow();
			ret = default;
			return true;
		}
		if (method == MethodName.Clear && args.Count == 0)
		{
			Clear();
			ret = default;
			return true;
		}
		if (method == MethodName.Remove && args.Count == 1)
		{
			Remove(VariantUtils.ConvertTo<Control>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.SetExtraFollowOffset && args.Count == 1)
		{
			SetExtraFollowOffset(VariantUtils.ConvertTo<Vector2>(in args[0]));
			ret = default;
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.ReplaceExistingTip && args.Count == 1)
		{
			ReplaceExistingTip(VariantUtils.ConvertTo<Control>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.ConnectTreeExiting && args.Count == 1)
		{
			ConnectTreeExiting(VariantUtils.ConvertTo<Control>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.CreateAndShowMapPointHistory && args.Count == 2)
		{
			ret = VariantUtils.CreateFrom<NHoverTipSet>(CreateAndShowMapPointHistory(VariantUtils.ConvertTo<Control>(in args[0]), VariantUtils.ConvertTo<NMapPointHistoryHoverTip>(in args[1])));
			return true;
		}
		if (method == MethodName.Clear && args.Count == 0)
		{
			Clear();
			ret = default;
			return true;
		}
		if (method == MethodName.Remove && args.Count == 1)
		{
			Remove(VariantUtils.ConvertTo<Control>(in args[0]));
			ret = default;
			return true;
		}
		ret = default;
		return false;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.SetFollowOwner)
		{
			return true;
		}
		if (method == MethodName.ReplaceExistingTip)
		{
			return true;
		}
		if (method == MethodName.ConnectTreeExiting)
		{
			return true;
		}
		if (method == MethodName.CreateAndShowMapPointHistory)
		{
			return true;
		}
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.SetAlignment)
		{
			return true;
		}
		if (method == MethodName.SetAlignmentForRelic)
		{
			return true;
		}
		if (method == MethodName.SetAlignmentForCardHolder)
		{
			return true;
		}
		if (method == MethodName.CorrectVerticalOverflow)
		{
			return true;
		}
		if (method == MethodName.CorrectHorizontalOverflow)
		{
			return true;
		}
		if (method == MethodName.Clear)
		{
			return true;
		}
		if (method == MethodName.Remove)
		{
			return true;
		}
		if (method == MethodName.SetExtraFollowOffset)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._textHoverTipContainer)
		{
			_textHoverTipContainer = VariantUtils.ConvertTo<VFlowContainer>(in value);
			return true;
		}
		if (name == PropertyName._cardHoverTipContainer)
		{
			_cardHoverTipContainer = VariantUtils.ConvertTo<NHoverTipCardContainer>(in value);
			return true;
		}
		if (name == PropertyName._owner)
		{
			_owner = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._followOwner)
		{
			_followOwner = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._followOffset)
		{
			_followOffset = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._extraOffset)
		{
			_extraOffset = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		Vector2 from;
		if (name == PropertyName.TextHoverTipDimensions)
		{
			from = TextHoverTipDimensions;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.CardHoverTipDimensions)
		{
			from = CardHoverTipDimensions;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName._textHoverTipContainer)
		{
			value = VariantUtils.CreateFrom(in _textHoverTipContainer);
			return true;
		}
		if (name == PropertyName._cardHoverTipContainer)
		{
			value = VariantUtils.CreateFrom(in _cardHoverTipContainer);
			return true;
		}
		if (name == PropertyName._owner)
		{
			value = VariantUtils.CreateFrom(in _owner);
			return true;
		}
		if (name == PropertyName._followOwner)
		{
			value = VariantUtils.CreateFrom(in _followOwner);
			return true;
		}
		if (name == PropertyName._followOffset)
		{
			value = VariantUtils.CreateFrom(in _followOffset);
			return true;
		}
		if (name == PropertyName._extraOffset)
		{
			value = VariantUtils.CreateFrom(in _extraOffset);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._textHoverTipContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._cardHoverTipContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName.TextHoverTipDimensions, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName.CardHoverTipDimensions, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._owner, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._followOwner, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._followOffset, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._extraOffset, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._textHoverTipContainer, Variant.From(in _textHoverTipContainer));
		info.AddProperty(PropertyName._cardHoverTipContainer, Variant.From(in _cardHoverTipContainer));
		info.AddProperty(PropertyName._owner, Variant.From(in _owner));
		info.AddProperty(PropertyName._followOwner, Variant.From(in _followOwner));
		info.AddProperty(PropertyName._followOffset, Variant.From(in _followOffset));
		info.AddProperty(PropertyName._extraOffset, Variant.From(in _extraOffset));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._textHoverTipContainer, out var value))
		{
			_textHoverTipContainer = value.As<VFlowContainer>();
		}
		if (info.TryGetProperty(PropertyName._cardHoverTipContainer, out var value2))
		{
			_cardHoverTipContainer = value2.As<NHoverTipCardContainer>();
		}
		if (info.TryGetProperty(PropertyName._owner, out var value3))
		{
			_owner = value3.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._followOwner, out var value4))
		{
			_followOwner = value4.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._followOffset, out var value5))
		{
			_followOffset = value5.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._extraOffset, out var value6))
		{
			_extraOffset = value6.As<Vector2>();
		}
	}
}
