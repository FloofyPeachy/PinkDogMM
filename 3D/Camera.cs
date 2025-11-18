using System.Collections.Specialized;
using System.Linq;
using Godot;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Render;

public enum CameraMode
{
    Orbit,
    Free
}

public enum CameraProjection
{
    Perspective,
    Orthogonal
}

public class Camera
{
    public Vector2L Rotation = new();
    public float Zoom = 20;
    public Vector3L Position = new Vector3L();
    public CameraMode Mode = CameraMode.Orbit;
    public CameraProjection Projection = CameraProjection.Perspective;
    public bool CenterOnAnchor = false;
    private ModelEditorState state;

    public Camera(ModelEditorState state)
    {
        this.state = state;
        state.ObjectSelectionChanged += (sender, tuple) =>
        {
            UpdateCamera();
            tuple.Item1.PropertyChanged += (sender, args) =>
            {
                if (!state.IsMoving)
                {
                    UpdateCamera();
                }
            };
        };
        state.MovingObject += moving =>
        {
            if (!moving)
            {
                UpdateCamera();
            }
        };
    }

    public void SetMode(CameraMode _mode, CameraProjection projection)
    {
        this.Mode = _mode;
        this.Projection = projection;
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (Mode == CameraMode.Free) return;

        if (state.Mode != EditorMode.Normal) return;
        if (state.SelectedObjects.Count != 0)
        {
            var selected = state.SelectedObjects.OfType<Part>().ToList();
            if (selected.Count == 0)
                return;

            var pos = selected
                .Select(PositionOfCorner)
                .Aggregate(Vector3.Zero, (a, b) => a + b) / selected.Count;

            Position.X = pos.Z;
            Position.Y = -pos.Y;
            Position.Z = pos.X;
        }
        else
        {
            Position.X = 0;
            Position.Y = 0;
            Position.Z = 0;
        }
    }


    private Vector3 PositionOfCorner(Part part)
    {
        var extra = Vector3.Zero;
        if (!CenterOnAnchor)
        {
            return ((part.Position.AsVector3() + part.Size.AsVector3() / 2));
        }

        int index = state.FocusedCorner;

        double GetComponent(double pos, double dim, double offset, float[]? shapeArray, bool addDim)
        {
            double shapeValue = 0;
            if (shapeArray != null && index >= 0 && index < shapeArray.Length)
                shapeValue = shapeArray[index];

            return addDim
                ? pos + dim + shapeValue + offset
                : pos - shapeValue + offset;
        }

        bool addDimX = state.FocusedCorner is 1 or 2 or 5 or 6;

        bool addDimY = state.FocusedCorner is >= 4 and <= 7;

        bool addDimZ = state.FocusedCorner is 2 or 3 or 6 or 7;

        Vector3 result = new Vector3(
            (float)GetComponent(part.Position.X, part.Size.X, part.Offset.X,
                (part is Shapebox shapebox ? shapebox.ShapeboxX.ToArray() : null), addDimX),
            (float)GetComponent(part.Position.Y, part.Size.Y, part.Offset.Y,
                (part is Shapebox shapebox1 ? shapebox1.ShapeboxY.ToArray() : null), addDimY),
            (float)GetComponent(part.Position.Z, part.Size.Z, part.Offset.Z,
                (part is Shapebox shapebox2 ? shapebox2.ShapeboxZ.ToArray() : null), addDimZ)
        );

        return result;
    }

    public override string ToString()
    {
        return
            $"{nameof(Position)}: {Position},{nameof(Rotation)}: {Rotation},{nameof(Zoom)}: {Zoom}, {nameof(Mode)}: {Mode}";
    }
}