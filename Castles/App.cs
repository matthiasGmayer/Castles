﻿using System;
using OpenTK;
using OpenTK.Input;
using OpenGL;
using OpenTK.Graphics;
using System.Threading;

namespace Castles
{
    class App : GameWindow
    {
        public static App app;


        Game game;
        public App() : base( 1080, 720, new GraphicsMode(32, 0, 0, 16), "Castles")
        { app = this; }

        static void Main(string[] args) => new App().Run();

        protected override void OnKeyDown(KeyboardKeyEventArgs e) => Actions.SetPressed(e.Key, true);
        protected override void OnKeyUp(KeyboardKeyEventArgs e) => Actions.SetPressed(e.Key, false);
        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            VSync = VSyncMode.Off;
            CursorVisible = false;
            Gl.Enable(EnableCap.Multisample);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);
            Gl.Enable(EnableCap.Blend);
            Gl.CullFace(CullFaceMode.Back);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
           
            Console.WriteLine(Math.Min(4f, Gl.GetFloat(GetPName.MaxTextureMaxAnisotropyExt)));
            game = new Game();
            game.SetProjection(Width, Height);
        }
        


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Gl.Viewport(0, 0, Width, Height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            game.Render();
            SwapBuffers();
        }


        protected override void OnClosed(EventArgs e) => Dispose();
        protected override void OnWindowBorderChanged(EventArgs e) => game.SetProjection(Width, Height);
        public override void Dispose() => game.Dispose();



        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            game.Update((float)e.Time);
        }

        
    }
}
