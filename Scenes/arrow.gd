extends MeshInstance3D

func _ready():
	var surface_tool = SurfaceTool.new()

	# Begin creating the mesh with primitive triangles
	surface_tool.begin(Mesh.PRIMITIVE_TRIANGLES)

	# Define the vertices of the rectangle as two triangles
	# Triangle 1
	surface_tool.add_vertex(Vector3(-1, 0, 1))
	surface_tool.add_vertex(Vector3(1, 0, 1))
	surface_tool.add_vertex(Vector3(1, 0, -1))

	# Triangle 2
	surface_tool.add_vertex(Vector3(-1, 0, 1))
	surface_tool.add_vertex(Vector3(1, 0, -1))
	surface_tool.add_vertex(Vector3(-1, 0, -1))

	# Create the mesh from the SurfaceTool data
	surface_tool.index()
	mesh = surface_tool.commit()
