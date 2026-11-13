extends SceneTree























const MSDF_SIZE: = 48
const MANIFEST: = "res://fonts/preload_manifest.json"
const WEIGHT_TEXT: = "res://preload_weight_text.json"





const SHAPED_REGULAR_LOCALES: = ["ara"]


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
	"tha": {
		"regular": "res://themes/fonts/tha/cs_chat_thai_ui_shared.tres",
		"bold": "res://themes/fonts/tha/cs_chat_thai_ui_shared.tres",
		"italic": "res://themes/fonts/tha/cs_chat_thai_ui_shared.tres",
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
}

var _weight_text: = {}


func _init() -> void :
	var ts: = TextServerManager.get_primary_interface()

	if not _load_weight_text():
		quit(1)
		return



	for lang in LOCALE_FONTS:
		if not _weight_text.has(lang):
			printerr(lang, " is missing from ", WEIGHT_TEXT, ". Add it to LANGS in scripts/extract_weight_text.py.")
			quit(1)
			return




	var regular_fonts: = {}
	for lang in LOCALE_FONTS:
		if lang not in SHAPED_REGULAR_LOCALES:
			regular_fonts[LOCALE_FONTS[lang]["regular"]] = true



	var work: = {}
	for lang in LOCALE_FONTS:
		for weight in LOCALE_FONTS[lang]:
			var tres_path: String = LOCALE_FONTS[lang][weight]
			if regular_fonts.has(tres_path):
				continue
			if not work.has(tres_path):
				work[tres_path] = {"sources": [], "text": PackedStringArray()}
			work[tres_path]["sources"].append([lang, weight])
			work[tres_path]["text"].append_array(_text_for(lang, weight))

	var manifest: = {}
	var failed: = false

	for tres_path in work:
		var font: FontVariation = load(tres_path)
		if font == null:
			printerr("could not load ", tres_path)
			failed = true
			continue

		var rid: RID = font.get_rids()[0]
		var size: = Vector2i(MSDF_SIZE, 0)


		ts.font_clear_glyphs(rid, size)
		for s in work[tres_path]["text"]:
			font.get_string_size(s, HORIZONTAL_ALIGNMENT_LEFT, -1, 24)

		var glyphs: = Array(ts.font_get_glyph_list(rid, size))
		glyphs.sort()

		var import_path: = _import_path_for(tres_path)
		if import_path.is_empty() or not _write_glyphs(import_path, glyphs):
			failed = true
			continue

		var sources: Array = work[tres_path]["sources"]
		sources.sort()
		manifest[import_path.replace("res://", "")] = {
			"sources": sources,
			"glyphs": glyphs.size(),
			"corpus_hash": _corpus_hash(sources),
			"glyph_hash": _glyph_hash(glyphs),
			"font_hash": _font_hash(import_path),
		}
		print("%-56s %5d glyphs  %s" % [import_path.replace("res://fonts/", ""), glyphs.size(), _describe(sources)])


	if failed:
		quit(1)
		return

	var f: = FileAccess.open(MANIFEST, FileAccess.WRITE)
	if f == null:
		printerr("could not write ", MANIFEST)
		quit(1)
		return
	f.store_string(JSON.stringify(manifest, "  ", true) + "\n")
	f.close()

	quit(0)


func _describe(sources: Array) -> String:
	var parts: = PackedStringArray()
	for s in sources:
		parts.append("%s/%s" % [s[0], s[1]])
	return "(" + ", ".join(parts) + ")"


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


func _text_for(lang: String, weight: String) -> PackedStringArray:
	var out: = PackedStringArray()
	if not _weight_text.has(lang):
		return out
	var by_weight: Dictionary = _weight_text[lang]
	if not by_weight.has(weight):
		return out
	for s in by_weight[weight]:
		out.append(s)
	return out


func _import_path_for(tres_path: String) -> String:
	var re: = RegEx.create_from_string("path=\"(res://fonts/[^\"]+)\"")
	var m: = re.search(FileAccess.get_file_as_string(tres_path))
	if m == null:
		printerr("could not find the base font for ", tres_path)
		return ""
	return m.get_string(1) + ".import"


func _write_glyphs(import_path: String, glyphs: Array) -> bool:
	var text: = FileAccess.get_file_as_string(import_path)
	if text.is_empty():
		printerr("could not read ", import_path)
		return false

	var re: = RegEx.create_from_string("\"glyphs\": \\[[^\\]]*\\]")
	var m: = re.search(text)
	if m == null:
		printerr("no glyphs array in ", import_path)
		return false
	if re.search(text, m.get_end()) != null:
		printerr("more than one glyphs array in ", import_path, "; this tool only writes the first")
		return false

	var parts: = PackedStringArray()
	for g in glyphs:
		parts.append(str(g))

	var f: = FileAccess.open(import_path, FileAccess.WRITE)
	if f == null:
		printerr("could not write ", import_path)
		return false
	f.store_string(text.substr(0, m.get_start()) + "\"glyphs\": [" + ", ".join(parts) + "]" + text.substr(m.get_end()))
	f.close()
	return true


func _font_hash(import_path: String) -> String:



	return FileAccess.get_sha256(import_path.trim_suffix(".import")).substr(0, 16)


func _glyph_hash(glyphs: Array) -> String:


	var ctx: = HashingContext.new()
	ctx.start(HashingContext.HASH_SHA256)
	for g in glyphs:
		ctx.update(str(g).to_utf8_buffer())
		ctx.update(",".to_utf8_buffer())
	return ctx.finish().hex_encode().substr(0, 16)


func _corpus_hash(sources: Array) -> String:


	var chars: = {}
	var pairs: = {}
	for source in sources:
		for s in _text_for(source[0], source[1]):
			for i in s.length():
				var c: = s.unicode_at(i)
				chars[c] = true
				if i > 0:
					var p: = s.unicode_at(i - 1)
					if p < 128 and c < 128:
						pairs[Vector2i(p, c)] = true
	var codes: = chars.keys()
	codes.sort()
	var ctx: = HashingContext.new()
	ctx.start(HashingContext.HASH_SHA256)
	for c in codes:
		ctx.update(str(c).to_utf8_buffer())
		ctx.update(",".to_utf8_buffer())
	var pair_keys: = pairs.keys()
	pair_keys.sort()
	for p in pair_keys:
		ctx.update(("%d:%d" % [p.x, p.y]).to_utf8_buffer())
		ctx.update(",".to_utf8_buffer())
	return ctx.finish().hex_encode().substr(0, 16)
