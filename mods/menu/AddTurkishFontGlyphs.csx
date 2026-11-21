using System;
using System.Collections.Generic;
using System.Linq;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModLib.Util;
using ImageMagick;

EnsureDataLoaded();

var trGlyphDefs = new (ushort code, char baseChar, string[] rows)[]
{
    (199, 'C', new[] {
        "......",
        "......",
        "......",
        "......",
        ".####.",
        "##..##",
        "##..##",
        "##....",
        "##....",
        "##....",
        "##..##",
        "##..##",
        ".####.",
        "..##..",
        "...##.",
        "..##.."
    }),
    (214, 'O', new[] {
        "......",
        ".#..#.",
        ".#..#.",
        "......",
        ".####.",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        ".####.",
        "......",
        "......",
        "......"
    }),
    (220, 'U', new[] {
        "......",
        ".#..#.",
        ".#..#.",
        "......",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        ".####.",
        "......",
        "......",
        "......"
    }),
    (231, 'c', new[] {
        "......",
        "......",
        "......",
        "......",
        "......",
        "......",
        ".####.",
        "##..##",
        "##..##",
        "##....",
        "##..##",
        "##..##",
        ".####.",
        "..##..",
        "...##.",
        "..##.."
    }),
    (246, 'o', new[] {
        "......",
        "......",
        "......",
        ".#..#.",
        ".#..#.",
        "......",
        ".####.",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        ".####.",
        "......",
        "......",
        "......"
    }),
    (252, 'u', new[] {
        "......",
        "......",
        "......",
        ".#..#.",
        ".#..#.",
        "......",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        ".#####",
        "......",
        "......",
        "......"
    }),
    (286, 'G', new[] {
        "......",
        ".#..#.",
        "..##..",
        "......",
        ".####.",
        "##..##",
        "##..##",
        "##....",
        "##.###",
        "##..##",
        "##..##",
        "##..##",
        ".####.",
        "......",
        "......",
        "......"
    }),
    (287, 'g', new[] {
        "......",
        "......",
        "......",
        ".#..#.",
        "..##..",
        "......",
        ".#####",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        "##..##",
        ".#####",
        "....##",
        "##..##",
        ".####."
    }),
    (304, 'I', new[] {
        "......",
        "..##..",
        "..##..",
        "......",
        "######",
        "..##..",
        "..##..",
        "..##..",
        "..##..",
        "..##..",
        "..##..",
        "..##..",
        "######",
        "......",
        "......",
        "......"
    }),
    (305, 'i', new[] {
        "......",
        "......",
        "......",
        "......",
        "......",
        "......",
        "####..",
        "..##..",
        "..##..",
        "..##..",
        "..##..",
        "..##..",
        "######",
        "......",
        "......",
        "......"
    }),
    (350, 'S', new[] {
        "......",
        "......",
        "......",
        "......",
        ".####.",
        "##..##",
        "##..##",
        "##....",
        ".####.",
        "....##",
        "##..##",
        "##..##",
        ".####.",
        "..##..",
        "...##.",
        "..##.."
    }),
    (351, 's', new[] {
        "......",
        "......",
        "......",
        "......",
        "......",
        "......",
        ".####.",
        "##..##",
        "##....",
        ".####.",
        "....##",
        "##..##",
        ".####.",
        "..##..",
        "...##.",
        "..##.."
    })
};

using (TextureWorker worker = new())
{
    var modifiedPages = new HashSet<UndertaleEmbeddedTexture>();
    var pageImages = new Dictionary<UndertaleEmbeddedTexture, MagickImage>();

    void PatchPixelFont(string fontName, int scale, int destY)
    {
        var font = Data.Fonts.ByName(fontName);
        if (font == null || font.Texture == null) return;

        var tpi = font.Texture;
        var texPage = tpi.TexturePage;
        if (!pageImages.TryGetValue(texPage, out var img))
        {
            img = worker.GetEmbeddedTexture(texPage);
            pageImages[texPage] = img;
        }

        using var pixels = img.GetPixelsUnsafe();
        int gw = 6 * scale;
        int gh = 16 * scale;
        int stepX = 8 * scale;

        for (int idx = 0; idx < trGlyphDefs.Length; idx++)
        {
            var def = trGlyphDefs[idx];
            int gx = (2 * scale) + (idx * stepX);
            int gy = destY;

            for (int py = 0; py < gh; py++)
            {
                string rowStr = def.rows[py / scale];
                for (int px = 0; px < gw; px++)
                {
                    bool on = rowStr[px / scale] == '#';
                    int absX = tpi.SourceX + gx + px;
                    int absY = tpi.SourceY + gy + py;
                    if (on)
                        pixels.SetPixel(absX, absY, new byte[] { 255, 255, 255, 255 });
                    else
                        pixels.SetPixel(absX, absY, new byte[] { 0, 0, 0, 0 });
                }
            }

            var existing = font.Glyphs.FirstOrDefault(g => g.Character == def.code);
            if (existing == null)
            {
                existing = new UndertaleFont.Glyph();
                font.Glyphs.Add(existing);
            }
            existing.Character = def.code;
            existing.SourceX = (ushort)gx;
            existing.SourceY = (ushort)gy;
            existing.SourceWidth = (ushort)gw;
            existing.SourceHeight = (ushort)gh;
            existing.Shift = (short)(7 * scale);
            existing.Offset = 0;
        }

        var sorted = font.Glyphs.OrderBy(g => g.Character).ToList();
        font.Glyphs.Clear();
        foreach (var g in sorted)
            font.Glyphs.Add(g);

        if (font.RangeEnd < 351)
            font.RangeEnd = 351;

        modifiedPages.Add(texPage);
    }

    PatchPixelFont("fnt_main", 1, 110);
    PatchPixelFont("fnt_mainbig", 2, 208);

    // Also ensure other fonts (fnt_small, fnt_dotumche, fnt_comicsans, fnt_tinynoelle) have fallback mappings for Turkish chars
    foreach (var otherFont in Data.Fonts)
    {
        if (otherFont == null || otherFont.Name == null) continue;
        string fn = otherFont.Name.Content;
        if (fn == "fnt_main" || fn == "fnt_mainbig" || fn.StartsWith("fnt_ja_")) continue;

        bool changed = false;
        foreach (var def in trGlyphDefs)
        {
            if (otherFont.Glyphs.Any(g => g.Character == def.code)) continue;
            var baseGlyph = otherFont.Glyphs.FirstOrDefault(g => g.Character == (ushort)def.baseChar);
            if (baseGlyph != null)
            {
                otherFont.Glyphs.Add(new UndertaleFont.Glyph
                {
                    Character = def.code,
                    SourceX = baseGlyph.SourceX,
                    SourceY = baseGlyph.SourceY,
                    SourceWidth = baseGlyph.SourceWidth,
                    SourceHeight = baseGlyph.SourceHeight,
                    Shift = baseGlyph.Shift,
                    Offset = baseGlyph.Offset
                });
                changed = true;
            }
        }
        if (changed)
        {
            var sorted = otherFont.Glyphs.OrderBy(g => g.Character).ToList();
            otherFont.Glyphs.Clear();
            foreach (var g in sorted)
                otherFont.Glyphs.Add(g);
            if (otherFont.RangeEnd < 351)
                otherFont.RangeEnd = 351;
        }
    }

    foreach (var texPage in modifiedPages)
    {
        var img = pageImages[texPage];
        texPage.TextureData.Image = GMImage.FromMagickImage(img).ConvertToFormat(texPage.TextureData.Image.Format);
        img.Dispose();
    }
}
