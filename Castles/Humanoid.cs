using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Castles
{
    class Humanoid : Entity, IUpdatable
    {

        private ShaderProgram entityShader = Shaders.GetShader("Entity");

        private Entity head, helm, chest;
        private Entity[] shoulders, feet, hands;

        public Model Head { set { head = new Entity(value, head); } }
        public Model Helm { set { helm = new Entity(value, helm); } }
        public Model Chest { set { chest = new Entity(value, chest); } }
        public Model[] Shoulders { set { shoulders[0] = new Entity(value[0], shoulders[0]); shoulders[1] = new Entity(value[1], shoulders[1]); } }
        public Model[] Feet { set { feet[0] = new Entity(value[0], feet[0]); feet[1] = new Entity(value[1], feet[1]); } }
        public Model[] Hands { set { hands[0] = new Entity(value[0], hands[0]); hands[1] = new Entity(value[1], hands[1]); } }



        public Humanoid(string head, string helm, string chest, string shoulder, string shoulder2, string foot, string foot2, string hand, string hand2) : base(null)
        {
            this.head = new Entity(Loader.LoadModel(head, entityShader), parent: this);
            this.helm = new Entity(Loader.LoadModel(helm, entityShader), parent: this);
            this.chest = new Entity(Loader.LoadModel(chest, entityShader), parent: this);
            shoulders = new Entity[] { new Entity(Loader.LoadModel(shoulder, entityShader), parent: this), new Entity(Loader.LoadModel(shoulder2, entityShader), parent: this) };
            feet = new Entity[] { new Entity(Loader.LoadModel(foot, entityShader), parent: this), new Entity(Loader.LoadModel(foot2, entityShader), parent: this) };
            hands = new Entity[] { new Entity(Loader.LoadModel(hand, entityShader), parent: this), new Entity(Loader.LoadModel(hand2, entityShader), parent: this) };
        }

        public Humanoid() : this("!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube")
        {

        }


        float chestHeight = 20f;
        public void Update(float delta)
        {

        }
    }
}
