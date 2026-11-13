using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Godot;

namespace MegaCrit.Sts2.Core.ClassicContent;

public static class ClassicWatcherAssets
{
    private static readonly Dictionary<string, Texture2D> Textures = new();
    public static Texture2D Texture(string name)
    {
        if (Textures.TryGetValue(name, out var cached)) return cached;
        bool jpg = name == "selectPortrait";
        using Stream stream = typeof(ClassicWatcherAssets).Assembly.GetManifestResourceStream("ClassicWatcherAssets." + name + (jpg ? ".jpg" : ".png"))
            ?? throw new InvalidOperationException("Missing embedded Watcher asset: " + name);
        using var bytes = new MemoryStream();
        stream.CopyTo(bytes);
        var image = new Image();
        if ((jpg ? image.LoadJpgFromBuffer(bytes.ToArray()) : image.LoadPngFromBuffer(bytes.ToArray())) != Error.Ok) throw new InvalidDataException(name);
        return Textures[name] = ImageTexture.CreateFromImage(image);
    }

    // Render STS1's setup-pose geometry directly, without loading its incompatible Spine 3.4 runtime.
    public static NCreatureVisuals CreateVisuals()
    {
        var root = new NCreatureVisuals();
        void Add(Node node, string name)
        {
            node.Name = name; root.AddChild(node); node.Owner = root; node.UniqueNameInOwner = true;
        }
        var body = new Node2D { Scale = new Vector2(1.15f,1.15f) };
        Add(body,"Visuals");
        using var stream = typeof(ClassicWatcherAssets).Assembly.GetManifestResourceStream("ClassicWatcherAssets.pose.json");
        using var pose = JsonDocument.Parse(stream);
        foreach (var part in pose.RootElement.EnumerateArray())
        {
            Texture2D texture = Texture(part.GetProperty("texture").GetString());
            var points = part.GetProperty("points").EnumerateArray().Select(v=>new Vector2(v[0].GetSingle(),v[1].GetSingle())).ToArray();
            var uv = part.GetProperty("uvs").EnumerateArray().Select(v=>new Vector2(v[0].GetSingle()*texture.GetWidth(),v[1].GetSingle()*texture.GetHeight())).ToArray();
            var triangles = part.GetProperty("triangles").EnumerateArray().Select(v=>v.GetInt32()).ToArray();
            var faces = new Godot.Collections.Array();
            for (int i=0;i<triangles.Length;i+=3) faces.Add(new int[] {triangles[i],triangles[i+1],triangles[i+2]});
            body.AddChild(new Polygon2D { Polygon = points, UV = uv, Polygons = faces, Texture = texture, Color = new Color(part.GetProperty("color").GetString()) });
        }
        Add(new Control { Position = new Vector2(-95,-320), Size = new Vector2(190,320) },"Bounds");
        Add(new Marker2D { Position = new Vector2(0,-350) },"IntentPos");
        Add(new Marker2D { Position = new Vector2(0,-165) },"CenterPos");
        Add(new Marker2D { Position = new Vector2(0,-100) },"OrbPos");
        Add(new Marker2D { Position = new Vector2(35,-270) },"TalkPos");
        Add(new Control(),"FormVfx");
        return root;
    }
}
