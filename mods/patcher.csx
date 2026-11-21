using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UndertaleModLib;
using UndertaleModLib.Compiler;
using UndertaleModLib.Models;
using UndertaleModLib.Util;
using ImageMagick;

EnsureDataLoaded();

string modsDir = Path.GetDirectoryName(ScriptPath);
string role = Environment.GetEnvironmentVariable("DR_ROLE") ?? "";
bool hasFps60 = (Environment.GetEnvironmentVariable("DR_MOD_FPS60") ?? "0") == "1";
bool installBorder = (Environment.GetEnvironmentVariable("DR_MOD_BORDER") ?? "0") == "1";
bool installMenu = (Environment.GetEnvironmentVariable("DR_MOD_MENU") ?? "0") == "1";

string cfgPath = Path.Combine(modsDir, "patch_config.txt");
if (File.Exists(cfgPath))
{
    foreach (var line in File.ReadAllLines(cfgPath))
    {
        var parts = line.Split(new[] { '=' }, 2);
        if (parts.Length != 2) continue;
        string k = parts[0].Trim().ToUpperInvariant();
        string v = parts[1].Trim();
        if (k == "ROLE" && !string.IsNullOrEmpty(v)) role = v.ToLowerInvariant();
        else if (k == "FPS60") hasFps60 = (v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase));
        else if (k == "BORDER") installBorder = (v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase));
        else if (k == "MENU") installMenu = (v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase));
    }
}

if (string.IsNullOrEmpty(role))
{
    string disp = Data?.GeneralInfo?.DisplayName?.Content?.ToLowerInvariant() ?? "";
    if (disp == "deltarune") role = "launcher";
    else if (disp.Contains("chapter 1")) role = "ch1";
    else if (disp.Contains("chapter 2")) role = "ch2";
    else if (disp.Contains("chapter 3")) role = "ch3";
    else if (disp.Contains("chapter 4")) role = "ch4";
    else if (disp.Contains("chapter 5")) role = "ch5";
}

ScriptMessage($"[Patcher] Role={role}, 60FPS={hasFps60}, Border={installBorder}, Menu={installMenu}");

var decompSettings = new Underanalyzer.Decompiler.DecompileSettings()
{
    RemoveSingleLineBlockBraces = true,
    EmptyLineAroundBranchStatements = true,
    EmptyLineBeforeSwitchCases = true,
};

// ============================================================================
// 1. BORDER MOD (NXRUNE)
// ============================================================================
if (installBorder)
{
    ApplyBorderMod(role, hasFps60, installMenu);
}

// ============================================================================
// 2. YENILENMIS TURKCE MENU MODU
// ============================================================================
if (installMenu)
{
    ApplyTurkishFontGlyphs();
    ApplyMenuMod(role, hasFps60, installBorder);
}

ScriptMessage($"[Patcher] {role} basariyla tamamlandi!");

// ============================================================================
// HELPER FUNCTIONS
// ============================================================================

void ApplyBorderMod(string r, bool fps60, bool menu)
{
    var importGroup = new CodeImportGroup(Data, null, decompSettings)
    {
        ThrowOnNoOpFindReplace = true
    };

    if (r == "launcher")
    {
        importGroup.QueueFindReplace("gml_Object_obj_init_pc_Create_0", "if (display_width > (640 * _ww) && display_height > (480 * _ww))", "if (display_width > (640 * _ww) && display_height > (360 * _ww))");
        importGroup.QueueFindReplace("gml_Object_obj_init_pc_Create_0", "window_set_size(640 * window_size_multiplier, 480 * window_size_multiplier);", "window_set_size(640 * window_size_multiplier, 360 * window_size_multiplier);");
        importGroup.QueueAppend("gml_Object_obj_init_pc_Create_0", @"application_surface_enable(true);
    application_surface_draw_enable(false);");
        importGroup.QueueFindReplace("gml_Object_obj_init_pc_Draw_77", "window_set_size(640 * window_size_multiplier, 480 * window_size_multiplier);", "window_set_size(640 * window_size_multiplier, 360 * window_size_multiplier);");
        importGroup.QueueAppend("gml_Object_obj_init_pc_Draw_77", @"var ww = window_get_width();
var wh = window_get_height();
var border_w = 1920;
var border_h = 1080;

var xx, yy;

if ((ww / wh) > (border_w / border_h))
{
    var scale = wh / border_h;
    border_w *= scale;
    border_h *= scale;
    xx = (320 * (wh / 1080)) + (abs(ww - border_w) / 2);
    yy = 60 * (wh / 1080);
}
else
{
    var scale = ww / border_w;
    border_w *= scale;
    border_h *= scale;
    xx = 320 * (ww / 1920);
    yy = (60 * (ww / 1920)) + (abs(wh - border_h) / 2);
}

texture_set_interpolation(false);
draw_enable_alphablend(false);
draw_surface_stretched(application_surface, xx, yy, ww - (2 * xx), wh - (2 * yy));
draw_enable_alphablend(true);
texture_set_interpolation(false);
");
        importGroup.QueueFindReplace("gml_Object_obj_screen_transition_Step_0", "if (global.is_console)", "    if (true)");
        importGroup.Import();
        Data.GameObjects.ByName("obj_init_pc").Persistent = true;
        return;
    }

    int chNum = int.Parse(r.Substring(2));
    string bordersPath = Path.Combine(modsDir, "border", "Borders", $"chapter{chNum}");
    Dictionary<string, UndertaleEmbeddedTexture> textures = new();
    int lastTextPage = Data.EmbeddedTextures.Count - 1;
    int lastTextPageItem = Data.TexturePageItems.Count - 1;

    foreach (var path in Directory.EnumerateFiles(bordersPath))
    {
        UndertaleEmbeddedTexture newtex = new UndertaleEmbeddedTexture();
        newtex.Name = new UndertaleString($"Texture {++lastTextPage}");
        newtex.TextureData.Image = GMImage.FromPng(File.ReadAllBytes(path));
        Data.EmbeddedTextures.Add(newtex);
        textures.Add(Path.GetFileName(path), newtex);
    }

    void AssignBorder(string name, UndertaleEmbeddedTexture tex, ushort x, ushort y, ushort sW, ushort sH, ushort tX = 0, ushort tY = 0, ushort tW = 0, ushort tH = 0, ushort bW = 0, ushort bH = 0)
    {
        if (tW == 0) tW = sW;
        if (tH == 0) tH = sH;
        if (bW == 0) bW = sW;
        if (bH == 0) bH = sH;
        var bg = Data.Sprites.ByName(name);
        if (bg is null) throw new ScriptException(name + " not found!");
        UndertaleTexturePageItem tpag = new UndertaleTexturePageItem();
        tpag.Name = new UndertaleString($"PageItem {++lastTextPageItem}");
        tpag.SourceX = x; tpag.SourceY = y;
        tpag.SourceWidth = sW; tpag.SourceHeight = sH;
        tpag.TargetX = tX; tpag.TargetY = tY;
        tpag.TargetWidth = tW; tpag.TargetHeight = tH;
        tpag.BoundingWidth = bW; tpag.BoundingHeight = bH;
        tpag.TexturePage = tex;
        Data.TexturePageItems.Add(tpag);
        bg.Textures[0].Texture = tpag;
    }

    if (chNum == 1)
    {
        AssignBorder("bg_border_line_1080", textures["bg_border_line_1080.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dark", textures["border_dark.png"], 2, 2, 1920, 1080);
        AssignBorder("border_light", textures["border_light.png"], 2, 2, 1920, 1080);

        importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", "if (display_width > (640 * _ww) && display_height > (480 * _ww))", "if (display_width > (640 * _ww) && display_height > (360 * _ww))");
        importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", "window_set_size(640 * window_size_multiplier, 480 * window_size_multiplier);", "window_set_size(640 * window_size_multiplier, 360 * window_size_multiplier);");
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_time_Create_0", @"if (global.is_console)
    {
        application_surface_enable(true);
        application_surface_draw_enable(false);
    }", @"application_surface_enable(true);
application_surface_draw_enable(false);");
        importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", "scr_enable_screen_border(global.is_console);", "scr_enable_screen_border(true);");
        importGroup.QueueFindReplace("gml_Object_obj_time_Draw_77", "if (scr_is_switch_os() || os_type == os_ps4 || os_type == os_ps5)", "if (true)");
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_time_Draw_77", @"var xx = floor((ww - (sw * global.window_scale)) / 2);
var yy = floor((wh - (sh * global.window_scale)) / 2);", @"var border_w = 1920;
var border_h = 1080;
var xx, yy;

if ((ww / wh) > (border_w / border_h))
{
    var scale = wh / border_h;
    border_w *= scale;
    border_h *= scale;
    xx = (320 * (wh / 1080)) + (abs(ww - border_w) / 2);
    yy = 60 * (wh / 1080);
}
else
{
    var scale = ww / border_w;
    border_w *= scale;
    border_h *= scale;
    xx = 320 * (ww / 1920);
    yy = (60 * (ww / 1920)) + (abs(wh - border_h) / 2);
}");
        importGroup.QueueFindReplace("gml_Object_obj_time_Draw_77", "draw_surface_ext(application_surface, xx, yy, global.window_scale, global.window_scale, 0, c_white, 1);", "draw_surface_stretched(application_surface, xx, yy, ww - (2 * xx), wh - (2 * yy));");
        importGroup.QueueFindReplace("gml_Object_obj_time_Draw_75", "window_set_size(640 * window_size_multiplier, 480 * window_size_multiplier);", "window_set_size(640 * window_size_multiplier, 360 * window_size_multiplier);");
        importGroup.QueuePrepend("gml_Object_obj_time_Draw_64", @"var ww = window_get_width();
var wh = window_get_height();
var border_w = 1920;
var border_h = 1080;
var xx, yy, scale;

if ((ww / wh) > (border_w / border_h))
{
    scale = wh / border_h;
    border_w *= scale;
    border_h *= scale;
    xx = (320 * (wh / 1080)) + (abs(ww - border_w) / 2);
    yy = 60 * (wh / 1080);
}
else
{
    scale = ww / border_w;
    border_w *= scale;
    border_h *= scale;
    xx = 320 * (ww / 1920);
    yy = (60 * (ww / 1920)) + (abs(wh - border_h) / 2);
}
display_set_gui_maximize();");
        importGroup.QueueFindReplace("gml_Object_obj_time_Draw_64", "draw_sprite_ext(scr_84_get_sprite(\"spr_quitmessage\"), quit_timer / 7, 4, 4, 2, 2, 0, c_white, quit_timer / 15);", "draw_sprite_ext(scr_84_get_sprite(\"spr_quitmessage\"), quit_timer / 7, xx + 4, yy + 4, 5 * scale, 5 * scale, 0, c_white, quit_timer / 15);");

        importGroup.QueueTrimmedLinesFindReplace("gml_GlobalScript_scr_draw_background_ps4", @"if (os_type == os_ps4 || os_type == os_ps5 || scr_is_switch_os())
    {
        var scale = window_get_width() / 1920;
        draw_background_stretched(bg, xx * scale, yy * scale, background_get_width(bg) * scale, background_get_height(bg) * scale);
    }
    else
    {
        draw_background(bg, xx, yy);
    }", @"var ww = window_get_width();
    var wh = window_get_height();
    var border_w = 1920;
    var border_h = 1080;
    var border_aspect = border_w / border_h;
    var window_aspect = ww / wh;
    var scale;
    
    if (window_aspect > border_aspect)
        scale = wh / border_h;
    else
        scale = ww / border_w;
    
    var draw_w = background_get_width(bg) * scale;
    var draw_h = background_get_height(bg) * scale;
    var off_x = (ww - (border_w * scale)) / 2;
    var off_y = (wh - (border_h * scale)) / 2;
    var draw_x = off_x + (xx * scale);
    var draw_y = off_y + (yy * scale);
    draw_background_stretched(bg, draw_x, draw_y, draw_w, draw_h);");

        if (!menu)
        {
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_DEVICE_MENU_Step_0", @"if (global.is_console)
                        {
                            global.screen_border_id = ini_read_string(""BORDER"", ""TYPE"", ""Dynamic"");", @"if (true)
                        {
                            global.screen_border_id = ini_read_string(""BORDER"", ""TYPE"", ""Dynamic"");");
        }
        importGroup.QueueFindReplace("gml_Object_DEVICE_MENU_Alarm_0", "if (global.is_console)", "if (true)");
        importGroup.QueueFindReplace("gml_Object_obj_chapter_continue_Alarm_0", "if (global.is_console)", "if (true)");

        if (fps60)
        {
            importGroup.QueueReplace("gml_Object_obj_darkcontroller_Draw_0", File.ReadAllText(Path.Combine(modsDir, "border", "ch1_darkcontroller_Draw_0_60fps.gml")));
            importGroup.QueueReplace("gml_Object_obj_darkcontroller_Step_0", File.ReadAllText(Path.Combine(modsDir, "border", "ch1_darkcontroller_Step_0_60fps.gml")));
        }
        else
        {
            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Draw_0", "draw_sprite(spr_heart, 0, _heartXPos, yy + 160 + (global.submenucoord[30] * 35));", "draw_sprite(spr_heart, 0, _heartXPos, yy + 140 + (global.submenucoord[30] * 35));");
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"draw_text(_xPos, yy + 150, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_86_0"")));
        draw_text(_selectXPos, yy + 150, string_hash_to_newline(audvol));
        draw_set_color(c_white);
        draw_text(_xPos, yy + 185, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_91_0"")));
        draw_text(_xPos, yy + 220, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_92_0"")));
        draw_text(_selectXPos, yy + 220, string_hash_to_newline(flashoff));", @"draw_text(_xPos, yy + 130, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_86_0"")));
        draw_text(_selectXPos, yy + 130, string_hash_to_newline(audvol));
        draw_set_color(c_white);
        draw_text(_xPos, yy + 165, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_91_0"")));
        draw_text(_xPos, yy + 200, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_92_0"")));
        draw_text(_selectXPos, yy + 200, string_hash_to_newline(flashoff));");
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"if (global.is_console)
        {
            draw_text(_xPos, yy + 255, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_94_0"")));
            draw_text(_selectXPos, yy + 255, string_hash_to_newline(runoff));
            
            if (global.submenu == 36)
                draw_set_color(c_yellow);
            else if (global.disable_border)
                draw_set_color(c_gray);
            
            draw_text(_xPos, yy + 290, border_text);
            draw_text(_selectXPos, yy + 290, border_options[selected_border]);
            draw_set_color(c_white);
            draw_text(_xPos, yy + 325, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 360, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_96_0"")));
        }
        else
        {
            draw_text(_xPos, yy + 255, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_93_0"")));
            draw_text(xx + 430, yy + 255, string_hash_to_newline(fullscreenoff));
            draw_text(_xPos, yy + 290, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_94_0"")));
            draw_text(xx + 430, yy + 290, string_hash_to_newline(runoff));
            draw_text(_xPos, yy + 325, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 360, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_96_0"")));
        }", @"draw_text(_xPos, yy + 235, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_93_0"")));
        draw_text(_selectXPos, yy + 235, string_hash_to_newline(fullscreenoff));
        draw_text(_xPos, yy + 270, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_94_0"")));
        draw_text(_selectXPos, yy + 270, string_hash_to_newline(runoff));
        if (global.submenu == 36)
            draw_set_color(c_yellow);
        else if (global.disable_border)
            draw_set_color(c_gray);
        draw_text(_xPos, yy + 305, border_text);
        draw_text(_selectXPos, yy + 305, border_options[selected_border]);
        draw_set_color(c_white);
        draw_text(_xPos, yy + 340, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
        draw_text(_xPos, yy + 375, string_hash_to_newline(scr_84_get_lang_string(""obj_darkcontroller_slash_Draw_0_gml_96_0"")));");

            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.is_console && global.submenu == 36)", "if (global.submenu == 36)");
            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.submenucoord[30] > 6)", "if (global.submenucoord[30] > 7)");
            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "global.submenucoord[30] = 6;", "global.submenucoord[30] = 7;");
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Step_0", @"if (global.is_console)
                {
                    if (global.submenucoord[30] == 3)
                    {
                        if (global.flag[11] == 0)
                            global.flag[11] = 1;
                        else
                            global.flag[11] = 0;
                    }
                    
                    if (global.submenucoord[30] == 4)
                    {
                        if (global.disable_border)
                        {
                            selectnoise = 0;
                        }
                        else
                        {
                            global.submenu = 36;
                            check_border = 1;
                            border_select = 0;
                        }
                    }
                    
                    if (global.submenucoord[30] == 5)
                        global.submenu = 34;
                    
                    if (global.submenucoord[30] == 6)
                        m_quit = 1;
                }
                else
                {
                    if (global.submenucoord[30] == 3)
                    {
                        with (obj_time)
                            fullscreen_toggle = 1;
                    }
                    
                    if (global.submenucoord[30] == 4)
                    {
                        if (global.flag[11] == 0)
                            global.flag[11] = 1;
                        else
                            global.flag[11] = 0;
                    }
                    
                    if (global.submenucoord[30] == 5)
                        global.submenu = 34;
                    
                    if (global.submenucoord[30] == 6)
                        m_quit = 1;
                }", @"if (global.submenucoord[30] == 3)
                    {
                        with (obj_time)
                            fullscreen_toggle = 1;
                    }
                    if (global.submenucoord[30] == 4)
                    {
                        if (global.flag[11] == 0)
                            global.flag[11] = 1;
                        else
                            global.flag[11] = 0;
                    }
                    if (global.submenucoord[30] == 5)
                    {
                        if (global.disable_border)
                        {
                            selectnoise = 0;
                        }
                        else
                        {
                            global.submenu = 36;
                            check_border = 1;
                            border_select = 0;
                        }
                    }
                    if (global.submenucoord[30] == 6)
                        global.submenu = 34;
                    if (global.submenucoord[30] == 7)
                        m_quit = 1;");
        }

        importGroup.Import();
        return;
    }

    // Chapters 2..5 Border Texture Assignment
    if (chNum == 2)
    {
        AssignBorder("border_line_1080", textures["border_line_1080.png"], 2, 2, 1920, 1080);
        AssignBorder("border_lw_town", textures["border_lw_town.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_castletown", textures["border_dw_castletown.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_cyber", textures["border_dw_cyber.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_mansion", textures["border_dw_mansion.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_city", textures["border_dw_city.png"], 2, 2, 1920, 1080);
        importGroup.QueueFindReplace("gml_Object_obj_initializer2_Create_0", "global.screen_border_id = \"\";", "global.screen_border_id = stringsetloc(\"Dynamic\", \"obj_initializer2_slash_Create_0_gml_22_0\");");
    }
    else if (chNum == 3)
    {
        AssignBorder("border_dw_tv_meta", textures["border_dw_tv_meta.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_tv_black", textures["border_dw_tv_black.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_green_room", textures["border_dw_green_room.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_word", textures["border_dw_word.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_teevie", textures["border_dw_teevie.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_red_smiles", textures["border_dw_red_smiles.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_blue_light", textures["border_dw_blue_light.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_green_sloppy_z", textures["border_dw_green_sloppy_z.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_blue_stars", textures["border_dw_blue_stars.png"], 2, 2, 1920, 1080);
        AssignBorder("border_lw_town_night", textures["border_lw_town_night.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_tv_blue", textures["border_dw_tv_blue.png"], 2, 2, 1920, 1080);
        AssignBorder("border_line_1080", textures["border_line_1080.png"], 2, 2, 1920, 1080);
        AssignBorder("border_lw_town", textures["border_lw_town.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_castletown", textures["border_dw_castletown.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_blue", textures["border_dw_blue.png"], 2, 2, 1920, 1080);
        AssignBorder("border_dw_green_sloppy", textures["border_dw_green_sloppy.png"], 2, 2, 1920, 1080);
    }
    else if (chNum == 4)
    {
        AssignBorder("border_dw_titan_base", textures["border_dw_titan_base.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
        AssignBorder("border_dw_titan_eyes_red", textures["border_dw_castletown.png"], 2, 1086, 1920, 941, 0, 70, 1920, 941);
        AssignBorder("border_lw_town_night", textures["border_lw_town_night.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
        AssignBorder("border_dw_titan_eyes", textures["border_dw_church_a.png"], 2, 1086, 1920, 881, 0, 100, 1920, 881);
        AssignBorder("border_line_1080", textures["border_line_1080.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
        AssignBorder("border_lw_town", textures["border_lw_town.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
        AssignBorder("border_dw_castletown", textures["border_dw_castletown.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
        AssignBorder("border_dw_church_c", textures["border_dw_church_c.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
        AssignBorder("border_dw_church_a", textures["border_dw_church_a.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
        AssignBorder("border_dw_church_b", textures["border_dw_church_b.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
    }
    else if (chNum == 5)
    {
        AssignBorder("border_lw_town_morning", textures["border_lw_town_morning.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_castle_left", textures["border_dw_castle_left.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_castle_cafe", textures["border_dw_castle_cafe.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_garden_cliff_bottom", textures["border_dw_garden_cliff_bottom.png"], 2, 2, 1920, 1350, 0, 0, 1920, 1350, 1920, 1350);
        AssignBorder("border_dw_garden", textures["border_dw_garden.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_castle_right", textures["border_dw_castle_right.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_garden_cliff_frame", textures["border_dw_garden_cliff_bottom_frame.png"], 2, 1023, 1337, 1017, 292, 32, 1337, 1017, 1920, 1080);
        AssignBorder("border_lw_town_sunset", textures["border_lw_town_sunset.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_garden_cliff_lattice", textures["border_dw_castle_cafe.png"], 2, 1086, 1864, 852, 28, 199, 1864, 852, 1920, 1350);
        AssignBorder("border_dw_pink_alt", textures["border_dw_pink_alt.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_garden_cliff_lattice_bottom", textures["border_dw_garden_cliff.png"], 1946, 2, 1, 1, 0, 0, 1, 1, 1920, 1350);
        AssignBorder("border_line_1080", textures["border_line_1080.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_lw_town", textures["border_lw_town.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_castletown", textures["border_dw_castletown.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_castle_top", textures["border_dw_castle_top.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_pink", textures["border_dw_pink.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_garden_cliff_bottom_frame", textures["border_dw_garden_cliff_bottom_frame.png"], 2, 2, 1337, 1017, 292, 32, 1337, 1017, 1920, 1080);
        AssignBorder("border_dw_castle_right_gold", textures["border_dw_castle_right_gold.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_lw_town_night", textures["border_lw_town_night.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080, 1920, 1080);
        AssignBorder("border_dw_garden_cliff", textures["border_dw_garden_cliff.png"], 2, 2, 1920, 1350, 0, 0, 1920, 1350, 1920, 1350);
    }

    // Common Ch2..5 obj_initializer2
    importGroup.QueueFindReplace("gml_Object_obj_initializer2_Create_0", @"if (global.is_console)
    loadtex = instance_create(0, 0, obj_prefetchtex);", @"if (true)
    loadtex = instance_create(0, 0, obj_prefetchtex);");

    if (chNum == 2)
    {
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_initializer2_Step_0", @"if (!textures_loaded)
        textures_loaded = loadtex.loaded;
    
    if (textures_loaded)
    {
    }
    else
    {
        exit;
    }
}", @"}
    if (!textures_loaded)
        textures_loaded = loadtex.loaded;
    
    if (textures_loaded)
    {
    }
    else
    {
        exit;
    }");
        importGroup.QueueFindReplace("gml_Object_obj_initializer2_Step_0", @"    if (global.is_console)
    {
        if (global.game_won == 1)", @"    if (true)
    {
        if (global.game_won == 1)");
    }
    else
    {
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_initializer2_Step_0", @"if (!textures_loaded)
        textures_loaded = loadtex.loaded;
    
    if (textures_loaded)
        show_debug_message_concat(""TEXTURES LOADED"");
    else
        exit;
}", @"}
    if (!textures_loaded)
        textures_loaded = loadtex.loaded;
    
    if (textures_loaded)
        show_debug_message_concat(""TEXTURES LOADED"");
    else
        exit;");
        importGroup.QueueTrimmedLinesFindReplace("gml_GlobalScript_scr_load", @"if (global.is_console)
        global.tempflag[95] = 1;", "global.tempflag[95] = 1;");
    }

    importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_initializer2_Step_0", @"if (global.is_console)
            global.screen_border_alpha = 0;", "global.screen_border_alpha = 0;");
    importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_initializer2_Step_0", @"if (global.is_console)
            global.screen_border_alpha = 1;", "global.screen_border_alpha = 1;");
    importGroup.QueueAppend("gml_Object_obj_initializer2_Step_0", "global.game_won = scr_completed_chapter_any_slot(global.chapter);");

    // obj_border_controller (Ch2..5)
    importGroup.QueueAppend("gml_Object_obj_border_controller_Create_0", @"application_surface_rects = {
    xx: 0,
    yy: 0,
    w: 640,
    h: 480,
    border_scale: 1
};");
    importGroup.QueueFindReplace("gml_Object_obj_border_controller_Draw_77",
@"var xx = floor((ww - (sw * global.window_scale)) / 2);
var yy = floor((wh - (sh * global.window_scale)) / 2);", @"var border_w = 1920;
var border_h = 1080;
var xx, yy;

if ((ww / wh) > (border_w / border_h))
{
    var scale = wh / border_h;
    border_w *= scale;
    border_h *= scale;
    xx = (320 * (wh / 1080)) + (abs(ww - border_w) / 2);
    yy = 60 * (wh / 1080);
}
else
{
    var scale = ww / border_w;
    border_w *= scale;
    border_h *= scale;
    xx = 320 * (ww / 1920);
    yy = (60 * (ww / 1920)) + (abs(wh - border_h) / 2);
}
application_surface_rects = 
{
    xx: xx,
    yy: yy,
    w: ww - (2 * xx),
    h: wh - (2 * yy),
    border_scale: scale
};");
    importGroup.QueueFindReplace("gml_Object_obj_border_controller_Draw_77", "draw_surface_ext(application_surface, xx, yy, global.window_scale, global.window_scale, 0, c_white, 1);", "draw_surface_stretched(application_surface, xx, yy, ww - (2 * xx), wh - (2 * yy));");

    // obj_time (Ch2..5)
    if (chNum == 2)
    {
        importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", @"if (global.is_console)
{
    if (!instance_exists(obj_gamecontroller))
        instance_create(0, 0, obj_gamecontroller);
    
    if (!i_ex(obj_border_controller))
        instance_create(0, 0, obj_border_controller);
}", @"if (global.is_console)
{
    if (!instance_exists(obj_gamecontroller))
        instance_create(0, 0, obj_gamecontroller);
}

if (!i_ex(obj_border_controller))
    instance_create(0, 0, obj_border_controller);");
    }
    else if (chNum == 3)
    {
        importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", @"if (global.is_console)
{
    if (!instance_exists(obj_gamecontroller))
        instance_create(0, 0, obj_gamecontroller);
    
    var border_controller = instance_create(0, 0, obj_border_controller);
    border_controller.init_border();
}", @"if (global.is_console)
{
    if (!instance_exists(obj_gamecontroller))
        instance_create(0, 0, obj_gamecontroller);
}
    var border_controller = instance_create(0, 0, obj_border_controller);
    border_controller.init_border();");

        importGroup.QueueFindReplace("gml_Object_obj_time_Step_0", @"if (global.is_console)
{
    if (!i_ex(obj_border_controller))
    {
        var border_controller = instance_create(0, 0, obj_border_controller);
        border_controller.init_border();
    }
}", @"if (!i_ex(obj_border_controller))
{
    var border_controller = instance_create(0, 0, obj_border_controller);
    border_controller.init_border();
}");
    }
    else
    {
        importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", @"if (global.is_console)
{
    if (!instance_exists(obj_gamecontroller))
        instance_create(0, 0, obj_gamecontroller);
    
    instance_create(0, 0, obj_border_controller);
}", @"if (global.is_console)
{
    if (!instance_exists(obj_gamecontroller))
        instance_create(0, 0, obj_gamecontroller);
}
    instance_create(0, 0, obj_border_controller);");
        importGroup.QueueFindReplace("gml_Object_obj_time_Step_0", "if (global.is_console)", "if (true)");
    }

    importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", "if (display_width > (640 * _ww) && display_height > (480 * _ww))", "if (display_width > (640 * _ww) && display_height > (360 * _ww))");
    importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", "window_set_size(640 * window_size_multiplier, 480 * window_size_multiplier);", "window_set_size(640 * window_size_multiplier, 360 * window_size_multiplier);");
    importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_time_Create_0", @"if (global.is_console)
    {
        application_surface_enable(true);
        application_surface_draw_enable(false);
    }", @"application_surface_enable(true);
application_surface_draw_enable(false);");
    importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", "scr_enable_screen_border(global.is_console);", "scr_enable_screen_border(true);");
    importGroup.QueueFindReplace("gml_Object_obj_time_Alarm_1", "window_set_size(640 * window_size_multiplier, 480 * window_size_multiplier);", "window_set_size(640 * window_size_multiplier, 360 * window_size_multiplier);");
    if (chNum != 5)
    {
        importGroup.QueueFindReplace("gml_Object_obj_time_Draw_77", "window_set_size(640 * window_size_multiplier, 480 * window_size_multiplier);", "window_set_size(640 * window_size_multiplier, 360 * window_size_multiplier);");
    }
    importGroup.QueuePrepend("gml_Object_obj_time_Draw_64", "display_set_gui_maximize();");
    importGroup.QueueFindReplace("gml_Object_obj_time_Draw_64", "draw_sprite_ext(scr_84_get_sprite(\"spr_quitmessage\"), quit_timer / 7, 4, 4, 2, 2, 0, c_white, quit_timer / 15);", "draw_sprite_ext(scr_84_get_sprite(\"spr_quitmessage\"), quit_timer / 7, obj_border_controller.application_surface_rects.xx + 4, obj_border_controller.application_surface_rects.yy + 4, 5 * obj_border_controller.application_surface_rects.border_scale, 5 * obj_border_controller.application_surface_rects.border_scale, 0, c_white, quit_timer / 15);");
    importGroup.QueueFindReplace("gml_Object_obj_time_Draw_75", "if (global.is_console)", "if (true)");

    // scr_draw_background_ps4 (Ch2..5)
    string ps4Replacement = @"var ww = window_get_width();
    var wh = window_get_height();
    var border_w = 1920;
    var border_h = 1080;
    var border_aspect = border_w / border_h;
    var window_aspect = ww / wh;
    var scale;
    
    if (window_aspect > border_aspect)
        scale = wh / border_h;
    else
        scale = ww / border_w;
    
    var draw_w = background_get_width(bg) * scale;
    var draw_h = background_get_height(bg) * scale;
    var off_x = (ww - (border_w * scale)) / 2;
    var off_y = (wh - (border_h * scale)) / 2;
    var draw_x = off_x + (xx * scale);
    var draw_y = off_y + (yy * scale);
    draw_background_stretched(bg, draw_x, draw_y, draw_w, draw_h);";

    if (chNum == 5)
    {
        ps4Replacement += @"
    c_bak = draw_get_color();
    a_bak = draw_get_alpha();
    draw_set_color(c_black);
    draw_set_alpha(1);
    
    if (window_aspect > border_aspect)
    {
        ossafe_fill_rectangle(0, 0, off_x - 1, wh);
        ossafe_fill_rectangle(draw_w + draw_x, 0, ww, wh);
    }
    else if (window_aspect != border_aspect)
    {
        ossafe_fill_rectangle(0, 0, ww, off_y - 1);
        ossafe_fill_rectangle(0, draw_h + draw_y, ww, wh);
    }
    
    draw_set_color(c_bak);
    draw_set_alpha(a_bak);";
    }

    importGroup.QueueTrimmedLinesFindReplace("gml_GlobalScript_scr_draw_background_ps4", @"if (os_type == os_ps4 || scr_is_switch_os() || os_type == os_ps5)
    {
        var scale = window_get_width() / 1920;
        draw_background_stretched(bg, xx * scale, yy * scale, background_get_width(bg) * scale, background_get_height(bg) * scale);
    }
    else
    {
        var scale = window_get_width() / 1920;
        draw_background_stretched(bg, xx * scale, yy * scale, background_get_width(bg) * scale, background_get_height(bg) * scale);
    }", ps4Replacement);

    // Chapter-specific extras
    if (chNum == 2 || chNum == 3)
    {
        int smScale = (chNum == 2) ? 2 : 3;
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_smallface_Draw_64", "draw_sprite_ext(sprite_index, image_index, x - cx, y - cy, image_xscale, image_yscale, image_angle, image_blend, facealpha);", $@"var base_scale = obj_border_controller.application_surface_rects.w / 640;
    var base_x = obj_border_controller.application_surface_rects.xx;
    var base_y = obj_border_controller.application_surface_rects.yy;
    var border_scale = obj_border_controller.application_surface_rects.border_scale;
    draw_sprite_ext(sprite_index, image_index, base_x + (x - cx) * base_scale, base_y + ((y - cy) * base_scale), image_xscale * {smScale} * border_scale, image_yscale * {smScale} * border_scale, image_angle, image_blend, facealpha);");
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_smallface_Draw_64", "draw_text((x + 70) - cx, (y + 10) - cy, string_hash_to_newline(mystring));", $"draw_text_transformed(base_x + (((x + 70) - cx) * base_scale), base_y + (((y + 10) - cy) * base_scale), string_hash_to_newline(mystring), {smScale} * border_scale, {smScale}* border_scale, 0);");
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_smallface_Draw_64", "draw_text((x + 70) - cx, (y + 15) - cy, string_hash_to_newline(mystring));", $"draw_text_transformed(base_x + (((x + 70) - cx) * base_scale), base_y + (((y + 15) - cy) * base_scale), string_hash_to_newline(mystring), {smScale} * border_scale, {smScale}* border_scale, 0);");
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_smallface_Draw_64", "draw_text((x + 70 + random(1)) - cx, (y + 15 + random(1)) - cy, string_hash_to_newline(partstring));", $"draw_text_transformed(base_x + (((x + 70 + random(1)) - cx) * base_scale), base_y + (((y + 15 + random(1)) - cy) * base_scale), string_hash_to_newline(partstring), {smScale} * border_scale, {smScale}* border_scale, 0);");
    }
    if (chNum == 2)
    {
        importGroup.QueueFindReplace("gml_Object_obj_ch2_lw_cutscenes_short_Create_0", "if (!noelle_chalk && global.is_console)", "if (!noelle_chalk)");
        if (!menu)
        {
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_DEVICE_MENU_Create_0", @"if (global.is_console)
    global.chapter_return = -1;", "global.chapter_return = -1;");
        }
    }
    if (chNum == 3)
    {
        importGroup.QueueFindReplace("gml_Object_obj_room_ranking_b_Step_0", "if (gacha_con == 121 && global.is_console)", "if (gacha_con == 121)");
        importGroup.QueueFindReplace("gml_Object_obj_shutta_photo_controller_Draw_74", "display_get_gui_width", "window_get_width");
        importGroup.QueueFindReplace("gml_Object_obj_shutta_photo_controller_Draw_74", "display_get_gui_height", "window_get_height");
    }
    if (chNum == 4)
    {
        importGroup.QueueFindReplace("gml_Object_obj_dw_church_ripplepuzzle_postgers_Step_0", "if (scr_is_switch_os())", "if (true)");
    }
    if (chNum == 5)
    {
        importGroup.QueueFindReplace("gml_Object_obj_date_controller_Step_0", "if (global.is_console)", "if (true)");
    }

    if (!menu)
    {
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_DEVICE_MENU_Step_0", @"if (!global.is_console)
                        {
                            ini_close();
                        }", @"if (!global.is_console)
                        {
                            global.screen_border_id = ini_read_string(""BORDER"", ""TYPE"", ""Dynamic"");
                            var _disable_border = global.screen_border_id == ""None"" || global.screen_border_id == ""なし"";
                            scr_enable_screen_border(!_disable_border);
                            ini_close();
                        }");
    }
    importGroup.QueueFindReplace("gml_Object_DEVICE_MENU_Alarm_0", "if (global.is_console)", "if (true)");
    importGroup.QueueFindReplace("gml_Object_obj_chapter_continue_Alarm_0", "if (global.is_console)", "if (true)");
    importGroup.QueueFindReplace("gml_GlobalScript_scr_text", "if (!paptalk && global.is_console)", "if (!paptalk)");
    importGroup.QueueFindReplace("gml_Object_obj_onion_event_Create_0", "if (global.is_console)", "if (true)");

    // obj_darkcontroller Draw_0 & Step_0 (Ch2..5)
    importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Draw_0", "draw_sprite(spr_heart, 0, _heartXPos, yy + 160 + (global.submenucoord[30] * 35));", "draw_sprite(spr_heart, 0, _heartXPos, yy + 140 + (global.submenucoord[30] * 35));");

    if (chNum < 5)
    {
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"draw_text(_xPos, yy + 150, string_hash_to_newline(stringsetloc(""Master Volume"", ""obj_darkcontroller_slash_Draw_0_gml_86_0"")));
        draw_text(_selectXPos, yy + 150, string_hash_to_newline(audvol));
        draw_set_color(c_white);
        draw_text(_xPos, yy + 185, string_hash_to_newline(stringsetloc(""Controls"", ""obj_darkcontroller_slash_Draw_0_gml_91_0"")));
        draw_text(_xPos, yy + 220, string_hash_to_newline(stringsetloc(""Simplify VFX"", ""obj_darkcontroller_slash_Draw_0_gml_92_0"")));
        draw_text(_selectXPos, yy + 220, string_hash_to_newline(flashoff));", @"draw_text(_xPos, yy + 130, string_hash_to_newline(stringsetloc(""Master Volume"", ""obj_darkcontroller_slash_Draw_0_gml_86_0"")));
        draw_text(_selectXPos, yy + 130, string_hash_to_newline(audvol));
        draw_set_color(c_white);
        draw_text(_xPos, yy + 165, string_hash_to_newline(stringsetloc(""Controls"", ""obj_darkcontroller_slash_Draw_0_gml_91_0"")));
        draw_text(_xPos, yy + 200, string_hash_to_newline(stringsetloc(""Simplify VFX"", ""obj_darkcontroller_slash_Draw_0_gml_92_0"")));
        draw_text(_selectXPos, yy + 200, string_hash_to_newline(flashoff));");

        if (fps60)
        {
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"if (global.is_console)
        {
            draw_text(_xPos, yy + 255, string_hash_to_newline(autorun_text));
            draw_text(_selectXPos, yy + 255, string_hash_to_newline(runoff));
            
            if (global.submenu == 36)
                draw_set_color(c_yellow);
            else if (global.disable_border)
                draw_set_color(c_gray);
            
            draw_text(_xPos, yy + 290, stringsetloc(""Border"", ""obj_darkcontroller_slash_Draw_0_gml_112_0""));
            draw_text(_selectXPos, yy + 290, border_options[selected_border]);
            draw_set_color(c_white);
            draw_text(_xPos, yy + 325, string_hash_to_newline(""Mod Settings""));
            draw_text(_xPos, yy + 360, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 395, string_hash_to_newline(back_text));
        }
        else
        {
            draw_text(_xPos, yy + 255, string_hash_to_newline(stringsetloc(""Fullscreen"", ""obj_darkcontroller_slash_Draw_0_gml_93_0"")));
            draw_text(xx + 430, yy + 255, string_hash_to_newline(fullscreenoff));
            draw_text(_xPos, yy + 290, string_hash_to_newline(autorun_text));
            draw_text(xx + 430, yy + 290, string_hash_to_newline(runoff));
            draw_text(_xPos, yy + 325, string_hash_to_newline(""Mod Settings""));
            draw_text(_xPos, yy + 360, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 395, string_hash_to_newline(back_text));
        }", @"draw_text(_xPos, yy + 235, string_hash_to_newline(stringsetloc(""Fullscreen"", ""obj_darkcontroller_slash_Draw_0_gml_93_0"")));
        draw_text(_selectXPos, yy + 235, string_hash_to_newline(fullscreenoff));
        draw_text(_xPos, yy + 270, string_hash_to_newline(autorun_text));
        draw_text(_selectXPos, yy + 270, string_hash_to_newline(runoff));
        if (global.submenu == 36)
            draw_set_color(c_yellow);
        else if (global.disable_border)
            draw_set_color(c_gray);
        draw_text(_xPos, yy + 305, stringsetloc(""Border"", ""obj_darkcontroller_slash_Draw_0_gml_112_0""));
        draw_text(_selectXPos, yy + 305, border_options[selected_border]);
        draw_set_color(c_white);
        draw_text(_xPos, yy + 340, string_hash_to_newline(""Mod Settings""));
        draw_text(_xPos, yy + 375, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
        draw_text(_xPos, yy + 410, string_hash_to_newline(back_text));");

            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.is_console && global.submenu == 36)", "if (global.submenu == 36)");
            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.submenucoord[30] > 7)", "if (global.submenucoord[30] > 8)");
            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "global.submenucoord[30] = 7;", "global.submenucoord[30] = 8;");

            string ch2Check = (chNum == 2) ? "\n        if (room == room_dw_mansion_krisroom && global.plot <= 100)\n            global.disable_border = true;\n" : "";
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Step_0", $@"if (global.is_console)
{{
    if (global.submenucoord[30] == 3)
    {{
        if (global.flag[11] == 0)
            global.flag[11] = 1;
        else
            global.flag[11] = 0;
    }}
    
    if (global.submenucoord[30] == 4)
    {{{ch2Check}
        if (global.disable_border)
        {{
            selectnoise = 0;
        }}
        else
        {{
            global.submenu = 36;
            check_border = 1;
            border_select = 0;
        }}
    }}
    
    if (global.submenucoord[30] == 5)
    {{
        global.submenu = 50;
        global.submenucoord[50] = 0;
    }}
    
    if (global.submenucoord[30] == 6)
        global.submenu = 34;
    
    if (global.submenucoord[30] == 7)
    {{
        m_quit = 1;
        cancelnoise = 1;
    }}
}}
else
{{
    if (global.submenucoord[30] == 3)
    {{
        with (obj_time)
            fullscreen_toggle = 1;
    }}
    
    if (global.submenucoord[30] == 4)
    {{
        if (global.flag[11] == 0)
            global.flag[11] = 1;
        else
            global.flag[11] = 0;
    }}
    
    if (global.submenucoord[30] == 5)
    {{
        global.submenu = 50;
        global.submenucoord[50] = 0;
    }}
    
    if (global.submenucoord[30] == 6)
        global.submenu = 34;
    
    if (global.submenucoord[30] == 7)
    {{
        m_quit = 1;
        cancelnoise = 1;
    }}
}}", $@"if (global.submenucoord[30] == 3)
{{
    with (obj_time)
        fullscreen_toggle = 1;
}}
if (global.submenucoord[30] == 4)
{{
    if (global.flag[11] == 0)
        global.flag[11] = 1;
    else
        global.flag[11] = 0;
}}
if (global.submenucoord[30] == 5)
{{{ch2Check}
    if (global.disable_border)
    {{
        selectnoise = 0;
    }}
    else
    {{
        global.submenu = 36;
        check_border = 1;
        border_select = 0;
    }}
}}
if (global.submenucoord[30] == 6)
{{
    global.submenu = 50;
    global.submenucoord[50] = 0;
}}
if (global.submenucoord[30] == 7)
    global.submenu = 34;
if (global.submenucoord[30] == 8)
{{
    m_quit = 1;
    cancelnoise = 1;
}}");
        }
        else
        {
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"if (global.is_console)
        {
            draw_text(_xPos, yy + 255, string_hash_to_newline(autorun_text));
            draw_text(_selectXPos, yy + 255, string_hash_to_newline(runoff));
            
            if (global.submenu == 36)
                draw_set_color(c_yellow);
            else if (global.disable_border)
                draw_set_color(c_gray);
            
            draw_text(_xPos, yy + 290, stringsetloc(""Border"", ""obj_darkcontroller_slash_Draw_0_gml_112_0""));
            draw_text(_selectXPos, yy + 290, border_options[selected_border]);
            draw_set_color(c_white);
            draw_text(_xPos, yy + 325, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 360, string_hash_to_newline(back_text));
        }
        else
        {
            draw_text(_xPos, yy + 255, string_hash_to_newline(stringsetloc(""Fullscreen"", ""obj_darkcontroller_slash_Draw_0_gml_93_0"")));
            draw_text(xx + 430, yy + 255, string_hash_to_newline(fullscreenoff));
            draw_text(_xPos, yy + 290, string_hash_to_newline(autorun_text));
            draw_text(xx + 430, yy + 290, string_hash_to_newline(runoff));
            draw_text(_xPos, yy + 325, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 360, string_hash_to_newline(back_text));
        }", @"draw_text(_xPos, yy + 235, string_hash_to_newline(stringsetloc(""Fullscreen"", ""obj_darkcontroller_slash_Draw_0_gml_93_0"")));
        draw_text(_selectXPos, yy + 235, string_hash_to_newline(fullscreenoff));
        draw_text(_xPos, yy + 270, string_hash_to_newline(autorun_text));
        draw_text(_selectXPos, yy + 270, string_hash_to_newline(runoff));
        if (global.submenu == 36)
            draw_set_color(c_yellow);
        else if (global.disable_border)
            draw_set_color(c_gray);
        draw_text(_xPos, yy + 305, stringsetloc(""Border"", ""obj_darkcontroller_slash_Draw_0_gml_112_0""));
        draw_text(_selectXPos, yy + 305, border_options[selected_border]);
        draw_set_color(c_white);
        draw_text(_xPos, yy + 340, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
        draw_text(_xPos, yy + 375, string_hash_to_newline(back_text));");

            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.is_console && global.submenu == 36)", "if (global.submenu == 36)");
            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.submenucoord[30] > 6)", "if (global.submenucoord[30] > 7)");
            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "global.submenucoord[30] = 6;", "global.submenucoord[30] = 7;");

            string ch2Check = (chNum == 2) ? "\n                        if (room == room_dw_mansion_krisroom && global.plot <= 100)\n                            global.disable_border = true;\n" : "";
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Step_0", $@"if (global.is_console)
                {{
                    if (global.submenucoord[30] == 3)
                    {{
                        if (global.flag[11] == 0)
                            global.flag[11] = 1;
                        else
                            global.flag[11] = 0;
                    }}
                    
                    if (global.submenucoord[30] == 4)
                    {{{ch2Check}
                        if (global.disable_border)
                        {{
                            selectnoise = 0;
                        }}
                        else
                        {{
                            global.submenu = 36;
                            check_border = 1;
                            border_select = 0;
                        }}
                    }}
                    
                    if (global.submenucoord[30] == 5)
                        global.submenu = 34;
                    
                    if (global.submenucoord[30] == 6)
                    {{
                        m_quit = 1;
                        cancelnoise = 1;
                    }}
                }}
                else
                {{
                    if (global.submenucoord[30] == 3)
                    {{
                        with (obj_time)
                            fullscreen_toggle = 1;
                    }}
                    
                    if (global.submenucoord[30] == 4)
                    {{
                        if (global.flag[11] == 0)
                            global.flag[11] = 1;
                        else
                            global.flag[11] = 0;
                    }}
                    
                    if (global.submenucoord[30] == 5)
                        global.submenu = 34;
                    
                    if (global.submenucoord[30] == 6)
                    {{
                        m_quit = 1;
                        cancelnoise = 1;
                    }}
                }}", $@"if (global.submenucoord[30] == 3)
                    {{
                        with (obj_time)
                            fullscreen_toggle = 1;
                    }}
                    if (global.submenucoord[30] == 4)
                    {{
                        if (global.flag[11] == 0)
                            global.flag[11] = 1;
                        else
                            global.flag[11] = 0;
                    }}
                    if (global.submenucoord[30] == 5)
                    {{{ch2Check}
                        if (global.disable_border)
                        {{
                            selectnoise = 0;
                        }}
                        else
                        {{
                            global.submenu = 36;
                            check_border = 1;
                            border_select = 0;
                        }}
                    }}
                    if (global.submenucoord[30] == 6)
                        global.submenu = 34;
                    if (global.submenucoord[30] == 7)
                    {{
                        m_quit = 1;
                        cancelnoise = 1;
                    }}");
        }
    }
    else
    {
        // Chapter 5 obj_darkcontroller
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"draw_text(_xPos, yy + 150, string_hash_to_newline(stringsetloc(""Master Volume"", ""obj_darkcontroller_slash_Draw_0_gml_86_0"")));
        draw_text(_selectXPos, yy + 150, string_hash_to_newline(audvol));
        draw_set_color(c_white);
        draw_text(_xPos, yy + 185, string_hash_to_newline(stringsetloc(""Controls"", ""obj_darkcontroller_slash_Draw_0_gml_91_0"")));",
        @"draw_text(_xPos, yy + 130, string_hash_to_newline(stringsetloc(""Master Volume"", ""obj_darkcontroller_slash_Draw_0_gml_86_0"")));
        draw_text(_selectXPos, yy + 130, string_hash_to_newline(audvol));
        draw_set_color(c_white);
        draw_text(_xPos, yy + 165, string_hash_to_newline(stringsetloc(""Controls"", ""obj_darkcontroller_slash_Draw_0_gml_91_0"")));");

        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"draw_text(_xPos, yy + 220, string_hash_to_newline(stringsetloc(""Simplify VFX"", ""obj_darkcontroller_slash_Draw_0_gml_92_0"")));
        draw_text(_selectXPos, yy + 220, string_hash_to_newline(flashoff));",
        @"draw_text(_xPos, yy + 200, string_hash_to_newline(stringsetloc(""Simplify VFX"", ""obj_darkcontroller_slash_Draw_0_gml_92_0"")));
        draw_text(_selectXPos, yy + 200, string_hash_to_newline(flashoff));");

        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"draw_text(_xPos, yy + 220, string_hash_to_newline(voicelines));
        draw_text(_selectXPos, yy + 220, thisstring);",
        @"draw_text(_xPos, yy + 200, string_hash_to_newline(voicelines));
        draw_text(_selectXPos, yy + 200, thisstring);");

        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"draw_text(_xPos, yy + 220, feathstr);", @"draw_text(_xPos, yy + 200, feathstr);");
        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"draw_text(_selectXPos, yy + 220 + 16, st[global.flag[25]]);", @"draw_text(_selectXPos, yy + 200 + 16, st[global.flag[25]]);");

        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"if (global.is_console)
        {
            var drawautorun = true;", @"if (true)
        {
            draw_text(_xPos, yy + 235, string_hash_to_newline(stringsetloc(""Fullscreen"", ""obj_darkcontroller_slash_Draw_0_gml_93_0"")));
            draw_text(xx + 430, yy + 235, string_hash_to_newline(fullscreenoff));
            var drawautorun = true;");

        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"draw_text(_xPos, yy + 255, string_hash_to_newline(autorun_text));
        draw_text(_selectXPos, yy + 255, string_hash_to_newline(runoff));",
        @"draw_text(_xPos, yy + 270, string_hash_to_newline(autorun_text));
        draw_text(_selectXPos, yy + 270, string_hash_to_newline(runoff));");

        importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"draw_text(_xPos, yy + 255, string_hash_to_newline(ralseis));
        draw_text(_selectXPos, yy + 255, string_hash_to_newline(thisstring));",
        @"draw_text(_xPos, yy + 270, string_hash_to_newline(ralseis));
        draw_text(_selectXPos, yy + 270, string_hash_to_newline(thisstring));");

        if (fps60)
        {
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"draw_text(_xPos, yy + 290, stringsetloc(""Border"", ""obj_darkcontroller_slash_Draw_0_gml_112_0""));
            draw_text(_selectXPos, yy + 290, border_options[selected_border]);
            draw_set_color(c_white);
            draw_text(_xPos, yy + 325, string_hash_to_newline(""Mod Settings""));
            draw_text(_xPos, yy + 360, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 395, string_hash_to_newline(back_text));",
            @"draw_text(_xPos, yy + 305, stringsetloc(""Border"", ""obj_darkcontroller_slash_Draw_0_gml_112_0""));
            draw_text(_selectXPos, yy + 305, border_options[selected_border]);
            draw_set_color(c_white);
            draw_text(_xPos, yy + 340, string_hash_to_newline(""Mod Settings""));
            draw_text(_xPos, yy + 375, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 410, string_hash_to_newline(back_text));");

            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.is_console && global.submenu == 36)", "if (global.submenu == 36)");
            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.submenucoord[30] > 7)", "if (global.submenucoord[30] > 8)");
            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "global.submenucoord[30] = 7;", "global.submenucoord[30] = 8;");
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Step_0", @"if (global.is_console)
                {
                    if (global.submenucoord[30] == 3)
                    {
                        if (global.chapter == 5 && global.tempflag[52] == 1)
                            global.flag[1392] = !global.flag[1392];
                        else
                            global.flag[11] = !global.flag[11];
                    }
                    
                    if (global.submenucoord[30] == 4)
                    {
                        if (global.disable_border)
                        {
                            selectnoise = 0;
                        }
                        else
                        {
                            global.submenu = 36;
                            check_border = 1;
                            border_select = 0;
                        }
                    }
                    
                    if (global.submenucoord[30] == 5)
                    {
                        global.submenu = 50;
                        global.submenucoord[50] = 0;
                    }
                    
                    if (global.submenucoord[30] == 6)
                        global.submenu = 34;
                    
                    if (global.submenucoord[30] == 7)
                    {
                        m_quit = 1;
                        cancelnoise = 1;
                    }
                }", @"if (true) {
                    if (global.submenucoord[30] == 3)
                    {
                        with (obj_time)
                            fullscreen_toggle = 1;
                    }
                    if (global.submenucoord[30] == 4)
                    {
                        if (global.chapter == 5 && global.tempflag[52] == 1)
                            global.flag[1392] = !global.flag[1392];
                        else
                            global.flag[11] = !global.flag[11];
                    }
                    if (global.submenucoord[30] == 5)
                    {
                        if (global.disable_border)
                        {
                            selectnoise = 0;
                        }
                        else
                        {
                            global.submenu = 36;
                            check_border = 1;
                            border_select = 0;
                        }
                    }
                    if (global.submenucoord[30] == 6)
                    {
                        global.submenu = 50;
                        global.submenucoord[50] = 0;
                    }
                    if (global.submenucoord[30] == 7)
                        global.submenu = 34;
                    if (global.submenucoord[30] == 8)
                    {
                        m_quit = 1;
                        cancelnoise = 1;
                    }
                }");
        }
        else
        {
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"draw_text(_xPos, yy + 290, stringsetloc(""Border"", ""obj_darkcontroller_slash_Draw_0_gml_112_0""));
            draw_text(_selectXPos, yy + 290, border_options[selected_border]);
            draw_set_color(c_white);
            draw_text(_xPos, yy + 325, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 360, string_hash_to_newline(back_text));",
            @"draw_text(_xPos, yy + 305, stringsetloc(""Border"", ""obj_darkcontroller_slash_Draw_0_gml_112_0""));
            draw_text(_selectXPos, yy + 305, border_options[selected_border]);
            draw_set_color(c_white);
            draw_text(_xPos, yy + 340, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 375, string_hash_to_newline(back_text));");

            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.is_console && global.submenu == 36)", "if (global.submenu == 36)");
            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.submenucoord[30] > 6)", "if (global.submenucoord[30] > 7)");
            importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "global.submenucoord[30] = 6;", "global.submenucoord[30] = 7;");
            importGroup.QueueTrimmedLinesFindReplace("gml_Object_obj_darkcontroller_Step_0", @"if (global.is_console)
                {
                    if (global.submenucoord[30] == 3)
                    {
                        if (global.chapter == 5 && global.tempflag[52] == 1)
                            global.flag[1392] = !global.flag[1392];
                        else
                            global.flag[11] = !global.flag[11];
                    }
                    
                    if (global.submenucoord[30] == 4)
                    {
                        if (global.disable_border)
                        {
                            selectnoise = 0;
                        }
                        else
                        {
                            global.submenu = 36;
                            check_border = 1;
                            border_select = 0;
                        }
                    }
                    
                    if (global.submenucoord[30] == 5)
                        global.submenu = 34;
                    
                    if (global.submenucoord[30] == 6)
                    {
                        m_quit = 1;
                        cancelnoise = 1;
                    }
                }", @"if (true) {
                    if (global.submenucoord[30] == 3)
                    {
                        with (obj_time)
                            fullscreen_toggle = 1;
                    }
                    if (global.submenucoord[30] == 4)
                    {
                        if (global.chapter == 5 && global.tempflag[52] == 1)
                            global.flag[1392] = !global.flag[1392];
                        else
                            global.flag[11] = !global.flag[11];
                    }
                    if (global.submenucoord[30] == 5)
                    {
                        if (global.disable_border)
                        {
                            selectnoise = 0;
                        }
                        else
                        {
                            global.submenu = 36;
                            check_border = 1;
                            border_select = 0;
                        }
                    }
                    if (global.submenucoord[30] == 6)
                        global.submenu = 34;
                    if (global.submenucoord[30] == 7)
                    {
                        m_quit = 1;
                        cancelnoise = 1;
                    }
                }");
        }
    }

    importGroup.Import();
}

void ApplyMenuMod(string r, bool fps60, bool border)
{
    var importGroup = new CodeImportGroup(Data, null, decompSettings);

    UndertaleGameObject EnsureObject(string objName, int depth, string roomName)
    {
        var obj = Data.GameObjects.ByName(objName);
        if (obj == null)
        {
            obj = new UndertaleGameObject
            {
                Name = Data.Strings.MakeString(objName),
                Visible = true,
                Persistent = false,
                Depth = depth,
                Awake = true
            };
            Data.GameObjects.Add(obj);
        }
        else
        {
            obj.Depth = depth;
            obj.Visible = true;
        }

        var room = Data.Rooms.ByName(roomName);
        if (room != null && !room.GameObjects.Any(go => go.ObjectDefinition == obj))
        {
            uint nextInstId = Data.GeneralInfo.LastObj + 1;
            Data.GeneralInfo.LastObj = nextInstId;
            room.GameObjects.Add(new UndertaleRoom.GameObject
            {
                X = 0,
                Y = 0,
                ObjectDefinition = obj,
                InstanceID = nextInstId,
                ScaleX = 1f,
                ScaleY = 1f,
                Color = 0xFFFFFFFF
            });
        }
        return obj;
    }

    void EnsureEventAndQueue(UndertaleGameObject obj, EventType evType, uint evSubtype, string codeName, string gmlSource)
    {
        var code = Data.Code.ByName(codeName);
        if (code == null)
        {
            code = new UndertaleCode { Name = Data.Strings.MakeString(codeName) };
            Data.Code.Add(code);
            if (Data.CodeLocals != null)
            {
                var locals = new UndertaleCodeLocals { Name = code.Name };
                locals.Locals.Add(new UndertaleCodeLocals.LocalVar
                {
                    Name = Data.Strings.MakeString("arguments"),
                    Index = 0
                });
                Data.CodeLocals.Add(locals);
            }
        }

        var evList = obj.Events[(int)evType];
        var existingEv = evList.FirstOrDefault(e => e.EventSubtype == evSubtype);
        if (existingEv == null)
        {
            var newEv = new UndertaleGameObject.Event { EventSubtype = evSubtype };
            newEv.Actions.Add(new UndertaleGameObject.EventAction
            {
                CodeId = code,
                ActionName = Data.Strings.MakeString("")
            });
            evList.Add(newEv);
        }
        else if (existingEv.Actions.Count == 0 || existingEv.Actions[0].CodeId == null)
        {
            existingEv.Actions.Clear();
            existingEv.Actions.Add(new UndertaleGameObject.EventAction
            {
                CodeId = code,
                ActionName = Data.Strings.MakeString("")
            });
        }

        importGroup.QueueReplace(code, gmlSource);
    }

    if (r == "launcher")
    {
        string lDir = Path.Combine(modsDir, "menu", "launcher");

        // Import spr_menu_glow if missing
        if (Data.Sprites.ByName("spr_menu_glow") == null)
        {
            string glowPath = Path.Combine(modsDir, "menu", "menu_glow.png");
            UndertaleEmbeddedTexture newtex = new UndertaleEmbeddedTexture();
            newtex.Name = new UndertaleString($"Texture {Data.EmbeddedTextures.Count}");
            newtex.TextureData.Image = GMImage.FromPng(File.ReadAllBytes(glowPath));
            Data.EmbeddedTextures.Add(newtex);

            UndertaleTexturePageItem tpag = new UndertaleTexturePageItem();
            tpag.Name = new UndertaleString($"PageItem {Data.TexturePageItems.Count}");
            tpag.SourceX = 0; tpag.SourceY = 0;
            tpag.SourceWidth = 128; tpag.SourceHeight = 128;
            tpag.TargetX = 0; tpag.TargetY = 0;
            tpag.TargetWidth = 128; tpag.TargetHeight = 128;
            tpag.BoundingWidth = 128; tpag.BoundingHeight = 128;
            tpag.TexturePage = newtex;
            Data.TexturePageItems.Add(tpag);

            var spr = new UndertaleSprite
            {
                Name = Data.Strings.MakeString("spr_menu_glow"),
                Width = 128,
                Height = 128,
                MarginLeft = 0,
                MarginRight = 127,
                MarginTop = 0,
                MarginBottom = 127,
                OriginX = 64,
                OriginY = 64
            };
            spr.Textures.Add(new UndertaleSprite.TextureEntry { Texture = tpag });
            Data.Sprites.Add(spr);
        }

        var bgObj = EnsureObject("obj_menu_bg", 90, "PLACE_CHAPTER_SELECT_2x");
        string bgCreate = "if (!variable_global_exists(\"fps_scale\"))\n{\n    global.fps_scale = 1;\n}\n" + File.ReadAllText(Path.Combine(lDir, "obj_menu_bg_Create_0.gml"));
        EnsureEventAndQueue(bgObj, EventType.Create, 0, "gml_Object_obj_menu_bg_Create_0", bgCreate);
        EnsureEventAndQueue(bgObj, EventType.Step, 0, "gml_Object_obj_menu_bg_Step_0", File.ReadAllText(Path.Combine(lDir, "obj_menu_bg_Step_0.gml")));
        EnsureEventAndQueue(bgObj, EventType.Draw, 0, "gml_Object_obj_menu_bg_Draw_0", File.ReadAllText(Path.Combine(lDir, "obj_menu_bg_Draw_0.gml")));

        var listObj = Data.GameObjects.ByName("obj_screen_select_list");
        EnsureEventAndQueue(listObj, EventType.Draw, 0, "gml_Object_obj_screen_select_list_Draw_0", File.ReadAllText(Path.Combine(lDir, "obj_screen_select_list_Draw_0.gml")));

        string chSelCreate = File.ReadAllText(Path.Combine(lDir, "obj_CHAPTER_SELECT_Create_0.gml"));
        if (fps60)
            chSelCreate = "room_speed = 60;\nglobal.fps_scale = 2;\n" + chSelCreate;
        else
            chSelCreate = "if (!variable_global_exists(\"fps_scale\"))\n{\n    global.fps_scale = 1;\n}\n" + chSelCreate;

        importGroup.QueueReplace("gml_GlobalScript_scr_init", File.ReadAllText(Path.Combine(lDir, "scr_init.gml")));
        importGroup.QueueReplace("gml_Object_obj_CHAPTER_SELECT_Create_0", chSelCreate);
        importGroup.QueueReplace("gml_Object_obj_screen_select_footer_Create_0", File.ReadAllText(Path.Combine(lDir, "obj_screen_select_footer_Create_0.gml")));
        importGroup.QueueReplace("gml_Object_obj_screen_select_list_Create_0", File.ReadAllText(Path.Combine(lDir, "obj_screen_select_list_Create_0.gml")));
        importGroup.QueueReplace("gml_Object_obj_screen_select_list_Step_0", File.ReadAllText(Path.Combine(lDir, "obj_screen_select_list_Step_0.gml")));
        importGroup.QueueReplace("gml_Object_obj_ui_chapter_Create_0", File.ReadAllText(Path.Combine(lDir, "obj_ui_chapter_Create_0.gml")));
        importGroup.QueueReplace("gml_Object_obj_ui_chapter_Step_0", File.ReadAllText(Path.Combine(lDir, "obj_ui_chapter_Step_0.gml")));
        importGroup.QueueReplace("gml_Object_obj_ui_chapter_Draw_0", File.ReadAllText(Path.Combine(lDir, "obj_ui_chapter_Draw_0.gml")));

        importGroup.Import();
        return;
    }

    // Chapters 1..5 Menu Mod
    string cDir = Path.Combine(modsDir, "menu", r);
    var chBgObj = EnsureObject("obj_chapter_bg", 90, "PLACE_MENU");
    string chBgCreate = "if (!variable_global_exists(\"fps_scale\"))\n{\n    global.fps_scale = 1;\n}\n" + File.ReadAllText(Path.Combine(cDir, "obj_chapter_bg_Create_0.gml"));
    EnsureEventAndQueue(chBgObj, EventType.Create, 0, "gml_Object_obj_chapter_bg_Create_0", chBgCreate);
    EnsureEventAndQueue(chBgObj, EventType.Step, 0, "gml_Object_obj_chapter_bg_Step_0", File.ReadAllText(Path.Combine(cDir, "obj_chapter_bg_Step_0.gml")));
    EnsureEventAndQueue(chBgObj, EventType.Draw, 0, "gml_Object_obj_chapter_bg_Draw_0", File.ReadAllText(Path.Combine(cDir, "obj_chapter_bg_Draw_0.gml")));

    importGroup.QueueReplace("gml_GlobalScript_snd_init", File.ReadAllText(Path.Combine(cDir, "snd_init.gml")));
    importGroup.QueueReplace("gml_GlobalScript_scr_84_load_ini", File.ReadAllText(Path.Combine(cDir, "scr_84_load_ini.gml")));
    if (r == "ch1")
    {
        importGroup.QueueReplace("gml_GlobalScript_scr_84_lang_load", File.ReadAllText(Path.Combine(cDir, "scr_84_lang_load.gml")));
    }

    string dmCreate = "if (!variable_global_exists(\"fps_scale\"))\n{\n    global.fps_scale = 1;\n}\n" + File.ReadAllText(Path.Combine(cDir, "DEVICE_MENU_Create_0.gml"));
    if (r == "ch2" && border)
    {
        dmCreate = dmCreate.Replace("if (global.is_console)\n{\n    global.chapter_return = -1;\n}", "global.chapter_return = -1;")
                           .Replace("if (global.is_console)\r\n{\r\n    global.chapter_return = -1;\r\n}", "global.chapter_return = -1;");
    }
    importGroup.QueueReplace("gml_Object_DEVICE_MENU_Create_0", dmCreate);
    importGroup.QueueReplace("gml_Object_DEVICE_MENU_Draw_0", File.ReadAllText(Path.Combine(cDir, "DEVICE_MENU_Draw_0.gml")));

    string dmStep = File.ReadAllText(Path.Combine(cDir, "DEVICE_MENU_Step_0.gml"));
    if (!border)
    {
        dmStep = dmStep.Replace("scr_enable_screen_border(!_disable_border);", "scr_enable_screen_border(global.is_console && !_disable_border);");
    }
    importGroup.QueueReplace("gml_Object_DEVICE_MENU_Step_0", dmStep);

    importGroup.Import();
}

void ApplyTurkishFontGlyphs()
{
    var trGlyphDefs = new (ushort code, char baseChar, string[] rows)[]
    {
        (199, 'C', new[] { "......","......","......","......",".####.","##..##","##..##","##....","##....","##....","##..##","##..##",".####.","..##..","...##.","..##.." }),
        (214, 'O', new[] { "......",".#..#.",".#..#.","......",".####.","##..##","##..##","##..##","##..##","##..##","##..##","##..##",".####.","......","......","......" }),
        (220, 'U', new[] { "......",".#..#.",".#..#.","......","##..##","##..##","##..##","##..##","##..##","##..##","##..##","##..##",".####.","......","......","......" }),
        (231, 'c', new[] { "......","......","......","......","......","......",".####.","##..##","##..##","##....","##..##","##..##",".####.","..##..","...##.","..##.." }),
        (246, 'o', new[] { "......","......","......",".#..#.",".#..#.","......",".####.","##..##","##..##","##..##","##..##","##..##",".####.","......","......","......" }),
        (252, 'u', new[] { "......","......","......",".#..#.",".#..#.","......","##..##","##..##","##..##","##..##","##..##","##..##",".#####","......","......","......" }),
        (286, 'G', new[] { "......",".#..#.","..##..","......",".####.","##..##","##..##","##....","##.###","##..##","##..##","##..##",".####.","......","......","......" }),
        (287, 'g', new[] { "......","......","......",".#..#.","..##..","......",".#####","##..##","##..##","##..##","##..##","##..##",".#####","....##","##..##",".####." }),
        (304, 'I', new[] { "......","..##..","..##..","......","######","..##..","..##..","..##..","..##..","..##..","..##..","..##..","######","......","......","......" }),
        (305, 'i', new[] { "......","......","......","......","......","......","####..","..##..","..##..","..##..","..##..","..##..","######","......","......","......" }),
        (350, 'S', new[] { "......","......","......","......",".####.","##..##","##..##","##....",".####.","....##","##..##","##..##",".####.","..##..","...##.","..##.." }),
        (351, 's', new[] { "......","......","......","......","......","......",".####.","##..##","##....",".####.","....##","##..##",".####.","..##..","...##.","..##.." })
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
}
