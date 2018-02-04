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
        private EntityCamera c;
        Entity d;
        Entity player;
        ShaderProgram entityShader = Shaders.GetShader("Entity");

        public Game()
        {
            Console.WriteLine(entityShader.ProgramLog);

            Create(new DirectionalLight(new Vector3(10, -10, 0), new Vector3(1, 1, 1)));
            //Create(new PointLight(new Vector3(0, 0, 0), new Vector3(1f, 0.5f, 0.5f)));
            //Create(new DirectionalLight(new Vector3(-1, 0, 1), new Vector3(1, 1, 1)));
            //d = Create(new Entity(Loader.LoadModel("!KnightSword", (Texture)null, entityShader), new Vector3(0,5,0), new Vector3(0, 0, 1.6f), 1f));
            //Create(new Entity(Loader.LoadModel("!Triangle", (Texture)null, entityShader), new Vector3(), new Vector3(0, 0, 0), 1f));
            player = Create(new Entity(Loader.LoadModel("!Dragon", (Texture)null, entityShader), new Vector3(), new Vector3(0, 0, 0), 1f));
            //d = Create(new Entity(Loader.LoadModel("!KnightSword", (Texture)null, entityShader), new Vector3(0, 5, 0), new Vector3(1.6f, 0, 0.5f), 3));
            c = Create(new EntityCamera(player));
            player.Position = new Vector3(100, 100, 100);
            c.Offset = new Vector3(0, 5, 0);
            c.Position = new Vector3(0, 5, 20);

            Create(new Terrain(0, 0));
            //Create(new Terrain(0, 1));
            //Create(new Terrain(0, -1));
            //Create(new Terrain(1, 0));
            //Create(new Terrain(1, 1));
            //Create(new Terrain(1, -1));
            //Create(new Terrain(-1, 0));
            //Create(new Terrain(-1, 1));
            //Create(new Terrain(-1, -1));
        }
        public void Update(float delta)
        {
            ManageObjects();
            ManageTerrain();

            float speed = delta * 500f;
            player.Position = new Vector3(player.Position.X, Terrain.GetHeight(player.Position.X, player.Position.Z), player.Position.Z);
            Vector3 v = new Vector3();

            if (Actions.IsPressed(OpenTK.Input.Key.S))
            {
                v += new Vector3(1, 0, 0);
            }
            if (Actions.IsPressed(OpenTK.Input.Key.W))
            {
                v -= new Vector3(1, 0, 0);
            }
            if (Actions.IsPressed(OpenTK.Input.Key.A))
            {
                v += new Vector3(0, 0, 1);
            }
            if (Actions.IsPressed(OpenTK.Input.Key.D))
            {
                v -= new Vector3(0, 0, 1);
            }


            if (v != new Vector3(0))
                player.Position += Matrix4.CreateRotationY(c.Horizontal) * (v / v.Length() * speed);
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
                Console.WriteLine("HALLO");
                Terras(tx, ty);
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

        Dictionary<Model, List<IRenderable>> entityMap = new Dictionary<Model, List<IRenderable>>();
        public void Render()
        {
            foreach (ShaderProgram shader in Shaders.GetShaders())
            {
                c.SetView(shader);
                SetLights(shader);
            }

            foreach (Terrain t in gameObjects.Where(o => o is Terrain))
            {
                t.Vao.Program.Use();
                t.Render();
                Gl.UseProgram(0);
            }

            foreach (var r in entityMap)
            {
                r.Key.Bind();
                r.Key.Vao.BindAttributes(r.Key.Program);
                foreach (IRenderable rend in r.Value)
                {

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
                    if (!entityMap.ContainsKey(r.Model))
                        entityMap.Add(r.Model, new List<IRenderable>());
                    entityMap[r.Model].Add(r);
                }
            }
            toAdd.Clear();

            foreach (object e in toRemove)
            {
                gameObjects.Remove(e);
                if (e is IRenderable r)
                {
                    entityMap[r.Model].Remove(r);
                }
            }
            toRemove.Clear();
        }

        internal void SetProjection(float width, float height)
        {
            foreach (ShaderProgram s in Shaders.GetShaders())
            {
                s.Use();
                s["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, width / height, 0.1f, 100000f));
            }
        }

        public void Dispose()
        {
            Loader.Dispose();
            Shaders.Dispose();
            Terrain.Dispose();
        }
    }
}
