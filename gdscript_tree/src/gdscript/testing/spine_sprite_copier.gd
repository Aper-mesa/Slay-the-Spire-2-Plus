extends SpineSprite

@export var target_spine: SpineSprite

func _ready() -> void :
	scale = target_spine.scale;
	skeleton_data_res = target_spine.skeleton_data_res;

func _process(_delta: float) -> void :
	target_spine.get_animation_state().apply(self.get_skeleton())
