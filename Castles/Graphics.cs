using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;


namespace Castles
{
    public enum FrameBuffers
    {
        waterReflection,
        waterRefraction,
        skyBox,
        waterDepth,
        GaussianBlur,
        GaussianBlur1,
        GaussianBlurHorizontal,
        GaussianBlurHorizontal1,
        GaussianBlur2,
        GaussianBlurHorizontal2
    }

    public static class Graphics
    {
        public static Dictionary<FrameBuffers, FBO> fbos = new Dictionary<FrameBuffers, FBO>();
        public static float viewDistance = 10000f;

        static Graphics()
        {
            fbos.Add(FrameBuffers.waterReflection, new FBO(new Size(Width, Height),
                new FramebufferAttachment[] { FramebufferAttachment.ColorAttachment0 },
                PixelInternalFormat.Rgba));
            fbos.Add(FrameBuffers.waterRefraction, new FBO(new Size(Width, Height),
                new FramebufferAttachment[] { FramebufferAttachment.ColorAttachment0 },
                PixelInternalFormat.Rgba));
            fbos.Add(FrameBuffers.waterDepth, new FBO(new Size(Width, Height),
                new FramebufferAttachment[] { FramebufferAttachment.ColorAttachment0 },
                PixelInternalFormat.Rgba));
            fbos.Add(FrameBuffers.skyBox, new FBO(new Size(Width, Height),
                new FramebufferAttachment[] { FramebufferAttachment.ColorAttachment0 },
                PixelInternalFormat.Rgba));
            fbos.Add(FrameBuffers.GaussianBlurHorizontal, new FBO(new Size(Width, Height),
                new FramebufferAttachment[] { FramebufferAttachment.ColorAttachment0 },
                PixelInternalFormat.Rgba));
            fbos.Add(FrameBuffers.GaussianBlur1, new FBO(new Size(Width / 8, Height / 8),
                new FramebufferAttachment[] { FramebufferAttachment.ColorAttachment0 },
                PixelInternalFormat.Rgba));
            fbos.Add(FrameBuffers.GaussianBlurHorizontal1, new FBO(new Size(Width / 8, Height / 8),
                new FramebufferAttachment[] { FramebufferAttachment.ColorAttachment0 },
                PixelInternalFormat.Rgba));
            fbos.Add(FrameBuffers.GaussianBlur, new FBO(new Size(Width, Height),
                new FramebufferAttachment[] { FramebufferAttachment.ColorAttachment0 },
                PixelInternalFormat.Rgba));
            fbos.Add(FrameBuffers.GaussianBlurHorizontal2, new FBO(new Size(Width / 4, Height / 4),
                new FramebufferAttachment[] { FramebufferAttachment.ColorAttachment0 },
                PixelInternalFormat.Rgba));
            fbos.Add(FrameBuffers.GaussianBlur2, new FBO(new Size(Width / 4, Height / 4),
                new FramebufferAttachment[] { FramebufferAttachment.ColorAttachment0 },
                PixelInternalFormat.Rgba));
        }


        public static void Dispose()
        {
            foreach (var r in fbos)
                r.Value.Dispose();
        }

        public static int Width => App.app.Width;
        public static int Height => App.app.Height;
        public static void Bind(Texture t, int textureUnit = 0, TextureTarget tr = TextureTarget.Texture2D)
        {
            if (t == null)
                return;
            Gl.ActiveTexture(textureUnit);
            Gl.BindTexture(tr, t.ID);
        }
        public static void Bind(Texture t, TextureTarget tr) => Bind(t, 0, tr);
        public static void Bind(TexturePack p, int a, int b, int c, int d, int e)
        {
            Bind(p.Diffuse, a);
            Bind(p.Normal, b);
            Bind(p.Specular, c);
            Bind(p.Displacement, d);
            Bind(p.Occlusion, e);
        }

        public static void Bind(TexturePack p, int offset) => Bind(p, offset++, offset++, offset++, offset++, offset++);
        public static void Bind(TexturePack p) => Bind(p, 0);


    }
    public class Texture
    {
        private static List<Texture> textures = new List<Texture>();

        public uint ID { get; }
        public int Width { get; }
        public int Height { get; }

        public Texture()
        {
            ID = Gl.GenTexture();
            textures.Add(this);
        }
        public Texture(uint id)
        {
            ID = id;
        }
        public Texture(string file) : this(TextureTarget.Texture2D, file)
        {
        }
        public Texture(TextureTarget t, string file) : this()
        {
            Gl.BindTexture(t, ID);
            TextureGl(t, file);
        }
        public static void TextureGl(TextureTarget t, string file, System.Drawing.RotateFlipType r = System.Drawing.RotateFlipType.RotateNoneFlipY)
        {
            System.Drawing.Bitmap b = new System.Drawing.Bitmap(file);
            b.RotateFlip(r);
            System.Drawing.Imaging.BitmapData data = b.LockBits(new System.Drawing.Rectangle(0, 0, b.Width, b.Height),
            System.Drawing.Imaging.ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Gl.PixelStoref(PixelStoreParameter.PackAlignment, 1);
            Gl.TexImage2D(t, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            b.UnlockBits(data);

            Gl.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.LinearMipMapLinear);
            Gl.TexParameterf(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, 0);

            if (Gl.IsExtensionSupported(Extension.GL_EXT_texture_filter_anisotropic))
                Gl.TexParameterf(TextureTarget.Texture2D, TextureParameterName.MaxAnisotropyExt,
                    Math.Min(4f, Gl.GetFloat(GetPName.MaxTextureMaxAnisotropyExt)));
        }


        public static void Dispose() => Gl.DeleteTextures(textures.Count, textures.Select(t => t.ID).ToArray());
    }
    public class TexturePack {
        public Texture Diffuse { get; }
        public Texture Normal { get; }
        public Texture Specular { get; }
        public Texture Displacement { get; }
        public Texture Occlusion { get; }

        public TexturePack(Texture diffuse, Texture normal = null, Texture specular = null, Texture displacement = null, Texture occlusion = null)
        {
            Diffuse = diffuse;
            Normal = normal;
            Specular = specular;
            Displacement = displacement;
            Occlusion = occlusion;
        }

        public bool HasDiffuse() => Diffuse != null;
        public bool HasNormal() => Normal != null;
        public bool HasSpecular() => Specular != null;
        public bool HasDisplacement() => Displacement != null;
        public bool HasOcclusion() => Occlusion != null;

        public void LoadBoolsToShader(ShaderProgram p, string prefix)
        {
            LoadBoolToShader(p, prefix, "hasDiffuse", HasDiffuse());
            LoadBoolToShader(p, prefix, "HasNormal", HasNormal());
            LoadBoolToShader(p, prefix, "HasSpecular", HasSpecular());
            LoadBoolToShader(p, prefix, "HasDisplacement", HasDisplacement());
            LoadBoolToShader(p, prefix, "HasOcclusion", HasOcclusion());
        }
        private void LoadBoolToShader(ShaderProgram p , string prefix, string name, bool value)
        {
            if (prefix == null || prefix.Equals(""))
                p[name]?.SetValue(value);
            else
                p[prefix + name.Substring(0, 1).ToUpper() + name.Substring(1)].SetValue(value);
        }
    }
}
