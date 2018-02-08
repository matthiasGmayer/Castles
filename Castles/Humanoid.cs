using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Castles
{
    class Humanoid : Entity, IUpdatable, IRenderable
    {

        private ShaderProgram entityShader = Shaders.GetShader("Entity");

        private Entity head, helm, chest;
        private Entity[] shoulders, feet, leg, hands;

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
            this.helm.Parent = this.head;
            this.head.Parent = this.chest;
            this.
        }

        public Humanoid() : this("!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube")
        {

        }


        float chestHeight = 20f;
        public void Update(float delta)
        {

        }
    }

    public class Joint
    {
        public readonly int index;// ID
        public readonly String name;
        public readonly List<Joint> children = new List<Joint>();

        private Matrix4 animatedTransform = new Matrix4();

        private readonly Matrix4 localBindTransform;
        private Matrix4 inverseBindTransform = new Matrix4();

        public Joint(int index, String name, Matrix4 bindLocalTransform)
        {
            this.index = index;
            this.name = name;
            this.localBindTransform = bindLocalTransform;
        }

        public void addChild(Joint child)
        {
            this.children.Add(child);
        }

        public Matrix4 getAnimatedTransform()
        {
            return animatedTransform;
        }

        public void setAnimationTransform(Matrix4 animationTransform)
        {
            this.animatedTransform = animationTransform;
        }

        public Matrix4 getInverseBindTransform()
        {
            return inverseBindTransform;
        }

        protected void calcInverseBindTransform(Matrix4 parentBindTransform)
        {
            Matrix4 bindTransform = parentBindTransform * localBindTransform;
            bindTransform.Inverse();
            inverseBindTransform.Inverse();
            foreach (Joint child in children)
            {
                child.calcInverseBindTransform(bindTransform);
            }
        }
    }
}
    