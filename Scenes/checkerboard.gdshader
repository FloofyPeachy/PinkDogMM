shader_type canvas_item;

uniform vec2 grid_size = vec2(10.0, 10.0); // Size of the checkerboard squares
uniform vec4 color1 : source_color = vec4(0.8, 0.8, 0.8, 1.0); // Light gray
uniform vec4 color2 : source_color = vec4(0.4, 0.4, 0.4, 1.0); // Dark gray
uniform vec2 aspect_ratio_vector = vec2(1, 1.1); // Default aspect ratio vector (to be updated by script)

void fragment() {
    // Calculate the aspect ratio
    float aspect_ratio = aspect_ratio_vector.x / aspect_ratio_vector.y;

    // Adjust UV coordinates to preserve aspect ratio
    vec2 adjusted_uv = vec2(UV.x, UV.y * aspect_ratio);

    // Calculate the grid position based on the adjusted UV and grid size
    vec2 grid_pos = floor(adjusted_uv * grid_size);

    // Use mod to alternate between color1 and color2
    float checker = mod(grid_pos.x + grid_pos.y, 2.0);

    // Assign the color based on the checker value
    COLOR = mix(color1, color2, checker);
}
