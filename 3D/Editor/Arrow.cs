using Godot;
using System;

public partial class Arrow : MeshInstance3D
{
	public override void _Ready()
	{
		Vector2 startingPoint = new Vector2(10, 10);
		Vector2 endingPoint = new Vector2(100, -80);
		float arrowSize = 20f;
		float flatness = 0.5f;
		Vector2 direction = (endingPoint - startingPoint).Normalized();
		Vector2 side1 = new Vector2(-direction.Y, direction.X);
		Vector2 side2 = new Vector2(direction.Y, -direction.X);

		// tip and base of arrowhead
		Vector2 e1 = endingPoint + side1 * arrowSize * flatness;
		Vector2 e2 = endingPoint + side2 * arrowSize * flatness;
		Vector2 p1 = e1 - direction * arrowSize;
		Vector2 p2 = e2 - direction * arrowSize;

		var st = new SurfaceTool();
		st.Begin(Mesh.PrimitiveType.Triangles);

		// Body of arrow = rectangle from start to end
		Vector2 bodySide1 = startingPoint + side1 * (arrowSize * flatness * 0.3f);
		Vector2 bodySide2 = startingPoint + side2 * (arrowSize * flatness * 0.3f);

		// map 2D points into XY plane in 3D
		Vector3 v_start1 = new Vector3(bodySide1.X, bodySide1.Y, 0);
		Vector3 v_start2 = new Vector3(bodySide2.X, bodySide2.Y, 0);
		Vector3 v_end1 = new Vector3(p1.X, p1.Y, 0);
		Vector3 v_end2 = new Vector3(p2.X, p2.Y, 0);
		Vector3 v_tip = new Vector3(endingPoint.X, endingPoint.Y, 0);

		// rectangle body (two triangles)
		st.AddVertex(v_start1);
		st.AddVertex(v_start2);
		st.AddVertex(v_end1);

		st.AddVertex(v_start2);
		st.AddVertex(v_end2);
		st.AddVertex(v_end1);

		// arrowhead (triangle)
		st.AddVertex(v_tip);
		st.AddVertex(v_end1);
		st.AddVertex(v_end2);

		
		this.Mesh = st.Commit();
	}
}
