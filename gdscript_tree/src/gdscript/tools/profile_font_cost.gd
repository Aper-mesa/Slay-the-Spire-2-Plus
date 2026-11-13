extends SceneTree

























const MSDF_SIZE: = 48
const LAYOUT_FONT_SIZE: = 24
const WEIGHT_TEXT: = "res://preload_weight_text.json"


const LOCALE_FONTS: = {
	"ara": {
		"regular": "res://themes/fonts/ara/alyamama_regular_shared.tres",
		"bold": "res://themes/fonts/ara/alyamama_bold_shared.tres",
		"italic": "res://themes/fonts/ara/alyamama_regular_shared.tres",
	},
	"jpn": {
		"regular": "res://themes/fonts/jpn/noto_sans_cjkjp_regular_shared.tres",
		"bold": "res://themes/fonts/jpn/noto_sans_cjkjp_bold_shared.tres",
		"italic": "res://themes/fonts/jpn/noto_sans_cjkjp_medium_shared.tres",
	},
	"zhs": {
		"regular": "res://themes/fonts/zhs/noto_sans_mono_cjksc_regular_shared.tres",
		"bold": "res://themes/fonts/zhs/source_han_serif_sc_bold_shared.tres",
		"italic": "res://themes/fonts/zhs/source_han_serif_sc_medium_shared.tres",
	},
	"zht": {
		"regular": "res://themes/fonts/zht/noto_sans_mono_cjktc_regular_shared.tres",
		"bold": "res://themes/fonts/zht/source_han_serif_tc_bold_shared.tres",
		"italic": "res://themes/fonts/zht/source_han_serif_tc_medium_shared.tres",
	},
	"kor": {
		"regular": "res://themes/fonts/kor/gyeonggi_cheonnyeon_batang_bold_shared.tres",
		"bold": "res://themes/fonts/kor/gyeonggi_cheonnyeon_batang_bold_shared.tres",
		"italic": "res://themes/fonts/kor/gyeonggi_cheonnyeon_batang_bold_shared.tres",
	},
	"rus": {
		"regular": "res://themes/fonts/rus/fira_sans_extra_condensed_regular_shared.tres",
		"bold": "res://themes/fonts/rus/fira_sans_extra_condensed_bold_shared.tres",
		"italic": "res://themes/fonts/rus/fira_sans_extra_condensed_italic_shared.tres",
	},

	"pol": {
		"regular": "res://themes/fonts/rus/fira_sans_extra_condensed_regular_shared.tres",
		"bold": "res://themes/fonts/rus/fira_sans_extra_condensed_bold_shared.tres",
		"italic": "res://themes/fonts/rus/fira_sans_extra_condensed_italic_shared.tres",
	},
	"tha": {
		"regular": "res://themes/fonts/tha/cs_chat_thai_ui_shared.tres",
		"bold": "res://themes/fonts/tha/cs_chat_thai_ui_shared.tres",
		"italic": "res://themes/fonts/tha/cs_chat_thai_ui_shared.tres",
	},


	"eng": {
		"regular": "res://themes/kreon_regular_shared.tres",
		"bold": "res://themes/kreon_bold_shared.tres",
	},
}

var _weight_text: = {}
var _corpus_cache: = {}


func _init() -> void :
	call_deferred("_run")


func _vram_mb() -> float:
	return RenderingServer.get_rendering_info(RenderingServer.RENDERING_INFO_TEXTURE_MEM_USED) / 1048576.0


func _settle() -> void :

	for i in 5:
		await process_frame


func _run() -> void :
	DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_DISABLED)
	var args: PackedStringArray = OS.get_cmdline_user_args()
	var lang: String = args[0] if args.size() > 0 else "jpn"
	if not LOCALE_FONTS.has(lang):
		printerr("unknown lang ", lang)
		quit(1)
		return

	if args.size() > 1 and args[1] == "draw":
		await _run_draw(lang)
		return

	if not _load_weight_text():
		quit(1)
		return

	var ts: = TextServerManager.get_primary_interface()
	var size: = Vector2i(MSDF_SIZE, 0)

	await _settle()
	var vram_base: = _vram_mb()



	var retained: Array[Font] = []
	var seen_tres: = {}
	var total_baked: = 0
	var total_runtime: = 0

	for weight in ["regular", "bold", "italic"]:
		if not LOCALE_FONTS[lang].has(weight):
			continue
		var tres_path: String = LOCALE_FONTS[lang][weight]
		var font: FontVariation = load(tres_path)
		retained.append(font)
		if seen_tres.has(tres_path):
			print("  %-8s shares %s" % [weight, tres_path.get_file()])
			continue
		seen_tres[tres_path] = true

		var rid: RID = font.get_rids()[0]
		await _settle()
		var baked: = ts.font_get_glyph_list(rid, size).size()
		var vram_after_load: = _vram_mb()

		var text: = _text_for(lang, weight)
		var start: = Time.get_ticks_usec()
		for s in text:
			font.get_string_size(s, HORIZONTAL_ALIGNMENT_LEFT, -1, LAYOUT_FONT_SIZE)
		var layout_ms: = (Time.get_ticks_usec() - start) / 1000.0

		var after: = ts.font_get_glyph_list(rid, size).size()
		total_baked += baked
		total_runtime += after - baked
		print("  %-8s baked=%-5d after=%-5d rasterized=%-5d layout=%.1fms strings=%d vram_after_load=%.1fMB" % [
			weight, baked, after, after - baked, layout_ms, text.size(), vram_after_load,
		])

	await _settle()
	var vram_end: = _vram_mb()
	print("RESULT lang=%s baked=%d rasterized=%d vram_base=%.1fMB vram_end=%.1fMB fonts_vram=%.1fMB" % [
		lang, total_baked, total_runtime, vram_base, vram_end, vram_end - vram_base,
	])
	quit()


func _load_weight_text() -> bool:
	if not FileAccess.file_exists(WEIGHT_TEXT):
		printerr("missing ", WEIGHT_TEXT, ". Run scripts/extract_weight_text.py first.")
		return false
	var parsed: Variant = JSON.parse_string(FileAccess.get_file_as_string(WEIGHT_TEXT))
	if typeof(parsed) != TYPE_DICTIONARY:
		printerr("could not parse ", WEIGHT_TEXT)
		return false
	_weight_text = parsed
	return true


func _rss_mb() -> float:
	var f: = FileAccess.open("/proc/self/status", FileAccess.READ)
	if f == null:
		return 0.0
	while not f.eof_reached():
		var line: = f.get_line()
		if line.begins_with("VmRSS:"):
			return line.split(":")[1].strip_edges().split(" ")[0].to_float() / 1024.0
	return 0.0


func _run_draw(lang: String) -> void :


	var keep_bi: = RegEx.create_from_string("\\[/?(?!b\\]|i\\])[a-zA-Z_][a-zA-Z0-9_]*(=[^\\]]*)?\\]")

	await _settle()
	var vram_base: = _vram_mb()
	var rss_base: = _rss_mb()

	var fonts: = LOCALE_FONTS[lang] as Dictionary
	var regular: Font = load(fonts["regular"])
	var bold: Font = load(fonts.get("bold", fonts["regular"]))
	var italic: Font = load(fonts.get("italic", fonts["regular"]))

	var label: = RichTextLabel.new()
	label.bbcode_enabled = true
	label.custom_minimum_size = Vector2(1600, 900)
	label.size = Vector2(1600, 900)
	label.add_theme_font_override("normal_font", regular)
	label.add_theme_font_override("bold_font", bold)
	label.add_theme_font_override("italics_font", italic)
	label.add_theme_font_size_override("normal_font_size", LAYOUT_FONT_SIZE)
	label.add_theme_font_size_override("bold_font_size", LAYOUT_FONT_SIZE)
	label.add_theme_font_size_override("italics_font_size", LAYOUT_FONT_SIZE)
	root.add_child(label)

	await _settle()
	var vram_loaded: = _vram_mb()

	var frames: PackedFloat64Array = PackedFloat64Array()
	for s in _load_corpus(lang):
		var t0: = Time.get_ticks_usec()
		label.text = keep_bi.sub(s, "", true)
		await process_frame
		frames.append((Time.get_ticks_usec() - t0) / 1000.0)

	await _settle()
	var vram_drawn: = _vram_mb()
	var rss_end: = _rss_mb()

	var sorted: = frames.duplicate()
	sorted.sort()
	var n: = sorted.size()
	if n == 0:
		printerr("no frames drawn for ", lang, ". Its corpus is empty.")
		quit(1)
		return
	var over: = 0
	var total: = 0.0
	for ft in frames:
		total += ft
		if ft > 16.7:
			over += 1

	print("DRAW lang=%s frames=%d total_ms=%.0f p50=%.2f p95=%.2f p99=%.2f worst=%.1f over_budget=%d vram_loaded=%.1fMB vram_drawn=%.1fMB rss_base=%.1fMB rss_end=%.1fMB static=%.1fMB" % [
		lang, n, total, sorted[n / 2], sorted[int(n * 0.95)], sorted[int(n * 0.99)],
		sorted[n - 1], over, vram_loaded, vram_drawn, rss_base, rss_end,
		OS.get_static_memory_usage() / 1048576.0,
	])
	quit()


func _load_corpus(lang: String) -> PackedStringArray:
	if _corpus_cache.has(lang):
		return _corpus_cache[lang]
	var out: = PackedStringArray()
	var dir: = DirAccess.open("res://localization/%s" % lang)
	if dir != null:
		for f in dir.get_files():
			if not f.ends_with(".json"):
				continue
			var parsed: Variant = JSON.parse_string(
				FileAccess.get_file_as_string("res://localization/%s/%s" % [lang, f])
			)
			if typeof(parsed) != TYPE_DICTIONARY:
				continue
			for k in parsed:
				if typeof(parsed[k]) == TYPE_STRING:
					out.append(parsed[k])
	_corpus_cache[lang] = out
	return out


func _text_for(lang: String, weight: String) -> PackedStringArray:
	var langs: = [lang]

	if lang == "rus":
		langs.append("pol")
	var out: = PackedStringArray()
	for l in langs:
		if not _weight_text.has(l):
			continue
		var by_weight: Dictionary = _weight_text[l]
		if not by_weight.has(weight):
			continue
		for s in by_weight[weight]:
			out.append(s)
	return out
