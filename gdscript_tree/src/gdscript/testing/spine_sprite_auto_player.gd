extends SpineSprite

@export var anim_name: String

func _ready() -> void :
	self.get_animation_state().set_animation(anim_name)
