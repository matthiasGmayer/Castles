using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OpenTK.Graphics.OpenGL;

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
        }

        public Humanoid() : this("!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube", "!Cube")
        {

        }


        float chestHeight = 20f;
        public void Update(float delta)
        {

        }
    }
    /*
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

    public class AnimatedModel
    {

        // skin
    private readonly Vao model;
	private readonly Texture texture;

	// skeleton
	private readonly Joint rootJoint;
	private readonly int jointCount;

    private readonly Animator animator;

	public AnimatedModel(Vao model, Texture texture, Joint rootJoint, int jointCount)
        {
            this.model = model;
            this.texture = texture;
            this.rootJoint = rootJoint;
            this.jointCount = jointCount;
            this.animator = new Animator(this);
            rootJoint.calcInverseBindTransform(new Matrix4());
        }


        public Vao getModel()
        {
            return model;
        }

        public Texture getTexture()
        {
            return texture;
        }

        public Joint getRootJoint()
        {
            return rootJoint;
        }

        public void delete()
        {
            model.delete();
            texture.delete();
        }

        public void doAnimation(Animation animation)
        {
            animator.doAnimation(animation);
        }

        public void update()
        {
            animator.update();
        }

        public Matrix4[] getJointTransforms()
        {
            Matrix4[] jointMatrices = new Matrix4[jointCount];
            addJointsToArray(rootJoint, jointMatrices);
            return jointMatrices;
        }

        private void addJointsToArray(Joint headJoint, Matrix4[] jointMatrices)
        {
            jointMatrices[headJoint.index] = headJoint.getAnimatedTransform();
            foreach (Joint childJoint in headJoint.children)
            {
                addJointsToArray(childJoint, jointMatrices);
            }
        }

    }

    public class Animation
    {

        private readonly float length;//in seconds
        private readonly KeyFrame[] keyFrames;


	public Animation(float lengthInSeconds, KeyFrame[] frames)
        {
            this.keyFrames = frames;
            this.length = lengthInSeconds;
        }

        public float getLength()
        {
            return length;
        }

        public KeyFrame[] getKeyFrames()
        {
            return keyFrames;
        }

    }

    public class JointTransform
    {

    private readonly Vector3 position;
	private readonly Quaternion rotation;

	public JointTransform(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        protected Matrix4 getLocalTransform()
        {
            Matrix4 matrix = new Matrix4();
            matrix.translate(position);
            Matrix4.mul(matrix * rotation.toRotationMatrix(), matrix);
            return matrix;
        }

        protected static JointTransform interpolate(JointTransform frameA, JointTransform frameB, float progression)
        {
            Vector3 pos = interpolate(frameA.position, frameB.position, progression);
            Quaternion rot = Quaternion.interpolate(frameA.rotation, frameB.rotation, progression);
            return new JointTransform(pos, rot);
        }

        private static Vector3 interpolate(Vector3 start, Vector3 end, float progression)
        {
            float x = start.x + (end.x - start.x) * progression;
            float y = start.y + (end.y - start.y) * progression;
            float z = start.z + (end.z - start.z) * progression;
            return new Vector3(x, y, z);
        }

    }

    public class KeyFrame
    {

        private readonly float timeStamp;
        private readonly Map<String, JointTransform> pose;

        public KeyFrame(float timeStamp, Map<String, JointTransform> jointKeyFrames)
        {
            this.timeStamp = timeStamp;
            this.pose = jointKeyFrames;
        }

        protected float getTimeStamp()
        {
            return timeStamp;
        }

        protected Map<String, JointTransform> getJointKeyFrames()
        {
            return pose;
        }

    }

    public class Quaternion
    {

        private float x, y, z, w;

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            normalize();
        }

        public void normalize()
        {
            float mag = (float)Math.Sqrt(w * w + x * x + y * y + z * z);
            w /= mag;
            x /= mag;
            y /= mag;
            z /= mag;
        }

        public Matrix4 toRotationMatrix()
        {
            Matrix4 matrix = new Matrix4();
            float xy = x * y;
            float xz = x * z;
            float xw = x * w;
            float yz = y * z;
            float yw = y * w;
            float zw = z * w;
            float xSquared = x * x;
            float ySquared = y * y;
            float zSquared = z * z;
            matrix.m00 = 1 - 2 * (ySquared + zSquared);
            matrix.m01 = 2 * (xy - zw);
            matrix.m02 = 2 * (xz + yw);
            matrix.m03 = 0;
            matrix.m10 = 2 * (xy + zw);
            matrix.m11 = 1 - 2 * (xSquared + zSquared);
            matrix.m12 = 2 * (yz - xw);
            matrix.m13 = 0;
            matrix.m20 = 2 * (xz - yw);
            matrix.m21 = 2 * (yz + xw);
            matrix.m22 = 1 - 2 * (xSquared + ySquared);
            matrix.m23 = 0;
            matrix.m30 = 0;
            matrix.m31 = 0;
            matrix.m32 = 0;
            matrix.m33 = 1;
            return matrix;
        }

        public static Quaternion fromMatrix(Matrix4 matrix)
        {
            float w, x, y, z;
            float diagonal = matrix.m00 + matrix.m11 + matrix.m22;
            if (diagonal > 0)
            {
                float w4 = (float)(Math.Sqrt(diagonal + 1f) * 2f);
                w = w4 / 4f;
                x = (matrix.m21 - matrix.m12) / w4;
                y = (matrix.m02 - matrix.m20) / w4;
                z = (matrix.m10 - matrix.m01) / w4;
            }
            else if ((matrix.m00 > matrix.m11) && (matrix.m00 > matrix.m22))
            {
                float x4 = (float)(Math.Sqrt(1f + matrix.m00 - matrix.m11 - matrix.m22) * 2f);
                w = (matrix.m21 - matrix.m12) / x4;
                x = x4 / 4f;
                y = (matrix.m01 + matrix.m10) / x4;
                z = (matrix.m02 + matrix.m20) / x4;
            }
            else if (matrix.m11 > matrix.m22)
            {
                float y4 = (float)(Math.Sqrt(1f + matrix.m11 - matrix.m00 - matrix.m22) * 2f);
                w = (matrix.m02 - matrix.m20) / y4;
                x = (matrix.m01 + matrix.m10) / y4;
                y = y4 / 4f;
                z = (matrix.m12 + matrix.m21) / y4;
            }
            else
            {
                float z4 = (float)(Math.Sqrt(1f + matrix.m22 - matrix.m00 - matrix.m11) * 2f);
                w = (matrix.m10 - matrix.m01) / z4;
                x = (matrix.m02 + matrix.m20) / z4;
                y = (matrix.m12 + matrix.m21) / z4;
                z = z4 / 4f;
            }
            return new Quaternion(x, y, z, w);
        }

        public static Quaternion interpolate(Quaternion a, Quaternion b, float blend)
        {
            Quaternion result = new Quaternion(0, 0, 0, 1);
            float dot = a.w * b.w + a.x * b.x + a.y * b.y + a.z * b.z;
            float blendI = 1f - blend;
            if (dot < 0)
            {
                result.w = blendI * a.w + blend * -b.w;
                result.x = blendI * a.x + blend * -b.x;
                result.y = blendI * a.y + blend * -b.y;
                result.z = blendI * a.z + blend * -b.z;
            }
            else
            {
                result.w = blendI * a.w + blend * b.w;
                result.x = blendI * a.x + blend * b.x;
                result.y = blendI * a.y + blend * b.y;
                result.z = blendI * a.z + blend * b.z;
            }
            result.normalize();
            return result;
        }

    }

    public class AnimatedModelLoader
    {

        public static AnimatedModel loadEntity(MyFile modelFile, MyFile textureFile)
        {
            AnimatedModelData entityData = ColladaLoader.loadColladaModel(modelFile, GeneralSettings.MAX_WEIGHTS);
            Vao model = createVao(entityData.getMeshData());
            Texture texture = loadTexture(textureFile);
            SkeletonData skeletonData = entityData.getJointsData();
            Joint headJoint = createJoints(skeletonData.headJoint);
            return new AnimatedModel(model, texture, headJoint, skeletonData.jointCount);
        }

        private static Texture loadTexture(MyFile textureFile)
        {
            Texture diffuseTexture = Texture.newTexture(textureFile).anisotropic().create();
            return diffuseTexture;
        }

        private static Joint createJoints(JointData data)
        {
            Joint joint = new Joint(data.index, data.nameId, data.bindLocalTransform);
            foreach (JointData child in data.children)
            {
                joint.addChild(createJoints(child));
            }
            return joint;
        }

        private static Vao createVao(MeshData data)
        {
            Vao vao = Vao.create();
            vao.bind();
            vao.createIndexBuffer(data.getIndices());
            vao.createAttribute(0, data.getVertices(), 3);
            vao.createAttribute(1, data.getTextureCoords(), 2);
            vao.createAttribute(2, data.getNormals(), 3);
            vao.createIntAttribute(3, data.getJointIds(), 3);
            vao.createAttribute(4, data.getVertexWeights(), 3);
            vao.unbind();
            return vao;
        }

    }

    public class AnimationLoader
    {

        public static Animation loadAnimation(MyFile colladaFile)
        {
            AnimationData animationData = ColladaLoader.loadColladaAnimation(colladaFile);
            KeyFrame[] frames = new KeyFrame[animationData.keyFrames.length];
            for (int i = 0; i < frames.length; i++)
            {
                frames[i] = createKeyFrame(animationData.keyFrames[i]);
            }
            return new Animation(animationData.lengthSeconds, frames);
        }

        private static KeyFrame createKeyFrame(KeyFrameData data)
        {
            Map<String, JointTransform> map = new HashMap<String, JointTransform>();
            foreach (JointTransformData jointData in data.jointTransforms)
            {
                JointTransform jointTransform = createTransform(jointData);
                map.put(jointData.jointNameId, jointTransform);
            }
            return new KeyFrame(data.time, map);
        }

        private static JointTransform createTransform(JointTransformData data)
        {
            Matrix4 mat = data.jointLocalTransform;
            Vector3 translation = new Vector3(mat.m30, mat.m31, mat.m32);
            Quaternion rotation = Quaternion.fromMatrix(mat);
            return new JointTransform(translation, rotation);
        }

    }

    public class Vao
    {

        private static readonly int BYTES_PER_FLOAT = 4;
        private static readonly int BYTES_PER_INT = 4;
        public readonly int id;
        private List<Vbo> dataVbos = new List<Vbo>();
        private Vbo indexVbo;
        private int indexCount;

        public static Vao create()
        {
            int id = GL30.glGenVertexArrays();
            return new Vao(id);
        }

        private Vao(int id)
        {
            this.id = id;
        }

        public int getIndexCount()
        {
            return indexCount;
        }

        public void bind(int attributes)
        {
            bind();
            foreach (int i in attributes)
            {
                GL20.glEnableVertexAttribArray(i);
            }
        }

        public void unbind(int attributes)
        {
            foreach (int i in attributes)
            {
                GL20.glDisableVertexAttribArray(i);
            }
            unbind();
        }

        public void createIndexBuffer(int[] indices)
        {
            this.indexVbo = Vbo.create(GL15.GL_ELEMENT_ARRAY_BUFFER);
            indexVbo.bind();
            indexVbo.storeData(indices);
            this.indexCount = indices.length;
        }

        public void createAttribute(int attribute, float[] data, int attrSize)
        {
            Vbo dataVbo = Vbo.create(GL15.GL_ARRAY_BUFFER);
            dataVbo.bind();
            dataVbo.storeData(data);
            GL20.glVertexAttribPointer(attribute, attrSize, GL11.GL_FLOAT, false, attrSize * BYTES_PER_FLOAT, 0);
            dataVbo.unbind();
            dataVbos.Add(dataVbo);
        }

        public void createIntAttribute(int attribute, int[] data, int attrSize)
        {
            Vbo dataVbo = Vbo.create(GL15.GL_ARRAY_BUFFER);
            dataVbo.bind();
            dataVbo.storeData(data);
            GL30.glVertexAttribIPointer(attribute, attrSize, GL11.GL_INT, attrSize * BYTES_PER_INT, 0);
            dataVbo.unbind();
            dataVbos.Add(dataVbo);
        }

        public void delete()
        {
            GL30.glDeleteVertexArrays(id);
            foreach (Vbo vbo in dataVbos)
            {
                vbo.delete();
            }
            indexVbo.delete();
        }

        private void bind()
        {
            GL30.glBindVertexArray(id);
        }

        private void unbind()
        {
            GL30.glBindVertexArray(0);
        }

    }

    public class Vbo
    {

        private readonly int vboId;
        private readonly int type;

        private Vbo(int vboId, int type)
        {
            this.vboId = vboId;
            this.type = type;
        }

        public static Vbo create(int type)
        {
            int id = GL15.glGenBuffers();
            return new Vbo(id, type);
        }

        public void bind()
        {
            GL15.glBindBuffer(type, vboId);
        }

        public void unbind()
        {
            GL15.glBindBuffer(type, 0);
        }

        public void storeData(float[] data)
        {
            FloatBuffer buffer = BufferUtils.createFloatBuffer(data.length);
            buffer.put(data);
            buffer.flip();
            storeData(buffer);
        }

        public void storeData(int[] data)
        {
            IntBuffer buffer = BufferUtils.createIntBuffer(data.length);
            buffer.put(data);
            buffer.flip();
            storeData(buffer);
        }

        public void storeData(IntBuffer data)
        {
            GL15.glBufferData(type, data, GL15.GL_STATIC_DRAW);
        }

        public void storeData(FloatBuffer data)
        {
            GL15.glBufferData(type, data, GL15.GL_STATIC_DRAW);
        }

        public void delete()
        {
            GL15.glDeleteBuffers(vboId);
        }

    }

    public class AnimationLoader
    {

        private static readonly Matrix4 CORRECTION = new Matrix4().Rotate((float) Math.toRadians(-90), new Vector3(1, 0, 0));
	
	private XmlNode animationData;
        private XmlNode jointHierarchy;

        public AnimationLoader(XmlNode animationData, XmlNode jointHierarchy)
        {
            this.animationData = animationData;
            this.jointHierarchy = jointHierarchy;
        }

        public AnimationData extractAnimation()
        {
            String rootNode = findRootJointName();
            float[] times = getKeyTimes();
            float duration = times[times.length - 1];
            KeyFrameData[] keyFrames = initKeyFrames(times);
            List<XmlNode> animationNodes = animationData.getChildren("animation");
            foreach (XmlNode jointNode in animationNodes)
            {
                loadJointTransforms(keyFrames, jointNode, rootNode);
            }
            return new AnimationData(duration, keyFrames);
        }

        private float[] getKeyTimes()
        {
            XmlNode timeData = animationData.getChild("animation").getChild("source").getChild("float_array");
            String[] rawTimes = timeData.getData().Split(" ");
            float[] times = new float[rawTimes.length];
            for (int i = 0; i < times.length; i++)
            {
                times[i] = Float.parseFloat(rawTimes[i]);
            }
            return times;
        }

        private KeyFrameData[] initKeyFrames(float[] times)
        {
            KeyFrameData[] frames = new KeyFrameData[times.length];
            for (int i = 0; i < frames.length; i++)
            {
                frames[i] = new KeyFrameData(times[i]);
            }
            return frames;
        }

        private void loadJointTransforms(KeyFrameData[] frames, XmlNode jointData, String rootNodeId)
        {
            String jointNameId = getJointName(jointData);
            String dataId = getDataId(jointData);
            XmlNode transformData = jointData.getChildWithAttribute("source", "id", dataId);
            String[] rawData = transformData.getChild("float_array").getData().Split(' ');
            processTransforms(jointNameId, rawData, frames, jointNameId.Equals(rootNodeId));
        }

        private void processTransforms(string jointNameId, string[] rawData, KeyFrameData[] frames, bool v)
        {
            throw new NotImplementedException();
        }

        private String getDataId(XmlNode jointData)
        {
            XmlNode node = jointData.getChild("sampler").getChildWithAttribute("input", "semantic", "OUTPUT");
            return node.getAttribute("source").Substring(1);
        }

        private String getJointName(XmlNode jointData)
        {
            XmlNode channelNode = jointData.getChild("channel");
            String data = channelNode.getAttribute("target");
            return data.Split('/')[0];
        }

        private void processTransforms(String jointName, String[] rawData, KeyFrameData[] keyFrames, bool root)
        {
            NvFloatBuffer buffer = BufferUtils.createFloatBuffer(16);
            float[] matrixData = new float[16];
            for (int i = 0; i < keyFrames.Length; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    matrixData[j] = float.ParseFloat(rawData[i * 16 + j]);
                }
                buffer.Clear();
                buffer.Put(matrixData);
                buffer.Flip();
                Matrix4 transform = new Matrix4();
                transform.Load(buffer);
                transform.Transpose();
                if (root)
                {
                    //because up axis in Blender is different to up axis in game
                    Matrix4f.mul(CORRECTION, transform, transform);
                }
                keyFrames[i].addJointTransform(new JointTransformData(jointName, transform));
            }
        }

        private String findRootJointName()
        {
            XmlNode skeleton = jointHierarchy.getChild("visual_scene").getChildWithAttribute("node", "id", "Armature");
            return skeleton.getChild("node").getAttribute("id");
        }


    }

    public class ColladaLoader
    {

        public static AnimatedModelData loadColladaModel(MyFile colladaFile, int maxWeights)
        {
            XmlNode node = XmlParser.loadXmlFile(colladaFile);

            SkinLoader skinLoader = new SkinLoader(node.getChild("library_controllers"), maxWeights);
            SkinningData skinningData = skinLoader.extractSkinData();

            SkeletonLoader jointsLoader = new SkeletonLoader(node.getChild("library_visual_scenes"), skinningData.jointOrder);
            SkeletonData jointsData = jointsLoader.extractBoneData();

            GeometryLoader g = new GeometryLoader(node.getChild("library_geometries"), skinningData.VerticesSkinData);
            MeshData meshData = g.extractModelData();

            return new AnimatedModelData(meshData, jointsData);
        }

        public static AnimationData loadColladaAnimation(MyFile colladaFile)
        {
            XmlNode node = XmlParser.loadXmlFile(colladaFile);
            XmlNode animNode = node.getChild("library_animations");
            XmlNode jointsNode = node.getChild("library_visual_scenes");
            AnimationLoader loader = new AnimationLoader(animNode, jointsNode);
            AnimationData animData = loader.extractAnimation();
            return animData;
        }

    }

    public class GeometryLoader
    {

        private static readonly Matrix4 CORRECTION = new Matrix4().Rotate((float) Math.ToRadians(-90), new Vector3(1, 0,0));
	
	private readonly XmlNode meshData;

	private readonly List<VertexSkinData> vertexWeights;

        private float[] verticesArray;
        private float[] normalsArray;
        private float[] texturesArray;
        private int[] indicesArray;
        private int[] jointIdsArray;
        private float[] weightsArray;

        List<Vertex> vertices = new List<Vertex>();
        List<Vector2> textures = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        public GeometryLoader(XmlNode geometryNode, List<VertexSkinData> vertexWeights)
        {
            this.vertexWeights = vertexWeights;
            this.meshData = geometryNode.getChild("geometry").getChild("mesh");
        }

        public MeshData extractModelData()
        {
            readRawData();
            assembleVertices();
            removeUnusedVertices();
            initArrays();
            convertDataToArrays();
            convertIndicesListToArray();
            return new MeshData(verticesArray, texturesArray, normalsArray, indicesArray, jointIdsArray, weightsArray);
        }

        private void readRawData()
        {
            readPositions();
            readNormals();
            readTextureCoords();
        }

        private void readPositions()
        {
            String positionsId = meshData.getChild("vertices").getChild("input").getAttribute("source").substring(1);
            XmlNode positionsData = meshData.getChildWithAttribute("source", "id", positionsId).getChild("float_array");
            int count = int.Parse(positionsData.getAttribute("count"));
            String[] posData = positionsData.getData().Split(' ');
            for (int i = 0; i < count / 3; i++)
            {
                float x = float.Parse(posData[i * 3]);
                float y = float.Parse(posData[i * 3 + 1]);
                float z = float.Parse(posData[i * 3 + 2]);
                Vector4 position = new Vector4(x, y, z, 1);
                Matrix4.Transform(CORRECTION, position, position);
                vertices.Add(new Vertex(vertices.Size(), new Vector3(position.x, position.y, position.z), vertexWeights.Get(vertices.Size())));
            }
        }

        private void readNormals()
        {
            String normalsId = meshData.getChild("polylist").getChildWithAttribute("input", "semantic", "NORMAL")
                    .getAttribute("source").Substring(1);
            XmlNode normalsData = meshData.getChildWithAttribute("source", "id", normalsId).getChild("float_array");
            int count = int.Parse(normalsData.getAttribute("count"));
            String[] normData = normalsData.getData().Split(' ');
            for (int i = 0; i < count / 3; i++)
            {
                float x = float.Parse(normData[i * 3]);
                float y = float.Parse(normData[i * 3 + 1]);
                float z = float.Parse(normData[i * 3 + 2]);
                Vector4 norm = new Vector4(x, y, z, 0f);
                Matrix4.transform(CORRECTION, norm, norm);
                normals.Add(new Vector3(norm.x, norm.y, norm.z));
            }
        }

        private void readTextureCoords()
        {
            String texCoordsId = meshData.getChild("polylist").getChildWithAttribute("input", "semantic", "TEXCOORD")
                    .getAttribute("source").Substring(1);
            XmlNode texCoordsData = meshData.getChildWithAttribute("source", "id", texCoordsId).getChild("float_array");
            int count = int.Parse(texCoordsData.getAttribute("count"));
            String[] texData = texCoordsData.getData().Split(' ');
            for (int i = 0; i < count / 2; i++)
            {
                float s = float.Parse(texData[i * 2]);
                float t = float.Parse(texData[i * 2 + 1]);
                textures.Add(new Vector2(s, t));
            }
        }

        private void assembleVertices()
        {
            XmlNode poly = meshData.getChild("polylist");
            int typeCount = poly.getChildren("input").Size();
            String[] indexData = poly.getChild("p").getData().Split(' ');
            for (int i = 0; i < indexData.Length / typeCount; i++)
            {
                int positionIndex = int.Parse(indexData[i * typeCount]);
                int normalIndex = int.Parse(indexData[i * typeCount + 1]);
                int texCoordIndex = int.Parse(indexData[i * typeCount + 2]);
                processVertex(positionIndex, normalIndex, texCoordIndex);
            }
        }


        private Vertex processVertex(int posIndex, int normIndex, int texIndex)
        {
            Vertex currentVertex = vertices.Get(posIndex);
            if (!currentVertex.isSet())
            {
                currentVertex.setTextureIndex(texIndex);
                currentVertex.setNormalIndex(normIndex);
                indices.Add(posIndex);
                return currentVertex;
            }
            else
            {
                return dealWithAlreadyProcessedVertex(currentVertex, texIndex, normIndex);
            }
        }

        private int[] convertIndicesListToArray()
        {
            this.indicesArray = new int[indices.Size()];
            for (int i = 0; i < indicesArray.Length; i++)
            {
                indicesArray[i] = indices.Get(i);
            }
            return indicesArray;
        }

        private float convertDataToArrays()
        {
            float furthestPoint = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                Vertex currentVertex = vertices.get(i);
                if (currentVertex.getLength() > furthestPoint)
                {
                    furthestPoint = currentVertex.getLength();
                }
                Vector3f position = currentVertex.getPosition();
                Vector2f textureCoord = textures.get(currentVertex.getTextureIndex());
                Vector3f normalVector = normals.get(currentVertex.getNormalIndex());
                verticesArray[i * 3] = position.x;
                verticesArray[i * 3 + 1] = position.y;
                verticesArray[i * 3 + 2] = position.z;
                texturesArray[i * 2] = textureCoord.x;
                texturesArray[i * 2 + 1] = 1 - textureCoord.y;
                normalsArray[i * 3] = normalVector.x;
                normalsArray[i * 3 + 1] = normalVector.y;
                normalsArray[i * 3 + 2] = normalVector.z;
                VertexSkinData weights = currentVertex.getWeightsData();
                jointIdsArray[i * 3] = weights.jointIds.get(0);
                jointIdsArray[i * 3 + 1] = weights.jointIds.get(1);
                jointIdsArray[i * 3 + 2] = weights.jointIds.get(2);
                weightsArray[i * 3] = weights.weights.get(0);
                weightsArray[i * 3 + 1] = weights.weights.get(1);
                weightsArray[i * 3 + 2] = weights.weights.get(2);

            }
            return furthestPoint;
        }

        private Vertex dealWithAlreadyProcessedVertex(Vertex previousVertex, int newTextureIndex, int newNormalIndex)
        {
            if (previousVertex.hasSameTextureAndNormal(newTextureIndex, newNormalIndex))
            {
                indices.add(previousVertex.getIndex());
                return previousVertex;
            }
            else
            {
                Vertex anotherVertex = previousVertex.getDuplicateVertex();
                if (anotherVertex != null)
                {
                    return dealWithAlreadyProcessedVertex(anotherVertex, newTextureIndex, newNormalIndex);
                }
                else
                {
                    Vertex duplicateVertex = new Vertex(vertices.Count, previousVertex.getPosition(), previousVertex.getWeightsData());
                    duplicateVertex.setTextureIndex(newTextureIndex);
                    duplicateVertex.setNormalIndex(newNormalIndex);
                    previousVertex.setDuplicateVertex(duplicateVertex);
                    vertices.add(duplicateVertex);
                    indices.add(duplicateVertex.getIndex());
                    return duplicateVertex;
                }

            }
        }

        private void initArrays()
        {
            this.verticesArray = new float[vertices.Count * 3];
            this.texturesArray = new float[vertices.Count * 2];
            this.normalsArray = new float[vertices.Count * 3];
            this.jointIdsArray = new int[vertices.Count * 3];
            this.weightsArray = new float[vertices.Count * 3];
        }

        private void removeUnusedVertices()
        {
            foreach (Vertex vertex in vertices)
            {
                vertex.averageTangents();
                if (!vertex.isSet())
                {
                    vertex.setTextureIndex(0);
                    vertex.setNormalIndex(0);
                }
            }
        }

    }

    public class SkeletonLoader
    {

        private XmlNode armatureData;

        private List<String> boneOrder;

        private int jointCount = 0;

        private static readonly Matrix4 CORRECTION = new Matrix4().rotate((float) Math.toRadians(-90), new Vector3f(1, 0, 0));

	public SkeletonLoader(XmlNode visualSceneNode, List<String> boneOrder)
        {
            this.armatureData = visualSceneNode.getChild("visual_scene").getChildWithAttribute("node", "id", "Armature");
            this.boneOrder = boneOrder;
        }

        public SkeletonData extractBoneData()
        {
            XmlNode headNode = armatureData.getChild("node");
            JointData headJoint = loadJointData(headNode, true);
            return new SkeletonData(jointCount, headJoint);
        }

        private JointData loadJointData(XmlNode jointNode, boolean isRoot)
        {
            JointData joint = extractMainJointData(jointNode, isRoot);
            for (XmlNode childNode : jointNode.getChildren("node"))
            {
                joint.addChild(loadJointData(childNode, false));
            }
            return joint;
        }

        private JointData extractMainJointData(XmlNode jointNode, boolean isRoot)
        {
            String nameId = jointNode.getAttribute("id");
            int index = boneOrder.indexOf(nameId);
            String[] matrixData = jointNode.getChild("matrix").getData().split(" ");
            Matrix4f matrix = new Matrix4f();
            matrix.load(convertData(matrixData));
            matrix.transpose();
            if (isRoot)
            {
                //because in Blender z is up, but in our game y is up.
                Matrix4f.mul(CORRECTION, matrix, matrix);
            }
            jointCount++;
            return new JointData(index, nameId, matrix);
        }

        private FloatBuffer convertData(String[] rawData)
        {
            float[] matrixData = new float[16];
            for (int i = 0; i < matrixData.length; i++)
            {
                matrixData[i] = Float.parseFloat(rawData[i]);
            }
            FloatBuffer buffer = BufferUtils.createFloatBuffer(16);
            buffer.put(matrixData);
            buffer.flip();
            return buffer;
        }

    }

    public class SkinLoader
    {

        private final XmlNode skinningData;
	private final int maxWeights;

        public SkinLoader(XmlNode controllersNode, int maxWeights)
        {
            this.skinningData = controllersNode.getChild("controller").getChild("skin");
            this.maxWeights = maxWeights;
        }

        public SkinningData extractSkinData()
        {
            List<String> jointsList = loadJointsList();
            float[] weights = loadWeights();
            XmlNode weightsDataNode = skinningData.getChild("vertex_weights");
            int[] effectorJointCounts = getEffectiveJointsCounts(weightsDataNode);
            List<VertexSkinData> vertexWeights = getSkinData(weightsDataNode, effectorJointCounts, weights);
            return new SkinningData(jointsList, vertexWeights);
        }

        private List<String> loadJointsList()
        {
            XmlNode inputNode = skinningData.getChild("vertex_weights");
            String jointDataId = inputNode.getChildWithAttribute("input", "semantic", "JOINT").getAttribute("source")
                    .substring(1);
            XmlNode jointsNode = skinningData.getChildWithAttribute("source", "id", jointDataId).getChild("Name_array");
            String[] names = jointsNode.getData().split(" ");
            List<String> jointsList = new ArrayList<String>();
            for (String name : names)
            {
                jointsList.add(name);
            }
            return jointsList;
        }

        private float[] loadWeights()
        {
            XmlNode inputNode = skinningData.getChild("vertex_weights");
            String weightsDataId = inputNode.getChildWithAttribute("input", "semantic", "WEIGHT").getAttribute("source")
                    .substring(1);
            XmlNode weightsNode = skinningData.getChildWithAttribute("source", "id", weightsDataId).getChild("float_array");
            String[] rawData = weightsNode.getData().split(" ");
            float[] weights = new float[rawData.length];
            for (int i = 0; i < weights.length; i++)
            {
                weights[i] = Float.parseFloat(rawData[i]);
            }
            return weights;
        }

        private int[] getEffectiveJointsCounts(XmlNode weightsDataNode)
        {
            String[] rawData = weightsDataNode.getChild("vcount").getData().split(" ");
            int[] counts = new int[rawData.length];
            for (int i = 0; i < rawData.length; i++)
            {
                counts[i] = Integer.parseInt(rawData[i]);
            }
            return counts;
        }

        private List<VertexSkinData> getSkinData(XmlNode weightsDataNode, int[] counts, float[] weights)
        {
            String[] rawData = weightsDataNode.getChild("v").getData().split(" ");
            List<VertexSkinData> skinningData = new ArrayList<VertexSkinData>();
            int pointer = 0;
            for (int count : counts)
            {
                VertexSkinData skinData = new VertexSkinData();
                for (int i = 0; i < count; i++)
                {
                    int jointId = Integer.parseInt(rawData[pointer++]);
                    int weightId = Integer.parseInt(rawData[pointer++]);
                    skinData.addJointEffect(jointId, weights[weightId]);
                }
                skinData.limitJointNumber(maxWeights);
                skinningData.add(skinData);
            }
            return skinningData;
        }

    }

    public class AnimatedModelData
    {

        private final SkeletonData joints;
	private final MeshData mesh;
	
	public AnimatedModelData(MeshData mesh, SkeletonData joints)
        {
            this.joints = joints;
            this.mesh = mesh;
        }

        public SkeletonData getJointsData()
        {
            return joints;
        }

        public MeshData getMeshData()
        {
            return mesh;
        }

    }

    public class AnimationData
    {

        public final float lengthSeconds;
        public final KeyFrameData[] keyFrames;

	public AnimationData(float lengthSeconds, KeyFrameData[] keyFrames)
        {
            this.lengthSeconds = lengthSeconds;
            this.keyFrames = keyFrames;
        }

    }

    public class JointData
    {

        public final int index;
        public final String nameId;
	public final Matrix4f bindLocalTransform;

	public final List<JointData> children = new ArrayList<JointData>();

        public JointData(int index, String nameId, Matrix4f bindLocalTransform)
        {
            this.index = index;
            this.nameId = nameId;
            this.bindLocalTransform = bindLocalTransform;
        }

        public void addChild(JointData child)
        {
            children.add(child);
        }

    }

    public class JointTransformData
    {

        public final String jointNameId;
	public final Matrix4f jointLocalTransform;

	public JointTransformData(String jointNameId, Matrix4f jointLocalTransform)
        {
            this.jointNameId = jointNameId;
            this.jointLocalTransform = jointLocalTransform;
        }
    }

    public class KeyFrameData
    {

        public final float time;
        public final List<JointTransformData> jointTransforms = new ArrayList<JointTransformData>();

        public KeyFrameData(float time)
        {
            this.time = time;
        }

        public void addJointTransform(JointTransformData transform)
        {
            jointTransforms.add(transform);
        }

    }

    public class MeshData
    {

        private static final int DIMENSIONS = 3;

        private float[] vertices;
        private float[] textureCoords;
        private float[] normals;
        private int[] indices;
        private int[] jointIds;
        private float[] vertexWeights;

        public MeshData(float[] vertices, float[] textureCoords, float[] normals, int[] indices,
                int[] jointIds, float[] vertexWeights)
        {
            this.vertices = vertices;
            this.textureCoords = textureCoords;
            this.normals = normals;
            this.indices = indices;
            this.jointIds = jointIds;
            this.vertexWeights = vertexWeights;
        }

        public int[] getJointIds()
        {
            return jointIds;
        }

        public float[] getVertexWeights()
        {
            return vertexWeights;
        }

        public float[] getVertices()
        {
            return vertices;
        }

        public float[] getTextureCoords()
        {
            return textureCoords;
        }

        public float[] getNormals()
        {
            return normals;
        }

        public int[] getIndices()
        {
            return indices;
        }

        public int getVertexCount()
        {
            return vertices.length / DIMENSIONS;
        }

    }

    public class SkeletonData
    {

        public final int jointCount;
        public final JointData headJoint;
	
	public SkeletonData(int jointCount, JointData headJoint)
        {
            this.jointCount = jointCount;
            this.headJoint = headJoint;
        }

    }

    public class SkinningData
    {

        public final List<String> jointOrder;
        public final List<VertexSkinData> verticesSkinData;

        public SkinningData(List<String> jointOrder, List<VertexSkinData> verticesSkinData)
        {
            this.jointOrder = jointOrder;
            this.verticesSkinData = verticesSkinData;
        }


    }

    public class Vertex
    {

        private static final int NO_INDEX = -1;

        private Vector3f position;
        private int textureIndex = NO_INDEX;
        private int normalIndex = NO_INDEX;
        private Vertex duplicateVertex = null;
        private int index;
        private float length;
        private List<Vector3f> tangents = new ArrayList<Vector3f>();
        private Vector3f averagedTangent = new Vector3f(0, 0, 0);


        private VertexSkinData weightsData;

        public Vertex(int index, Vector3f position, VertexSkinData weightsData)
        {
            this.index = index;
            this.weightsData = weightsData;
            this.position = position;
            this.length = position.length();
        }

        public VertexSkinData getWeightsData()
        {
            return weightsData;
        }

        public void addTangent(Vector3f tangent)
        {
            tangents.add(tangent);
        }

        public void averageTangents()
        {
            if (tangents.isEmpty())
            {
                return;
            }
            for (Vector3f tangent : tangents)
            {
                Vector3f.add(averagedTangent, tangent, averagedTangent);
            }
            averagedTangent.normalise();
        }

        public Vector3f getAverageTangent()
        {
            return averagedTangent;
        }

        public int getIndex()
        {
            return index;
        }

        public float getLength()
        {
            return length;
        }

        public boolean isSet()
        {
            return textureIndex != NO_INDEX && normalIndex != NO_INDEX;
        }

        public boolean hasSameTextureAndNormal(int textureIndexOther, int normalIndexOther)
        {
            return textureIndexOther == textureIndex && normalIndexOther == normalIndex;
        }

        public void setTextureIndex(int textureIndex)
        {
            this.textureIndex = textureIndex;
        }

        public void setNormalIndex(int normalIndex)
        {
            this.normalIndex = normalIndex;
        }

        public Vector3f getPosition()
        {
            return position;
        }

        public int getTextureIndex()
        {
            return textureIndex;
        }

        public int getNormalIndex()
        {
            return normalIndex;
        }

        public Vertex getDuplicateVertex()
        {
            return duplicateVertex;
        }

        public void setDuplicateVertex(Vertex duplicateVertex)
        {
            this.duplicateVertex = duplicateVertex;
        }

    }

    public class VertexSkinData
    {

        public readonly List<int> jointIds = new List<int>();
        public readonly List<float> weights = new List<float>();

        public void addJointEffect(int jointId, float weight)
        {
            for (int i = 0; i < weights.Count; i++)
            {
                if (weight > weights.[i])
                {
                    jointIds.Insert(i, jointId);
                    weights.Insert(i, weight);
                    return;
                }
            }
            jointIds.Add(jointId);
            weights.Add(weight);
        }

        public void limitJointNumber(int max)
        {
            if (jointIds.Count > max)
            {
                float[] topWeights = new float[max];
                float total = saveTopWeights(topWeights);
                refillWeightList(topWeights, total);
                removeExcessJointIds(max);
            }
            else if (jointIds.Count < max)
            {
                fillEmptyWeights(max);
            }
        }

        private void fillEmptyWeights(int max)
        {
            while (jointIds.Count < max)
            {
                jointIds.add(0);
                weights.add(0f);
            }
        }

        private float saveTopWeights(float[] topWeightsArray)
        {
            float total = 0;
            for (int i = 0; i < topWeightsArray.length; i++)
            {
                topWeightsArray[i] = weights.get(i);
                total += topWeightsArray[i];
            }
            return total;
        }

        private void refillWeightList(float[] topWeights, float total)
        {
            weights.clear();
            for (int i = 0; i < topWeights.length; i++)
            {
                weights.add(Math.min(topWeights[i] / total, 1));
            }
        }

        private void removeExcessJointIds(int max)
        {
            while (jointIds.Count > max)
            {
                jointIds.remove(jointIds.Count - 1);
            }
        }



    }

    public class XmlNode
    {

        private String name;
        private Map<String, String> attributes;
        private String data;
        private Map<String, List<XmlNode>> childNodes;

        protected XmlNode(String name)
        {
            this.name = name;
        }

        /**
         * @return The name of the XML node.
        // */
        //public String getName()
        //{
        //    return name;
        //}

        /**
         * @return Any text data contained between the start and end tag of the
         *         node.
         */
        //public String getData()
        //{
        //    return data;
        //}

        /**
         * Gets the value of a certain attribute of the node. Returns {@code null}
         * if the attribute doesn't exist.
         * 
         * @param attr
         *            - the name of the attribute.
         * @return The value of the attribute.
         */
        //public String getAttribute(String attr)
        //{
        //    if (attributes != null)
        //    {
        //        return attributes.get(attr);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        }

        /**
         * Gets a certain child node of this node.
         * 
         * @param childName
         *            - the name of the child node.
         * @return The child XML node with the given name.
         */
        //public XmlNode getChild(String childName)
        //{
        //    if (childNodes != null)
        //    {
        //        List<XmlNode> nodes = childNodes.get(childName);
        //        if (nodes != null && !nodes.isEmpty())
        //        {
        //            return nodes.get(0);
        //        }
        //    }
        //    return null;

        //}

        /**
         * Gets a child node with a certain name, and with a given value of a given
         * attribute. Used to get a specific child when there are multiple child
         * nodes with the same node name.
         * 
         * @param childName
         *            - the name of the child node.
         * @param attr
         *            - the attribute whose value is to be checked.
         * @param value
         *            - the value that the attribute must have.
         * @return The child node which has the correct name and the correct value
         *         for the chosen attribute.
         */
        //public XmlNode getChildWithAttribute(String childName, String attr, String value)
        //{
        //    List<XmlNode> children = getChildren(childName);
        //    if (children == null || children.isEmpty())
        //    {
        //        return null;
        //    }
        //    for (XmlNode child : children)
        //    {
        //        String val = child.getAttribute(attr);
        //        if (value.equals(val))
        //        {
        //            return child;
        //        }
        //    }
        //    return null;
        //}

        /**
         * Get the child nodes of this node that have a given name.
         * 
         * @param name
         *            - the name of the child nodes.
         * @return A list of the child nodes with the given name. If none exist then
         *         an empty list is returned.
         */
        //public List<XmlNode> getChildren(String name)
        //{
        //    if (childNodes != null)
        //    {
        //        List<XmlNode> children = childNodes.get(name);
        //        if (children != null)
        //        {
        //            return children;
        //        }
        //    }
        //    return new ArrayList<XmlNode>();
        //}

        /**
         * Adds a new attribute to this node. An attribute has a name and a value.
         * Attributes are stored in a HashMap which is initialized in here if it was
         * previously null.
         * 
         * @param attr
         *            - the name of the attribute.
         * @param value
         *            - the value of the attribute.
         */
        //protected void addAttribute(String attr, String value)
        //{
        //    if (attributes == null)
        //    {
        //        attributes = new HashMap<String, String>();
        //    }
        //    attributes.put(attr, value);
        //}

        /**
         * Adds a child node to this node.
         * 
         * @param child
         *            - the child node to add.
         */
        //protected void addChild(XmlNode child)
        //{
        //    if (childNodes == null)
        //    {
        //        childNodes = new HashMap<String, List<XmlNode>>();
        //    }
        //    List<XmlNode> list = childNodes.get(child.name);
        //    if (list == null)
        //    {
        //        list = new ArrayList<XmlNode>();
        //        childNodes.put(child.name, list);
        //    }
        //    list.add(child);
        //}

        /**
         * Sets some data for this node.
         * 
         * @param data
         *            - the data for this node (text that is found between the start
         *            and end tags of this node).
         */
 //       protected void setData(String data)
 //       {
 //           this.data = data;
 //       }

 //   }

 //   public class XmlParser
 //   {

 //       private static final Pattern DATA = Pattern.compile(">(.+?)<");
	//private static final Pattern START_TAG = Pattern.compile("<(.+?)>");
	//private static final Pattern ATTR_NAME = Pattern.compile("(.+?)=");
	//private static final Pattern ATTR_VAL = Pattern.compile("\"(.+?)\"");
	//private static final Pattern CLOSED = Pattern.compile("(</|/>)");

	/**
	 * Reads an XML file and stores all the data in {@link XmlNode} objects,
	 * allowing for easy access to the data contained in the XML file.
	 * 
	 * @param file - the XML file
	 * @return The root node of the XML structure.
	 */
	/*public static XmlNode loadXmlFile(MyFile file)
        {
            BufferedReader reader = null;
            try
            {
                reader = file.getReader();
            }
            catch (Exception e)
            {
                e.printStackTrace();
                System.err.println("Can't find the XML file: " + file.getPath());
                System.exit(0);
                return null;
            }
            try
            {
                reader.readLine();
                XmlNode node = loadNode(reader);
                reader.close();
                return node;
            }
            catch (Exception e)
            {
                e.printStackTrace();
                System.err.println("Error with XML file format for: " + file.getPath());
                System.exit(0);
                return null;
            }
        }

        private static XmlNode loadNode(BufferedReader reader) throws Exception
        {
            String line = reader.readLine().trim();
		if (line.startsWith("</")) {
			return null;
		}
		String[] startTagParts = getStartTag(line).split(" ");
        XmlNode node = new XmlNode(startTagParts[0].replace("/", ""));
        addAttributes(startTagParts, node);
        addData(line, node);
		if (CLOSED.matcher(line).find()) {
			return node;
		}
    XmlNode child = null;
		while ((child = loadNode(reader)) != null) {
			node.addChild(child);
		}
		return node;
	}

	private static void addData(String line, XmlNode node)
    {
    Matcher matcher = DATA.matcher(line);
        if (matcher.find())
        {
        node.setData(matcher.group(1));
        }
    }

    private static void addAttributes(String[] titleParts, XmlNode node)
    {
        for (int i = 1; i < titleParts.length; i++)
        {
            if (titleParts[i].contains("="))
            {
            addAttribute(titleParts[i], node);
            }
        }
    }

    private static void addAttribute(String attributeLine, XmlNode node)
    {
        Matcher nameMatch = ATTR_NAME.matcher(attributeLine);
        nameMatch.find();
        Matcher valMatch = ATTR_VAL.matcher(attributeLine);
        valMatch.find();
        node.addAttribute(nameMatch.group(1), valMatch.group(1));
    }

    private static String getStartTag(String line)
    {
        Matcher match = START_TAG.matcher(line);
        match.find();
        return match.group(1);
    }*/

    
    

