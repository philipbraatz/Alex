﻿using Alex.Common.Graphics;
using Alex.Common.Gui.Graphics;
using Alex.Common.Utils.Vectors;
using Alex.Entities;
using Microsoft.Xna.Framework;
using RocketUI;

namespace Alex.Gui.Elements.Context3D
{
    public class GuiEntityModelView : GuiContext3DElement
    {
        public override PlayerLocation TargetPosition
        {
            get { return Entity?.KnownPosition ?? new PlayerLocation(Vector3.Zero); }
            set
            {
                if (Entity != null) 
                {
                    Entity.KnownPosition = value;
                    Entity.RenderLocation = value;
                }                    
            }
        }

        private Entity _entity;

        public Entity Entity
        {
            get => _entity;
            set
            {
                
                if (!value.IsSpawned)
                {
                    value.OnSpawn();
                }
                
                _entity = value;
                Drawable = new EntityDrawable(_entity);
            }
        }

        public GuiEntityModelView(Entity entity)
        {
            Background = AlexGuiTextures.PanelGeneric;
            Entity = entity;
            Camera.CameraPositionOffset = new Vector3(0f, 1.62f, -2f);
            Camera.TargetPositionOffset = new Vector3(0f, 1.8f, 0f);
        }

        public void SetEntityRotation(float yaw, float pitch)
        {
            TargetPosition.Yaw = yaw;
            TargetPosition.Pitch = pitch;
        }

        public void SetEntityRotation(float yaw, float pitch, float headYaw)
        {
            TargetPosition.Yaw = yaw;
            TargetPosition.Pitch = pitch;
            TargetPosition.HeadYaw = headYaw;
        }

        class EntityDrawable : IGuiContext3DDrawable
        {
            public Entity Entity { get; }

            public EntityDrawable(Entity entity)
            {
                Entity = entity;
            }

            public void UpdateContext3D(IUpdateArgs args, IGuiRenderer guiRenderer)
            { 
                Entity?.Update(args);
            }

            public void DrawContext3D(IRenderArgs args, IGuiRenderer guiRenderer)
            {
                Entity?.Render(args, false);
            }
        }
    }
}