using System;
using System.Collections.Generic;
using System.Linq;
using OpenGL;
using System.Numerics;
using System.Threading.Tasks;

namespace Castles
{
    class Game
    {
        private IList<object> gameObjects = new List<object>();
        private Camera c;
        Entity player;
        ShaderProgram entityShader = Shaders.GetShader("Entity");
        Water water;
        public Game()
        {
            water = Create(new Water(new Vector3(0, 0, 0), new Vector2(10000, 10000), 1, 0.02f));
            Create(new Skybox("!Sky2"));

            Create(new DirectionalLight(new Vector3(10, -10, 0), new Vector3(1, 1, 1)));
            player = Create(new Entity(Loader.LoadModel("!Dragon", entityShader)));
            c = Create(new EntityCamera(player));
            Create(new Terrain(0, 0));
            Create(new Terrain(0, 1));
            Create(new Terrain(0, -1));
            Create(new Terrain(1, 0));
            Create(new Terrain(1, 1));
            Create(new Terrain(1, -1));
            Create(new Terrain(-1, 0));
            Create(new Terrain(-1, 1));
            Create(new Terrain(-1, -1));
        }

        float time;
        public void Update(float delta)
        {
            time += delta / 1000;
            time %= 10000;
            ManageObjects();
            ManageTerrain();
            float speed = delta * 500f;
            player.Position = new Vector3(player.Position.X, Terrain.GetHeight(player.Position.X, player.Position.Z) + 30, player.Position.Z);
            Vector3 v = new Vector3();

            if (Actions.IsPressed(OpenTK.Input.Key.S))
                v += new Vector3(1, 0, 0);
            if (Actions.IsPressed(OpenTK.Input.Key.W))
                v -= new Vector3(1, 0, 0);
            if (Actions.IsPressed(OpenTK.Input.Key.A))
                v += new Vector3(0, 0, 1);
            if (Actions.IsPressed(OpenTK.Input.Key.D))
                v -= new Vector3(0, 0, 1);


            if (v != new Vector3(0))
                player.Position += Matrix4.CreateRotationY((float)Math.PI / 2 + c.yaw) * (v / v.Length() * speed);

            foreach (IUpdatable u in gameObjects.Where(o => o is IUpdatable))
            {
                u.Update(delta);
            }

            //Console.WriteLine(1 / delta);

            if (Actions.IsPressed(OpenTK.Input.Key.Escape))
            {
                App.app.Exit();
            }

            Actions.Update(delta);
        }

        (int, int) lastTile;
        private void ManageTerrain()
        {
            (int, int) tile = Terrain.GetTerrainTile(player.Position.X, player.Position.Y);
            (int tx, int ty) = tile;


            if (!tile.Equals(lastTile))
            {
                //Terras(tx, ty);
                //Create(Terrain.GetTerrain(tx, tz));
                //Create(Terrain.GetTerrain(tx + 1, tz - 1));
                //Create(Terrain.GetTerrain(tx + 1, tz));
                //Create(Terrain.GetTerrain(tx + 1, tz + 1));
                //Create(Terrain.GetTerrain(tx, tz - 1));
                //Create(Terrain.GetTerrain(tx, tz));
                //Create(Terrain.GetTerrain(tx, tz + 1));
                //Create(Terrain.GetTerrain(tx - 1, tz - 1));
                //Create(Terrain.GetTerrain(tx - 1, tz));
                //Create(Terrain.GetTerrain(tx - 1, tz + 1));
                lastTile = tile;
            }
        }

        public async void Terras(int x, int z)
        {
            //await CreateAsync(Terrain.GetTerrain(x + 1, z));
            //await CreateAsync(Terrain.GetTerrain(x + 1, z + 1));
            //await CreateAsync(Terrain.GetTerrain(x + 1, z - 1));
        }

        public Task<T> CreateAsync<T>(T t) where T : class
        {
            return Task.Factory.StartNew(() => Create(t));
        }

        Dictionary<Model, List<object>> renderMap = new Dictionary<Model, List<object>>();
        public void Render()
        {
            SetupShaders();
            RenderGui();
            RenderTerrain();
            RenderEntites();
            RenderWater();
        }

        private void RenderWater()
        {
            Water.waterShader.Use();
            foreach (Water w in gameObjects.Where(o => o is Water))
            {
                Graphics.fbos[FrameBuffers.waterRefraction].Enable();
                //SetupShaders(new Vector4(0, -1, 0, w.Position.Y));
                RenderTerrain();
                RenderEntites(o => !(o is Skybox));
                Graphics.fbos[FrameBuffers.waterRefraction].Disable();
                Graphics.fbos[FrameBuffers.waterReflection].Enable();
                float y = c.Position.Y;
                c.pitch *= -1;
                c.Position = new Vector3(c.Position.X, 2 * w.Position.Y - y, c.Position.Z);
                SetupShaders(new Vector4(0, 1, 0, -w.Position.Y));
                RenderTerrain();
                RenderEntites();
                Graphics.fbos[FrameBuffers.waterReflection].Disable();
                c.Position = new Vector3(c.Position.X, y, c.Position.Z);
                c.pitch *= -1;
                SetupShaders();
                w.Bind();
                Gl.DrawElements(BeginMode.Triangles, Water.quad.Vao.VertexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }
        }

        private void RenderGui()
        {
            foreach (var r in renderMap)
            {
                r.Key.Bind();
                r.Key.Vao.BindAttributes(r.Key.Program);
                foreach (IGUI rend in r.Value.Where(o => o is IGUI))
                {
                    rend.Model.Program.Use();
                    Gl.DrawElements(BeginMode.Triangles, r.Key.Vao.VertexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    Gl.UseProgram(0);
                }
                Gl.BindVertexArray(0);
            }
        }

        private void SetupShaders(Vector4 clipPlane)
        {
            if (clipPlane == new Vector4(0))
                Gl.Disable(EnableCap.ClipPlane0);
            else
                Gl.Enable(EnableCap.ClipPlane0);

            Shaders.With("pointLightNumber").ForEach(s => SetLights(s));
            Shaders.With("view_matrix").ForEach(s => c.SetView(s));
            Shaders.With("rotate_view_matrix").ForEach(s => c.SetRotateView(s));
            Shaders.With("clipPlane").ForEach(s => { s.Use(); s["clipPlane"].SetValue(clipPlane); });
            Shaders.With("time").ForEach(s => { s.Use(); s["time"].SetValue(time); });
        }
        private void SetupShaders() => SetupShaders(new Vector4());

        private void RenderTerrain()
        {
            foreach (Terrain t in gameObjects.Where(o => o is Terrain))
            {
                t.Vao.Program.Use();
                t.Render();
                Gl.UseProgram(0);
            }
        }

        private void RenderEntites() => RenderEntites(s => true);
        private void RenderEntites(Predicate<IRenderable> p)
        {
            foreach (var r in renderMap)
            {
                r.Key.Bind();
                r.Key.Vao.BindAttributes(r.Key.Program);
                foreach (IRenderable rend in r.Value.Where(o => o is IRenderable))
                {
                    if (!p(rend))
                        continue;
                    r.Key.Program.Use();
                    if (rend is ITransformable t)
                        r.Key.Program["transformation_matrix"]?.SetValue(t.GetTransformationMatrix());
                    else
                        r.Key.Program["transformation_matrix"]?.SetValue(Matrix4.Identity);
                    r.Key.Program["reflectivity"]?.SetValue(r.Key.Reflectivity);
                    r.Key.Program["shineDamper"]?.SetValue(r.Key.ShineDamper);
                    Gl.DrawElements(BeginMode.Triangles, r.Key.Vao.VertexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    Gl.UseProgram(0);
                }
                Gl.BindVertexArray(0);
            }
        }

        public void SetLights(ShaderProgram program)
        {
            program.Use();
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
            Gl.UseProgram(0);
        }


        IList<object> toAdd = new List<object>();

        public T Create<T>(T e) where T : class
        {
            if (toAdd.Contains(e))
                return null;
            toAdd.Add(e);
            if (e is Entity)
                foreach (Entity child in (e as Entity).Childs)
                    Create(child);
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
                if (e is IRenderable r)
                {
                    if (r.Model == null)
                        continue;
                    if (!renderMap.ContainsKey(r.Model))
                        renderMap.Add(r.Model, new List<object>());
                    renderMap[r.Model].Add(r);
                }
                if (e is IGUI g)
                {
                    if (g.Model == null)
                        continue;
                    if (!renderMap.ContainsKey(g.Model))
                        renderMap.Add(g.Model, new List<object>());
                    renderMap[g.Model].Add(g);
                }
            }
            toAdd.Clear();

            foreach (object e in toRemove)
            {
                gameObjects.Remove(e);
                if (e is IRenderable r)
                {
                    renderMap[r.Model].Remove(r);
                }
            }
            toRemove.Clear();
        }

        internal void SetProjection(float width, float height)
        {
            Shaders.With("projection_matrix").ForEach(s =>{s.Use();s["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, width / height, 1f, Graphics.viewDistance));});
            Shaders.With("near").ForEach(s => { s.Use(); s["near"].SetValue(1f); });
            Shaders.With("far").ForEach(s => { s.Use(); s["far"].SetValue(Graphics.viewDistance); });
        }

        public void Dispose()
        {
            Texture.Dispose();
            Model.Dispose();
            Shaders.Dispose();
            Terrain.Dispose();
            Graphics.Dispose();
            foreach (IDisposable d in gameObjects.Where(o => o is IDisposable))
                d.Dispose();
        }
    }
}
