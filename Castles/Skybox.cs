using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace Castles
{
    class Skybox
    {
        public Skybox()
        {
            Gl.ActiveTexture(0);
            Gl.Enable(EnableCap.TextureCubeMap);
            uint id = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.TextureCubeMapPositiveX, id);
            Gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.TexImage2D(TextureTarget.TextureCubeMapNegativeX,0,PixelInternalFormat.Rgba, 10,10,0,PixelFormat.Rgba, PixelType.UnsignedByte, )
            IntPtr i = new IntPtr();
        }
    }
}
