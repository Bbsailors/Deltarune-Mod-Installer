if (MENU_NO < 0)
{
    exit;
}
scr_84_set_draw_font("main");
var pulse = 0.5 + (0.5 * sin(current_time * 0.005));
PREV_MENU = MENU_NO;
if (MENU_NO == 1)
{
    PREV_MENU = 0;
}
if (MENU_NO == 4)
{
    PREV_MENU = 3;
}
if (MENU_NO == 6)
{
    PREV_MENU = 5;
}
if (MENU_NO == 7)
{
    PREV_MENU = 5;
}
var on_slots = true;
if (MENU_NO == 0 && MENUCOORD[0] >= 3)
{
    on_slots = false;
}
if ((MENU_NO == 2 || MENU_NO == 3 || MENU_NO == 5) && MENUCOORD[MENU_NO] == 3)
{
    on_slots = false;
}
var active_slot = _last_slot;
if (MENU_NO >= 0 && MENUCOORD[PREV_MENU] >= 0 && MENUCOORD[PREV_MENU] <= 2)
{
    active_slot = MENUCOORD[PREV_MENU];
    _last_slot = active_slot;
}
var slot_spacing = 208;
var slot_w = 194;
var slot_h = 86;
_slot_scroll = lerp(_slot_scroll, active_slot * slot_spacing, 1 - power(0.78, 1 / global.fps_scale));
for (i = 0; i < 3; i += 1)
{
    CONT_THIS = 0;
    if (MENUCOORD[0] == i && MENU_NO == 1)
    {
        CONT_THIS = 1;
    }
    if (MENUCOORD[3] == i && MENU_NO == 4)
    {
        CONT_THIS = 4;
    }
    if (MENUCOORD[5] == i && MENU_NO == 6)
    {
        CONT_THIS = 6;
    }
    if (MENUCOORD[5] == i && MENU_NO == 7)
    {
        CONT_THIS = 7;
    }
    var is_sel_slot = on_slots && MENUCOORD[PREV_MENU] == i;
    var card_pop = is_sel_slot ? -3 : 0;
    BOX_X1 = round((63 + (i * slot_spacing)) - _slot_scroll);
    BOX_Y1 = 36 + card_pop;
    BOX_X2 = BOX_X1 + slot_w;
    BOX_Y2 = BOX_Y1 + slot_h;
    draw_set_alpha(is_sel_slot ? 0.86 : 0.62);
    draw_set_color(make_color_rgb(8, 4, 10));
    draw_rectangle(BOX_X1, BOX_Y1, BOX_X2, BOX_Y2, false);
    var bcol = COL_A;
    if (is_sel_slot || (MENU_NO == 1 && MENUCOORD[0] == i))
    {
        bcol = COL_B;
    }
    if (MENU_NO == 3 || MENU_NO == 4)
    {
        if (MENUCOORD[2] == i)
        {
            bcol = COL_PLUS;
        }
    }
    if (MENU_NO == 7 && MENUCOORD[5] == i)
    {
        bcol = 255;
    }
    draw_set_color(bcol);
    draw_set_alpha(is_sel_slot ? 0.24 : 0.1);
    draw_rectangle(BOX_X1 + 2, BOX_Y1 + 2, BOX_X2 - 2, BOX_Y1 + 20, false);
    var b_alf = is_sel_slot ? 1 : 0.55;
    draw_sprite_ext(spr_pxwhite, 0, BOX_X1 - 2, BOX_Y1 - 2, slot_w + 4, 2, 0, bcol, b_alf);
    draw_sprite_ext(spr_pxwhite, 0, BOX_X1 - 2, BOX_Y2, slot_w + 4, 2, 0, bcol, b_alf);
    draw_sprite_ext(spr_pxwhite, 0, BOX_X1 - 2, BOX_Y1 - 2, 2, slot_h + 4, 0, bcol, b_alf);
    draw_sprite_ext(spr_pxwhite, 0, BOX_X2, BOX_Y1 - 2, 2, slot_h + 4, 0, bcol, b_alf);
    draw_sprite_ext(spr_pxwhite, 0, BOX_X1 + 6, BOX_Y1 + 21, slot_w - 12, 1, 0, bcol, b_alf * 0.65);
    draw_set_alpha(1);
    draw_set_color(bcol);
    draw_set_font(fnt_main);
    draw_text_transformed(BOX_X1 + 8, BOX_Y1 + 4, "KAYIT 0" + string(i + 1), 0.75, 0.75, 0);
    if (CONT_THIS < 4)
    {
        if (FILE[i] == 0)
        {
            scr_84_set_draw_font("main");
        }
        else if (INITLANG[i] == 0)
        {
            draw_set_font(fnt_main);
        }
        else if (INITLANG[i] == 1)
        {
            draw_set_font(fnt_ja_main);
        }
        draw_set_halign(fa_right);
        draw_text_shadow(BOX_X2 - 8, BOX_Y1 + 3, TIME_STRING[i]);
        draw_set_halign(fa_left);
        draw_text_shadow(BOX_X1 + 22, BOX_Y1 + 26, (FILE[i] == 0) ? "[BOS]" : NAME[i]);
        scr_84_set_draw_font("main");
    }
    if (CONT_THIS >= 1)
    {
        if (TYPE == 0)
        {
            if (MENU_NO == 1)
            {
                SELTEXT_C = " ";
                SELTEXT_A = "DEVAM";
                SELTEXT_B = "GERI";
                if (FILE[MENUCOORD[0]] == 0)
                {
                    SELTEXT_A = "BASLA";
                    SELTEXT_B = "GERI";
                }
            }
            if (MENU_NO == 4)
            {
                SELTEXT_A = "EVET";
                SELTEXT_B = "HAYIR";
                SELTEXT_C = "UZERINE YAZILACAK.";
            }
            if (MENU_NO == 6)
            {
                SELTEXT_A = "EVET";
                SELTEXT_B = "HAYIR";
                SELTEXT_C = "GERCEKTEN SILINSIN MI?";
            }
            if (MENU_NO == 7)
            {
                SELTEXT_A = "SIL";
                SELTEXT_B = "HAYIR";
                SELTEXT_C = "TAMAMEN YOK EDILECEK.";
            }
        }
        else
        {
            if (MENU_NO == 1)
            {
                SELTEXT_C = " ";
                SELTEXT_A = "Devam";
                SELTEXT_B = "Geri";
                if (FILE[MENUCOORD[0]] == 0)
                {
                    SELTEXT_A = "Basla";
                    SELTEXT_B = "Geri";
                }
            }
            if (MENU_NO == 4)
            {
                SELTEXT_A = "Evet";
                SELTEXT_B = "Hayir";
                SELTEXT_C = "Uzerine yazilsin mi?";
            }
            if (MENU_NO == 6)
            {
                SELTEXT_A = "Evet";
                SELTEXT_B = "Hayir";
                SELTEXT_C = "Bu dosya silinsin mi?";
            }
            if (MENU_NO == 7)
            {
                SELTEXT_A = "Evet!";
                SELTEXT_B = "Hayir!";
                SELTEXT_C = "Gercekten silinsin mi?";
            }
        }
        draw_set_color(COL_B);
        if (MENU_NO == 7)
        {
            draw_set_color(c_red);
        }
        if (SELTEXT_C != " ")
        {
            draw_text_shadow(BOX_X1 + 10, BOX_Y1 + 26, SELTEXT_C);
        }
        var c0_sel = MENUCOORD[MENU_NO] == 0;
        var c1_sel = MENUCOORD[MENU_NO] == 1;
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1 + 10, BOX_Y1 + 52, 82, 24, 0, c_black, 0.55);
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1 + 102, BOX_Y1 + 52, 82, 24, 0, c_black, 0.55);
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1 + 10, BOX_Y1 + 52, 82, 1, 0, c0_sel ? COL_B : COL_A, 0.9);
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1 + 10, BOX_Y1 + 75, 82, 1, 0, c0_sel ? COL_B : COL_A, 0.9);
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1 + 102, BOX_Y1 + 52, 82, 1, 0, c1_sel ? COL_B : COL_A, 0.9);
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1 + 102, BOX_Y1 + 75, 82, 1, 0, c1_sel ? COL_B : COL_A, 0.9);
        draw_set_color(COL_A);
        if (c0_sel)
        {
            draw_set_color(COL_B);
            HEARTX = BOX_X1 + 16;
            HEARTY = BOX_Y1 + 60;
        }
        draw_text_shadow(BOX_X1 + 28, BOX_Y1 + 55, SELTEXT_A);
        draw_set_color(COL_A);
        if (c1_sel)
        {
            draw_set_color(COL_B);
            HEARTX = BOX_X1 + 108;
            HEARTY = BOX_Y1 + 60;
        }
        draw_text_shadow(BOX_X1 + 120, BOX_Y1 + 55, SELTEXT_B);
    }
    else
    {
        draw_text_shadow(BOX_X1 + 22, BOX_Y1 + 46, PLACE[i]);
        draw_set_font(fnt_main);
        draw_set_halign(fa_center);
        draw_set_color(is_sel_slot ? COL_B : COL_A);
        draw_set_alpha(is_sel_slot ? 0.9 : 0.55);
        var st_txt = (FILE[i] == 1) ? "[ Z : DEVAM ET ]" : "[ Z : YENI KAYIT ]";
        if (MENU_NO == 2)
        {
            st_txt = "[ Z : KOPYALA ]";
        }
        if (MENU_NO == 3)
        {
            st_txt = (MENUCOORD[2] == i) ? "[ KAYNAK DOSYA ]" : "[ Z : BURAYA KOPYALA ]";
        }
        if (MENU_NO == 5)
        {
            st_txt = "[ Z : DOSYAYI SIL ]";
        }
        draw_text_transformed(BOX_X1 + (slot_w * 0.5), BOX_Y1 + 69, st_txt, 0.65, 0.65, 0);
        draw_set_halign(fa_left);
        draw_set_alpha(1);
        scr_84_set_draw_font("main");
    }
}
for (var d = 0; d < 3; d++)
{
    var d_on = d == active_slot;
    draw_set_color(d_on ? COL_B : COL_A);
    draw_set_alpha(d_on ? 1 : 0.45);
    var dx = 160 + ((d - 1) * 12);
    draw_rectangle(dx - (d_on ? 3 : 1), 125, dx + (d_on ? 3 : 1), 128, false);
}
if (on_slots)
{
    draw_set_color(COL_B);
    draw_set_alpha(0.45 + (0.4 * pulse));
    if (active_slot > 0)
    {
        for (var k = 0; k < 5; k++)
        {
            draw_rectangle(6 + k, 78 - k, 7 + k, 78 + k, false);
        }
    }
    if (active_slot < 2)
    {
        for (var k = 0; k < 5; k++)
        {
            draw_rectangle(313 - k, 78 - k, 314 - k, 78 + k, false);
        }
    }
}
draw_set_alpha(1);
if (MENU_NO >= 0)
{
    if (MENU_NO == 0 || MENU_NO == 2 || MENU_NO == 3 || MENU_NO == 5)
    {
        if (MENUCOORD[MENU_NO] >= 0 && MENUCOORD[MENU_NO] <= 2)
        {
            var cur_slot_x = round((63 + (MENUCOORD[MENU_NO] * slot_spacing)) - _slot_scroll);
            HEARTX = cur_slot_x + 8;
            HEARTY = 63;
        }
    }
    if (MENU_NO >= 2 && (MENU_NO == 2 || MENU_NO == 3 || MENU_NO == 5))
    {
        CANCELTEXT = (TYPE == 1) ? "Iptal" : "IPTAL";
        var c_sel = MENUCOORD[PREV_MENU] == 3;
        var cx1 = 96;
        var cy1 = c_sel ? 148 : 151;
        var cx2 = 224;
        var cy2 = cy1 + 42;
        var ccol = c_sel ? COL_B : COL_A;
        draw_sprite_ext(spr_pxwhite, 0, cx1, cy1, cx2 - cx1, cy2 - cy1, 0, make_color_rgb(10, 4, 12), c_sel ? 0.88 : 0.65);
        draw_sprite_ext(spr_pxwhite, 0, cx1 - 2, cy1 - 2, (cx2 - cx1) + 4, 2, 0, ccol, c_sel ? 1 : 0.6);
        draw_sprite_ext(spr_pxwhite, 0, cx1 - 2, cy2, (cx2 - cx1) + 4, 2, 0, ccol, c_sel ? 1 : 0.6);
        draw_sprite_ext(spr_pxwhite, 0, cx1 - 2, cy1 - 2, 2, (cy2 - cy1) + 4, 0, ccol, c_sel ? 1 : 0.6);
        draw_sprite_ext(spr_pxwhite, 0, cx2, cy1 - 2, 2, (cy2 - cy1) + 4, 0, ccol, c_sel ? 1 : 0.6);
        draw_set_color(ccol);
        draw_set_halign(fa_center);
        draw_text_shadow(160, cy1 + 12, CANCELTEXT);
        draw_set_halign(fa_left);
        if (c_sel)
        {
            HEARTX = cx1 + 12;
            HEARTY = cy1 + 17;
        }
    }
    if (MENU_NO == 0 || MENU_NO == 1)
    {
        COPYTEXT = (TYPE == 1) ? "Kopyala" : "KOPYALA";
        ERASETEXT = (TYPE == 1) ? "Sil" : "SIL";
        CHSELECTTEXT = (TYPE == 1) ? "Bolum Secimi" : "BOLUM SECIMI";
        if (global.lang == "ja")
        {
            LANGUAGETEXT = (TYPE == 1) ? "Turkce" : "TURKCE";
        }
        else
        {
            LANGUAGETEXT = "æ—¥æœ¬èªž";
        }
        QUITTEXT = (TYPE == 1) ? "Oyundan Cik" : "OYUNDAN CIK";
        var btn_labels = [COPYTEXT, ERASETEXT, CHSELECTTEXT, LANGUAGETEXT, QUITTEXT];
        var btn_tags = ["01 / KOPYALA", "02 / SIL", "03 / BOLUMLER", "04 / DIL", "05 / CIKIS"];
        var btn_count = global.is_console ? 4 : 5;
        if (MENU_NO == 0 && MENUCOORD[0] >= 3)
        {
            _last_btn = clamp(MENUCOORD[0] - 3, 0, btn_count - 1);
        }
        var btn_w = 128;
        var btn_h = 54;
        var btn_spacing = 140;
        _btn_scroll = lerp(_btn_scroll, _last_btn * btn_spacing, 1 - power(0.78, 1 / global.fps_scale));
        for (var b = 0; b < btn_count; b++)
        {
            var b_coord = b + 3;
            var b_sel = MENU_NO == 0 && MENUCOORD[0] == b_coord;
            var bx1 = round((96 + (b * btn_spacing)) - _btn_scroll);
            var by1 = 142 + (b_sel ? -4 : 0);
            var bx2 = bx1 + btn_w;
            var by2 = by1 + btn_h;
            var bcol = b_sel ? COL_B : COL_A;
            draw_sprite_ext(spr_pxwhite, 0, bx1, by1, btn_w, btn_h, 0, make_color_rgb(10, 4, 12), b_sel ? 0.9 : 0.64);
            draw_sprite_ext(spr_pxwhite, 0, bx1 + 2, by1 + 2, btn_w - 4, 16, 0, bcol, b_sel ? 0.25 : 0.1);
            var ba = b_sel ? 1 : 0.55;
            draw_sprite_ext(spr_pxwhite, 0, bx1 - 2, by1 - 2, btn_w + 4, 2, 0, bcol, ba);
            draw_sprite_ext(spr_pxwhite, 0, bx1 - 2, by2, btn_w + 4, 2, 0, bcol, ba);
            draw_sprite_ext(spr_pxwhite, 0, bx1 - 2, by1 - 2, 2, btn_h + 4, 0, bcol, ba);
            draw_sprite_ext(spr_pxwhite, 0, bx2, by1 - 2, 2, btn_h + 4, 0, bcol, ba);
            draw_sprite_ext(spr_pxwhite, 0, bx1 + 6, by1 + 19, btn_w - 12, 1, 0, bcol, ba * 0.6);
            draw_set_font(fnt_main);
            draw_set_color(bcol);
            draw_set_alpha(b_sel ? 0.95 : 0.65);
            draw_set_halign(fa_center);
            draw_text_transformed(bx1 + (btn_w * 0.5), by1 + 4, btn_tags[b], 0.65, 0.65, 0);
            draw_set_alpha(1);
            if (b == 3)
            {
                if (global.lang == "en")
                {
                    draw_set_font(fnt_ja_main);
                }
                else
                {
                    draw_set_font(fnt_main);
                }
            }
            else
            {
                scr_84_set_draw_font("main");
            }
            draw_set_color(bcol);
            draw_text_shadow(bx1 + (btn_w * 0.5), by1 + 25, btn_labels[b]);
            draw_set_halign(fa_left);
            scr_84_set_draw_font("main");
            if (b_sel)
            {
                HEARTX = bx1 + 10;
                HEARTY = by1 + 30;
            }
        }
        for (var b = 0; b < btn_count; b++)
        {
            var b_on = b == _last_btn;
            draw_set_color(b_on ? COL_B : COL_A);
            draw_set_alpha((!on_slots ? 1 : 0.5) * (b_on ? 1 : 0.45));
            var dx = (160 - ((btn_count - 1) * 10 * 0.5)) + (b * 10);
            draw_rectangle(dx - (b_on ? 3 : 1), 203, dx + (b_on ? 3 : 1), 206, false);
        }
        if (!on_slots)
        {
            draw_set_color(COL_B);
            draw_set_alpha(0.45 + (0.4 * pulse));
            for (var k = 0; k < 5; k++)
            {
                draw_rectangle(6 + k, 168 - k, 7 + k, 168 + k, false);
                draw_rectangle(313 - k, 168 - k, 314 - k, 168 + k, false);
            }
        }
        draw_set_alpha(1);
    }
    draw_set_font(fnt_main);
    if (TYPE == 1)
    {
        draw_set_alpha(0.4);
        draw_set_color(c_white);
        draw_text_transformed(195, 230, "DELTARUNE " + version_text + "(C) Toby Fox 2018-2026 ", 0.5, 0.5, 0);
    }
    else
    {
        draw_set_color(COL_A);
        draw_text_transformed(248, 230, version_text, 0.5, 0.5, 0);
        draw_set_color(c_white);
    }
    scr_84_set_draw_font("main");
    draw_set_alpha(1);
    if (MESSAGETIMER <= 0)
    {
        if (TYPE == 0)
        {
            if (MENU_NO == 0 || MENU_NO == 1)
            {
                TEMPCOMMENT = " ";
            }
            if (MENU_NO == 2)
            {
                TEMPCOMMENT = "KOPYALANACAK OLANI SECIN.";
            }
            if (MENU_NO == 3)
            {
                TEMPCOMMENT = "HEDEF DOSYAYI SECIN.";
            }
            if (MENU_NO == 4)
            {
                TEMPCOMMENT = "UZERINE YAZILACAK.";
            }
            if (MENU_NO == 5 || MENU_NO == 6 || MENU_NO == 7)
            {
                TEMPCOMMENT = "SILINECEK OLANI SECIN.";
            }
        }
        if (TYPE == 1)
        {
            if (MENU_NO == 0 || MENU_NO == 1)
            {
                TEMPCOMMENT = "Lutfen bir dosya secin.";
            }
            if (MENU_NO == 2)
            {
                TEMPCOMMENT = "Kopyalanacak dosyayi secin.";
            }
            if (MENU_NO == 3)
            {
                TEMPCOMMENT = "Kopyalanacak yeri secin.";
            }
            if (MENU_NO == 4)
            {
                TEMPCOMMENT = "Dosyanin uzerine yazilacak.";
            }
            if (MENU_NO == 5 || MENU_NO == 6 || MENU_NO == 7)
            {
                TEMPCOMMENT = "Silinecek dosyayi secin.";
            }
        }
    }
    draw_set_color(COL_B);
    draw_set_halign(fa_center);
    draw_text_shadow(160, 16, TEMPCOMMENT);
    draw_set_halign(fa_left);
    MESSAGETIMER -= (1 / global.fps_scale);
}
if (abs(HEARTX - HEARTXCUR) <= 2)
{
    HEARTXCUR = HEARTX;
}
if (abs(HEARTY - HEARTYCUR) <= 2)
{
    HEARTYCUR = HEARTY;
}
HEARTXCUR += ((HEARTX - HEARTXCUR) * (1 - power(0.5, 1 / global.fps_scale)));
HEARTYCUR += ((HEARTY - HEARTYCUR) * (1 - power(0.5, 1 / global.fps_scale)));
draw_sprite(spr_heartsmall, 0, HEARTXCUR, HEARTYCUR);
draw_set_color(COL_B);
draw_set_font(fnt_main);
draw_text_transformed(__view_get(e__VW.XView, 0) + 8, __view_get(e__VW.YView, 0) + 3, "BOLUM 1", 0.75, 0.75, 0);
draw_set_color(c_white);

enum e__VW
{
    XView,
    YView,
    WView,
    HView,
    Angle,
    HBorder,
    VBorder,
    HSpeed,
    VSpeed,
    Object,
    Visible,
    XPort,
    YPort,
    WPort,
    HPort,
    Camera,
    SurfaceID
}
