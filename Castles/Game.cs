using System;
using System.Collections.Generic;
using System.Linq;
using OpenGL;
using System.Numerics;

namespace Castles
{
    class Game
    {
        private IList<object> gameObjects = new List<object>();
        private EntityCamera c;
        Entity d;
        ShaderProgram entityShader = Shaders.GetShader("Entity");
        ShaderProgram terrainShader = Shaders.GetShader("Entity");
        Terrain t;

        public Game()
        {
            Create(new PointLight(new Vector3(0, 100, 0), new Vector3(1, 1, 1)));
            //Create(new PointLight(new Vector3(0, 0, 0), new Vector3(1f, 0.5f, 0.5f)));
            //Create(new DirectionalLight(new Vector3(-1, 0, 1), new Vector3(1, 1, 1)));
            //d = Create(new Entity(Loader.LoadModel("!KnightSword", (Texture)null, entityShader), new Vector3(0,5,0), new Vector3(0, 0, 1.6f), 1f));
            //Create(new Entity(Loader.LoadModel("!Triangle", (Texture)null, entityShader), new Vector3(), new Vector3(0, 0, 0), 1f));
            Create(new Entity(Loader.LoadModel("!Dragon", (Texture)null, entityShader), new Vector3(), new Vector3(0, 0, 0), 1f));
            d = Create(new Entity(Loader.LoadModel("!KnightSword", (Texture)null, entityShader), new Vector3(0,5,0), new Vector3(1.6f,0,0.5f), 3));
            c = Create(new EntityCamera(d));
            c.Position = new Vector3(0, 5, 20);
            //c.Offset = new Vector3(0,5,0);
            t = Create(new Terrain(terrainShader));
        }
        public void Update(float delta)
        {
            ManageObjects();
            //d.Rotation += new Vector3(delta, 0, 
            //    delta);
            //d.Rotation += new Vector3(0, delta, 0);
            if (Actions.IsPressed(OpenTK.Input.Key.S))
            {
                d.Position += new Vector3(0, 0, delta * 10f);
            }
            if (Actions.IsPressed(OpenTK.Input.Key.W))
            {
                d.Position += new Vector3(0, 0, -delta * 10f);
            }
            if (Actions.IsPressed(OpenTK.Input.Key.A))
            {
                d.Position += new Vector3(-delta * 10f,0,0);
            }
            if (Actions.IsPressed(OpenTK.Input.Key.D))
            {
                d.Position += new Vector3(delta * 10f,0,0);
            }
            foreach (IUpdatable u in gameObjects.Where(o => o is IUpdatable))
            {
                u.Update(delta);
            }

            //Console.WriteLine(1 / delta);

            if (Actions.IsPressed(OpenTK.Input.Key.Escape))
            {
                App.app.Exit();
            }
        }
        public void Render()
        {
            c.SetView(entityShader);
            SetLights(entityShader);
            foreach (Entity e in gameObjects.Where(x => x is Entity))
            {
                e.Render();
            }

            t.Render();
        }


        public void SetLights(ShaderProgram program)
        {
            int i = 0;
            foreach (PointLight l in gameObjects.Where(x => x is PointLight))
            {
                l.SetUniforms(i, program);
                i++;
            }
            int j = 0;
            foreach (DirectionalLight l in gameObjects.Where(x => x is DirectionalLight))
            {
                l.SetUniforms(j, program);
                j++;
            }
            program["pointLightNumber"].SetValue(i);
            program["dirLightNumber"].SetValue(j);
            
        }


        IList<object> toAdd = new List<object>();

        public T Create<T>(T e)
        {
            toAdd.Add(e);
            return e;
        }

        IList<object> toRemove = new List<object>();

        public void Remove(object e)
        {
            toRemove.Add(e);
        }

        protected virtual void ManageObjects()
        {
            foreach (object e in toAdd)
            {
                gameObjects.Add(e);
            }
            toAdd.Clear();

            foreach (object e in toRemove)
            {
                gameObjects.Remove(e);
            }
            toRemove.Clear();
        }

        internal void SetProjection(float width, float height)
        {
            entityShader.Use();
            entityShader["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, width / height, 0.1f, 1000f));
        }

        public void Dispose()
        {
            foreach (IDisposable d in gameObjects.Where(x => x is IDisposable))
            {
                d.Dispose();
            }
            entityShader.DisposeChildren = true;
            entityShader.Dispose();
        }
    }
}
