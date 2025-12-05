using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd.Core;
using PinkDogMM_Gd.Core.Actions.All.TheModel;
using PinkDogMM_Gd.Core.Schema;
using PinkDogMM_Gd.Render;

namespace PinkDogMM_Gd.Scenes;

public partial class PivotXY2 : Node3D
{
    private Node3D pivotY;
    private Node3D pivotX;
    private Camera3D camera;
    private PopupMenu popupMenu;
    private const float Sensitivity = 0.5f;
    private const float ZoomSpeed = 0.3f;
    private Vector3 ActualPosition;
    private Vector2 Pan;
    private const float RayLength = 1000.0f;
    private const float DragThreshold = 2f;
    private Vector2 ClickPosition = Vector2.Zero;
    private Vector3 CamPosition = Vector3.Zero;
    private bool RightButtDown = false;
    private bool menuUp;
    private ModelEditorState state;

    

    public override void _Ready()
    {
        pivotY = GetNode<Node3D>("PivotY");
        popupMenu = GetNode<PopupMenu>("PopupMenu");
        pivotX = GetNode<Node3D>("PivotY/PivotX");
        camera = GetNode<Camera3D>("PivotY/PivotX/Camera3D");

        state = Model.Get(this).State;

        state.Camera.Position.PropertyChanged += (sender, args) =>
        {
            SetTranslation();
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        
        
        
        
        
        
        
        
        
        this.RotationDegrees = new Vector3(0,0,0);
        var forward = new Vector3(
            float.Cos(pivotX.Rotation.X) * float.Cos(pivotY.Rotation.Y),
            float.Sin(pivotX.Rotation.X),
            float.Cos(pivotX.Rotation.X) * float.Sin(pivotY.Rotation.Y)
        ).Normalized();

        camera.Projection = state.Camera.Projection == CameraProjection.Perspective
            ? Camera3D.ProjectionType.Perspective
            : Camera3D.ProjectionType.Orthogonal;

        if (state.Camera.Projection == CameraProjection.Orthogonal)
        {
            camera.Position = camera.Position.Lerp(new Vector3(Pan.X,Pan.Y,1000), 
                (float)delta * 24.0f);
            camera.Size = float.Lerp(camera.Size, CamPosition.Z,
                (float)delta * 24.0f);
        }
        else
        {
            camera.Position = camera.Position.Lerp(new Vector3(0,0, state.Camera.Zoom ),
                (float)delta * 24.0f);
        }
        this.GlobalPosition = GlobalPosition.Lerp(ActualPosition * new Vector3(1,1 ,1) * 1/16,
            (float)delta * 16.0f);
        
        if (state.Camera.Mode == CameraMode.Orbit) return;
        
        
        
        
        
        
        

// Right vector as before
        var right = forward.Cross(Vector3.Up).Normalized();

// Input (WASD)
        float xInput = Convert.ToSingle(Input.IsPhysicalKeyPressed(Key.D)) -
                       Convert.ToSingle(Input.IsPhysicalKeyPressed(Key.A));
        float zInput = Convert.ToSingle(Input.IsPhysicalKeyPressed(Key.W)) -
                       Convert.ToSingle(Input.IsPhysicalKeyPressed(Key.S));

// Combine—this time, use the full forward vector (with its Y)
        var moveDir = (right * zInput) + (forward * xInput);
        if (moveDir.LengthSquared() > 0)
            moveDir = moveDir.Normalized();

        var speed = 16.0f;
        var v3 = state.Camera.Position.AsVector3();
        
        v3 += moveDir * speed;
        if (v3 != state.Camera.Position.AsVector3())
        {
            v3 *= (float)delta;
            state.Camera.Position.X = v3.X;
            state.Camera.Position.Y = v3.Y;
            state.Camera.Position.Z = v3.Z;
        }
       
        
        
        
        
        /*this.GlobalPosition = GlobalPosition.Lerp(ActualPosition,
            (float)delta * 16.0f);*/
        //GD.Print(forward);
        /*_direction = new Vector3(
           (_d ? 1.0f : 0.0f) - (_a ? 1.0f : 0.0f),
           (_e ? 1.0f : 0.0f) - (_q ? 1.0f : 0.0f),
           (_s ? 1.0f : 0.0f) - (_w ? 1.0f : 0.0f)
       );

       var offset = _direction.Normalized() * _acceleration * _vel_multiplier * (float)delta
           +_velocity.Normalized() * _deceleration * _vel_multiplier * (float)delta;


       var speed_multi = 1f;
       if (_shift) speed_multi *= SHIFT_MULTIPLIER;
       if (_alt) speed_multi *= ALT_MULTIPLIER;
       if (_direction == Vector3.Zero && offset.LengthSquared() > _velocity.LengthSquared())
       {
           _velocity = Vector3.Zero;
       }
       else
       {
           _velocity.X = Mathf.Clamp(_velocity.X + offset.X, -_vel_multiplier, _vel_multiplier);
           _velocity.Y = Mathf.Clamp(_velocity.Y + offset.Y, -_vel_multiplier, _vel_multiplier);
           _velocity.Y = Mathf.Clamp(_velocity.Y + offset.Y, -_vel_multiplier, _vel_multiplier);
       }
       */

        //GD.Print(_velocity);
        //Translate(_velocity * (float)delta * speed_multi);
    }

    public void SetTranslation()
    {
        ActualPosition = state.Camera.Position.AsVector3();
       
        //state.Camera.Zoom = state.Camera.Zoom - state.Camera.Position.Y;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton button)
        {
            switch (button.ButtonIndex)
            {
                case MouseButton.WheelUp:
                    if (state.Camera.Projection == CameraProjection.Perspective)
                    {
                        CamPosition = camera.Position + new Vector3(0, 0,  -ZoomSpeed);
                        state.Camera.Zoom = CamPosition.Z;
                    }
                    else
                    {
                        CamPosition += new Vector3(0, 0,  -ZoomSpeed);
                        state.Camera.Zoom = CamPosition.Z * -10;
                    }
                    
                    //state.Camera.Position.Z--;
                    GetViewport().SetInputAsHandled();
                    break;
                case MouseButton.WheelDown:
                    if (state.Camera.Projection == CameraProjection.Perspective)
                    {
                        CamPosition = camera.Position + new Vector3(0, 0,  ZoomSpeed);
                        state.Camera.Zoom = CamPosition.Z;
                    }
                    else
                    {
                        CamPosition -= new Vector3(0, 0,  -ZoomSpeed);
                        state.Camera.Zoom = CamPosition.Z * 10;
                    }

                    //state.Camera.Position.Z++;
                    GetViewport().SetInputAsHandled();
                    break;
                default:
                    break;
            }
        }

        if (@event is InputEventKey key)
        {
            /*switch (key.Keycode)
            {
                case Key.W:
                    _w = key.Pressed;
                    break;
                case Key.S:
                    _s = key.Pressed;
                    break;
                case Key.A:
                    _a = key.Pressed;
                    break;
                case Key.D:
                    _d = key.Pressed;
                    break;
                case Key.Q:
                    _q = key.Pressed;
                    break;
                case Key.E:
                    _e = key.Pressed;
                    break;
                case Key.Shift:
                    _shift = key.Pressed;
                    break;
                case Key.Alt:
                    _alt = key.Pressed;
                    break;
                default:
                    break;
            }*/
        }

        if (@event is not InputEventMouseMotion motion) return;

        
        
        

        /*if (motion.Position.DistanceTo(ClickPosition) < DragThreshold) return;*/

        if (!Input.IsMouseButtonPressed(MouseButton.Middle) && Input.IsMouseButtonPressed(MouseButton.Right))
        {
            pivotY.Rotate(pivotY.Transform.Basis.Y, -motion.Relative.X * Mathf.DegToRad(Sensitivity));
            pivotX.Rotate(pivotX.Transform.Basis.X, -motion.Relative.Y * Mathf.DegToRad(Sensitivity));

            state.Camera.Rotation.Y = pivotX.Rotation.X;
            state.Camera.Rotation.X = pivotY.Rotation.Y;
        }
        else if (Input.IsMouseButtonPressed(MouseButton.Middle))
        {
            Pan += ((new Vector2(-motion.Relative.X, motion.Relative.Y))  * Mathf.DegToRad(Sensitivity));
        }
        
        
    }
}