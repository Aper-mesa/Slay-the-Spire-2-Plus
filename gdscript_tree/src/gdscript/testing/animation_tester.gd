extends Node



@export_group("Main Animations")
@export var track0Anims: Array[String]
@export var track1Anims: Array[String]

@export_group("Optional Settings")
@export var skin: String
@export var track_2_anim: String
@export var disableScript: bool = false;

var spineSprite: SpineSprite
var animState: SpineAnimationState
var curIndex: int = 0;
var keyPressed: bool = false;
var isSkipping: bool = false;
var hasTrack1Anims: bool = false;






func _ready():
	if (disableScript == true):
		print("Animation Tester script disable box checked.")
		return

	var parent: Node = get_node("..")
	if (parent is SpineSprite):
		spineSprite = parent
		animState = spineSprite.get_animation_state()
	else:
		print("WARNING: Parent of animation tester needs to be a SpineSprite!")
		return

	var _wtf = animState.set_animation(track0Anims[0], false, 0)

	if (track1Anims.size() > 0):
		hasTrack1Anims = true
		var _ftw = animState.set_animation(track1Anims[0], false, 1)

	queueNext()
	spineSprite.animation_ended.connect(_animation_ended)
	if ( !track_2_anim.is_empty()):
		animState.set_animation(track_2_anim, true, 2)
	if (skin.length() > 0):
		var custom_skin = spineSprite.new_skin("newSkin")
		var data = spineSprite.get_skeleton().get_data()
		custom_skin.add_skin(data.find_skin(skin))
		spineSprite.get_skeleton().set_skin(custom_skin)
		spineSprite.get_skeleton().set_slots_to_setup_pose()





func _process(_delta):
	if (disableScript == true):
		return

	if (Input.is_key_pressed(KEY_SPACE)):
		if ( !keyPressed):
			if ( !isSkipping):
				keyPressed = true
				skipAhead()
	else:
		keyPressed = false





func skipAhead():
	isSkipping = true;
	animState.set_animation(track0Anims[curIndex], false, 0)

	if (hasTrack1Anims):
		if curIndex < track1Anims.size():
			animState.set_animation(track1Anims[curIndex], false, 1)
		else:
			animState.set_empty_animation(1, 0.0)


	queueNext()
	print("skipping animation")





func queueNext():
	curIndex += 1
	if curIndex == track0Anims.size():
		curIndex = 0;
	print("queueing animaton at index ", curIndex)
	animState.add_animation(track0Anims[curIndex], 0.0, false, 0)


	if (hasTrack1Anims):
		if curIndex < track1Anims.size():
			animState.add_animation(track1Anims[curIndex], 0.0, false, 1)
		else:
			animState.add_empty_animation(1, 0.0, 0.0)







func _animation_ended(_sprite: SpineSprite, _animation_state: SpineAnimationState, _track_entry: SpineTrackEntry):
	if (_track_entry.get_track_index() != 0):
		return
	if (isSkipping):
		isSkipping = false
		print("animation skipped")
		return
	else:
		print("queueing next animation")
		queueNext()
