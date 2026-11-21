var _has_prev = true;
var _num_btns = 6;
var _active_slot = 0;
var _active_btn = _last_btn;
var _in_slot_mode = true;
var _in_btn_mode = false;
if (MENU_NO == 0)
{
    if (MENUCOORD[0] <= 2)
    {
        _active_slot = MENUCOORD[0];
        _last_slot = _active_slot;
        _in_slot_mode = true;
    }
    else
    {
        _active_slot = _last_slot;
        _in_slot_mode = false;
        _in_btn_mode = true;
        if (MENUCOORD[0] == 3)
        {
            _active_btn = 0;
        }
        else if (MENUCOORD[0] == 4)
        {
            _active_btn = 1;
        }
        else if (MENUCOORD[0] == 5)
        {
            _active_btn = 2;
        }
        else if (MENUCOORD[0] == 6)
        {
            _active_btn = 3;
        }
        else if (MENUCOORD[0] == 7)
        {
            _active_btn = 4;
        }
        else if (MENUCOORD[0] == 8)
        {
            _active_btn = 5;
        }
        _last_btn = _active_btn;
    }
}
else if (MENU_NO == 1 || MENU_NO == 4 || MENU_NO == 6 || MENU_NO == 7)
{
    if (MENU_NO == 1)
    {
        _active_slot = MENUCOORD[0];
    }
    if (MENU_NO == 4)
    {
        _active_slot = MENUCOORD[3];
    }
    if (MENU_NO == 6 || MENU_NO == 7)
    {
        _active_slot = MENUCOORD[5];
    }
    _last_slot = _active_slot;
}
else if (MENU_NO == 2 || MENU_NO == 3 || MENU_NO == 5 || MENU_NO == 10)
{
    if (MENUCOORD[MENU_NO] <= 2)
    {
        _active_slot = MENUCOORD[MENU_NO];
        _last_slot = _active_slot;
    }
    else
    {
        _active_slot = _last_slot;
    }
}
else if (MENU_NO == 11)
{
    _active_slot = MENUCOORD[10];
    _last_slot = _active_slot;
}
_slot_scroll = lerp(_slot_scroll, _active_slot, 1 - power(0.78, 1 / global.fps_scale));
_btn_scroll = lerp(_btn_scroll, _active_btn, 1 - power(0.78, 1 / global.fps_scale));
if (BGMADE == 1)
{
    ANIM_SINER += 1;
    ANIM_SINER_B += 1;
    BG_SINER += 1;
    if (BG_ALPHA < 0.5)
    {
        BG_ALPHA += (0.04 - (BG_ALPHA / 14));
    }
    if (BG_ALPHA > 0.5)
    {
        BG_ALPHA = 0.5;
    }
    var __WAVE_HEIGHT = 240;
    var __WAVE_WIDTH = 320;
    for (i = 0; i < (__WAVE_HEIGHT - 50); i += 1)
    {
        var __WAVEMINUS = BGMAGNITUDE * (i / __WAVE_HEIGHT);
        if (__WAVEMINUS > BGMAGNITUDE)
        {
            __WAVEMINUS = BGMAGNITUDE;
        }
        draw_background_part_ext(IMAGE_MENU, 0, i, __WAVE_WIDTH, 1, sin((i / 8) + (BG_SINER / 30)) * __WAVEMINUS, -10 + i, 1, 1, image_blend, BG_ALPHA * 0.8);
        draw_background_part_ext(IMAGE_MENU, 0, i, __WAVE_WIDTH, 1, -sin((i / 8) + (BG_SINER / 30)) * __WAVEMINUS, -10 + i, 1, 1, image_blend, BG_ALPHA * 0.8);
    }
    var _xx_siner_add = (sin(ANIM_SINER_B / 10) * 0.6) - 0.25;
    if (_xx_siner_add >= 0)
    {
        TRUE_ANIM_SINER += _xx_siner_add;
    }
    draw_sprite_ext(IMAGE_MENU_ANIMATION, ANIM_SINER / 12, 0, 0, 1, 1, 0, image_blend, BG_ALPHA * 0.46);
    draw_sprite_ext(IMAGE_MENU_ANIMATION, (ANIM_SINER / 12) + 0.4, 0, 0, 1, 1, 0, image_blend, BG_ALPHA * 0.56);
    draw_sprite_ext(IMAGE_MENU_ANIMATION, (ANIM_SINER / 12) + 0.8, 0, 0, 1, 1, 0, image_blend, BG_ALPHA * 0.7);
}
if (MENU_NO < 0)
{
    exit;
}
scr_84_set_draw_font("main");
draw_set_halign(fa_center);
draw_set_valign(fa_top);
var header_text = "BOLUM " + string(global.chapter) + "  -  KAYIT SECIMI";
if (MENU_NO == 2)
{
    header_text = (TYPE == 0) ? "KOPYALANACAK DOSYAYI SECIN." : "Kopyalanacak dosyayi secin.";
}
if (MENU_NO == 3)
{
    header_text = (TYPE == 0) ? "KOPYALANACAK HEDEFI SECIN." : "Hedef dosyayi secin.";
}
if (MENU_NO == 4)
{
    header_text = (TYPE == 0) ? "DOSYANIN UZERINE YAZILACAK." : "Dosyanin uzerine yazilacak.";
}
if (MENU_NO == 5)
{
    header_text = (TYPE == 0) ? "SILINECEK DOSYAYI SECIN." : "Silinecek dosyayi secin.";
}
if (MENU_NO == 6 || MENU_NO == 7)
{
    header_text = (TYPE == 0) ? "GERCEKTEN SILINSIN MI?" : "Gercekten silinsin mi?";
    if (MENU_NO == 7)
    {
        header_text = (TYPE == 0) ? "YOK EDILECEK." : "Gercekten silinsin mi?";
    }
}
if (MENU_NO == 10 || MENU_NO == 11)
{
    header_text = "Bolum " + string(global.chapter - 1) + " kaydi secin.";
}
draw_set_color(make_color_rgb(10, 8, 18));
draw_set_alpha(0.75);
draw_rectangle(16, 6, 304, 24, false);
draw_set_alpha(0.85);
draw_set_color(COL_A);
draw_rectangle(16, 23, 304, 24, false);
draw_set_alpha(1);
draw_set_color((MENU_NO == 6 || MENU_NO == 7) ? COL_PLUS : COL_B);
draw_text_shadow(160, 7, header_text);
for (var d = 0; d < 3; d++)
{
    var dot_x = 148 + (d * 12);
    var dot_y = 29;
    var is_cur_dot = _active_slot == d;
    draw_set_color(is_cur_dot ? COL_PLUS : COL_A);
    draw_set_alpha(is_cur_dot ? 1 : 0.45);
    draw_rectangle(dot_x - (is_cur_dot ? 3 : 1), dot_y - 1, dot_x + (is_cur_dot ? 3 : 1), dot_y + 1, false);
}
draw_set_alpha(1);
var slot_w = 194;
var slot_h = 86;
var slot_spacing = 208;
var slot_cy = 36;
for (i = 0; i < 3; i += 1)
{
    var card_cx = 160 + ((i - _slot_scroll) * slot_spacing);
    var x1 = round(card_cx - (slot_w * 0.5));
    var y1 = slot_cy;
    var x2 = x1 + slot_w;
    var y2 = y1 + slot_h;
    if (x2 < -16 || x1 > 336)
    {
        continue;
    }
    var dist = abs(i - _slot_scroll);
    var card_alpha = clamp(1 - (dist * 0.45), 0.35, 1);
    var is_sel_card = false;
    if ((MENU_NO == 0 || MENU_NO == 2 || MENU_NO == 3 || MENU_NO == 5 || MENU_NO == 10) && MENUCOORD[MENU_NO] == i)
    {
        is_sel_card = true;
    }
    if ((MENU_NO == 1 && MENUCOORD[0] == i) || (MENU_NO == 4 && MENUCOORD[3] == i) || ((MENU_NO == 6 || MENU_NO == 7) && MENUCOORD[5] == i) || (MENU_NO == 11 && MENUCOORD[10] == i))
    {
        is_sel_card = true;
    }
    var border_col = is_sel_card ? COL_B : COL_A;
    if (MENU_NO == 3 && MENUCOORD[2] == i)
    {
        border_col = COL_PLUS;
    }
    if ((MENU_NO == 6 || MENU_NO == 7) && MENUCOORD[5] == i)
    {
        border_col = 255;
    }
    draw_set_color(c_black);
    draw_set_alpha(0.55 * card_alpha);
    draw_rectangle(x1 + 3, y1 + 3, x2 + 3, y2 + 3, false);
    draw_set_color(is_sel_card ? make_color_rgb(20, 6, 10) : make_color_rgb(10, 8, 16));
    draw_set_alpha(0.9 * card_alpha);
    draw_rectangle(x1, y1, x2, y2, false);
    draw_set_color(border_col);
    draw_set_alpha((is_sel_card ? 0.95 : 0.45) * card_alpha);
    draw_rectangle(x1, y1, x2, y1 + 18, false);
    draw_set_color(border_col);
    draw_set_alpha(card_alpha);
    draw_rectangle(x1, y1, x2, y1 + 1, false);
    draw_rectangle(x1, y2 - 1, x2, y2, false);
    draw_rectangle(x1, y1, x1 + 1, y2, false);
    draw_rectangle(x2 - 1, y1, x2, y2, false);
    scr_84_set_draw_font("main");
    draw_set_halign(fa_left);
    draw_set_valign(fa_top);
    draw_set_color(is_sel_card ? make_color_rgb(12, 10, 22) : COL_B);
    draw_text(x1 + 8, y1 + 1, "KAYIT 0" + string(i + 1));
    if (MENU_NO < 10)
    {
        draw_set_halign(fa_right);
        draw_text(x2 - 8, y1 + 1, TIME_STRING[i]);
        draw_set_halign(fa_left);
        draw_set_color(border_col);
        draw_set_alpha(card_alpha);
        draw_text_shadow(x1 + 10, y1 + 23, NAME[i]);
        draw_set_color(is_sel_card ? COL_PLUS : COL_A);
        draw_text_shadow(x1 + 10, y1 + 41, PLACE[i]);
        draw_set_color(border_col);
        draw_set_alpha(0.3 * card_alpha);
        draw_rectangle(x1 + 8, y1 + 60, x2 - 8, y1 + 61, false);
        draw_set_alpha(card_alpha);
        if ((MENU_NO == 1 && MENUCOORD[0] == i) || (MENU_NO == 4 && MENUCOORD[3] == i) || ((MENU_NO == 6 || MENU_NO == 7) && MENUCOORD[5] == i))
        {
            var opt1 = "";
            var opt2 = "";
            if (MENU_NO == 1)
            {
                opt1 = (TYPE == 0) ? "DEVAM" : "Devam";
                opt2 = (TYPE == 0) ? "GERI" : "Geri";
                if (FILE[MENUCOORD[0]] == 0)
                {
                    opt1 = (TYPE == 0) ? "BASLA" : "Basla";
                }
            }
            if (MENU_NO == 4)
            {
                opt1 = (TYPE == 0) ? "EVET" : "Evet";
                opt2 = (TYPE == 0) ? "HAYIR" : "Hayir";
            }
            if (MENU_NO == 6)
            {
                opt1 = (TYPE == 0) ? "EVET!" : "Evet";
                opt2 = (TYPE == 0) ? "HAYIR!" : "Hayir";
            }
            if (MENU_NO == 7)
            {
                opt1 = (TYPE == 0) ? "SIL!" : "Evet";
                opt2 = (TYPE == 0) ? "SILME" : "Hayir";
            }
            var sel0 = MENUCOORD[MENU_NO] == 0;
            draw_set_color(sel0 ? COL_PLUS : COL_A);
            draw_text_shadow(x1 + 26, y1 + 64, opt1);
            var sel1 = MENUCOORD[MENU_NO] == 1;
            draw_set_color(sel1 ? COL_PLUS : COL_A);
            draw_text_shadow(x1 + 118, y1 + 64, opt2);
            var hx = (MENUCOORD[MENU_NO] == 0) ? (x1 + 14) : (x1 + 106);
            draw_sprite(spr_heartsmall, 0, hx, y1 + 68);
        }
        else
        {
            draw_set_halign(fa_center);
            draw_set_color(is_sel_card ? COL_PLUS : COL_A);
            var hint_str = (FILE[i] == 1) ? "[ Z : DEVAM ET ]" : "[ YENI OYUN ]";
            if (MENU_NO == 2)
            {
                hint_str = "[ KOPYALA ]";
            }
            if (MENU_NO == 3)
            {
                hint_str = (MENUCOORD[2] == i) ? "[ KAYNAK DOSYA ]" : "[ BURAYA KOPYALA ]";
            }
            if (MENU_NO == 5)
            {
                hint_str = "[ DOSYAYI SIL ]";
            }
            draw_text_shadow(card_cx, y1 + 64, hint_str);
            if (is_sel_card && MENUCOORD[MENU_NO] <= 2)
            {
                draw_sprite(spr_heartsmall, 0, x1 + 12, y1 + 68);
            }
        }
    }
    else
    {
        var loadable = 0;
        if (INCOMPLETE_LOAD == 0 && COMPLETEFILE_PREV[i] == 1)
        {
            loadable = 1;
        }
        if (INCOMPLETE_LOAD == 1 && INCOMPLETEFILE_PREV[i] == 1)
        {
            loadable = 1;
        }
        if (loadable == 1)
        {
            draw_set_halign(fa_right);
            draw_text(x2 - 8, y1 + 1, COMPLETETIME_STRING[i]);
            draw_set_halign(fa_left);
            draw_set_color(border_col);
            draw_text_shadow(x1 + 10, y1 + 23, COMPLETEFILE_PREV_NAME[i]);
            draw_set_color(is_sel_card ? COL_PLUS : COL_A);
            draw_text_shadow(x1 + 10, y1 + 41, COMPLETEFILE_PREV_PLACE[i]);
        }
        else
        {
            draw_set_halign(fa_left);
            draw_set_color(COL_A);
            draw_text_shadow(x1 + 10, y1 + 32, COMPLETEFILE_PREV_NAME[i]);
        }
        draw_set_color(border_col);
        draw_set_alpha(0.3 * card_alpha);
        draw_rectangle(x1 + 8, y1 + 60, x2 - 8, y1 + 61, false);
        draw_set_alpha(card_alpha);
        if (MENU_NO == 11 && MENUCOORD[10] == i)
        {
            draw_set_halign(fa_left);
            draw_set_color((MENUCOORD[11] == 0) ? COL_PLUS : COL_A);
            draw_text_shadow(x1 + 24, y1 + 64, "Basla");
            draw_set_color((MENUCOORD[11] == 1) ? COL_PLUS : COL_A);
            draw_text_shadow(x1 + 116, y1 + 64, "Geri");
            var hx = (MENUCOORD[11] == 0) ? (x1 + 12) : (x1 + 104);
            draw_sprite(spr_heartsmall, 0, hx, y1 + 68);
        }
        else
        {
            draw_set_halign(fa_center);
            draw_set_color(is_sel_card ? COL_PLUS : COL_A);
            draw_text_shadow(card_cx, y1 + 64, (loadable == 1) ? "[ KAYDI AKTAR ]" : "[ VERI YOK ]");
            if (is_sel_card && MENUCOORD[10] <= 2)
            {
                draw_sprite(spr_heartsmall, 0, x1 + 12, y1 + 68);
            }
        }
    }
}
if ((MENU_NO == 0 || MENU_NO == 2 || MENU_NO == 3 || MENU_NO == 5 || MENU_NO == 10) && MENUCOORD[MENU_NO] <= 2)
{
    var arr_bob = sin(current_time * 0.008) * 2;
    draw_set_halign(fa_center);
    draw_set_color(COL_PLUS);
    if (_active_slot > 0)
    {
        draw_text_shadow(48 - arr_bob, 70, "<");
    }
    if (_active_slot < 2)
    {
        draw_text_shadow(272 + arr_bob, 70, ">");
    }
}
if (MENU_NO == 0 || MENU_NO == 1)
{
    var btn_w = 128;
    var btn_h = 54;
    var btn_spacing = 140;
    var btn_y1 = 142;
    var btn_y2 = btn_y1 + btn_h;
    var b_titles = [];
    var b_subs = [];
    var b_badges = [];
    b_titles[0] = (TYPE == 0) ? "KOPYALA" : "Kopyala";
    b_subs[0] = "Kaydi kopyala";
    b_badges[0] = "01 / KOPYALA";
    b_titles[1] = (TYPE == 0) ? "SIL" : "Sil";
    b_subs[1] = "Kaydi sil";
    b_badges[1] = "02 / SIL";
    b_titles[2] = "Bl " + string(global.chapter - 1) + " Kayitlari";
    b_subs[2] = "Kayit aktar";
    b_badges[2] = "03 / ONCEKI BL";
    var lang_label = (global.lang == "en") ? "Dil / Lang" : "Turkce";
    b_titles[3] = lang_label;
    b_subs[3] = "Dil degistir";
    b_badges[3] = "04 / DIL";
    b_titles[4] = "Bolum Secimi";
    b_subs[4] = "Bolum listesi";
    b_badges[4] = "05 / BOLUMLER";
    b_titles[5] = "Oyundan Cik";
    b_subs[5] = "Cikis";
    b_badges[5] = "06 / CIKIS";
    draw_set_halign(fa_center);
    for (var bd = 0; bd < _num_btns; bd++)
    {
        var bdx = (160 - ((_num_btns - 1) * 10 * 0.5)) + (bd * 10);
        var bdy = 135;
        var b_cur = _active_btn == bd;
        draw_set_color(b_cur ? (_in_btn_mode ? COL_PLUS : COL_B) : COL_A);
        draw_set_alpha(b_cur ? 1 : 0.4);
        draw_rectangle(bdx - (b_cur ? 3 : 1), bdy - 1, bdx + (b_cur ? 3 : 1), bdy + 1, false);
    }
    draw_set_alpha(1);
    for (var b = 0; b < _num_btns; b++)
    {
        var bcx = 160 + ((b - _btn_scroll) * btn_spacing);
        var bx1 = round(bcx - (btn_w * 0.5));
        var bx2 = bx1 + btn_w;
        if (bx2 < -16 || bx1 > 336)
        {
            continue;
        }
        var bdist = abs(b - _btn_scroll);
        var balpha = clamp(1 - (bdist * 0.45), 0.3, 1);
        if (!_in_btn_mode)
        {
            balpha *= 0.78;
        }
        var is_btn_sel = _in_btn_mode && _active_btn == b;
        var b_col = is_btn_sel ? COL_PLUS : ((_active_btn == b) ? COL_B : COL_A);
        draw_set_color(c_black);
        draw_set_alpha(0.5 * balpha);
        draw_rectangle(bx1 + 2, btn_y1 + 2, bx2 + 2, btn_y2 + 2, false);
        draw_set_color(is_btn_sel ? make_color_rgb(20, 6, 10) : make_color_rgb(10, 8, 16));
        draw_set_alpha(0.9 * balpha);
        draw_rectangle(bx1, btn_y1, bx2, btn_y2, false);
        draw_set_color(b_col);
        draw_set_alpha((is_btn_sel ? 0.9 : 0.35) * balpha);
        draw_rectangle(bx1, btn_y1, bx2, btn_y1 + 15, false);
        draw_set_color(b_col);
        draw_set_alpha(balpha);
        draw_rectangle(bx1, btn_y1, bx2, btn_y1 + 1, false);
        draw_rectangle(bx1, btn_y2 - 1, bx2, btn_y2, false);
        draw_rectangle(bx1, btn_y1, bx1 + 1, btn_y2, false);
        draw_rectangle(bx2 - 1, btn_y1, bx2, btn_y2, false);
        draw_set_halign(fa_left);
        draw_set_color(is_btn_sel ? make_color_rgb(12, 10, 22) : COL_B);
        draw_text(bx1 + 6, btn_y1, b_badges[b]);
        draw_set_halign(fa_center);
        draw_set_color(is_btn_sel ? COL_PLUS : COL_B);
        draw_text_shadow(bcx + (is_btn_sel ? 5 : 0), btn_y1 + 22, b_titles[b]);
        if (is_btn_sel)
        {
            draw_sprite(spr_heartsmall, 0, bx1 + 10, btn_y1 + 26);
        }
    }
    if (_in_btn_mode)
    {
        var barr_bob = sin(current_time * 0.008) * 2;
        draw_set_halign(fa_center);
        draw_set_color(COL_PLUS);
        draw_text_shadow(84 - barr_bob, btn_y1 + 20, "<");
        draw_text_shadow(236 + barr_bob, btn_y1 + 20, ">");
    }
}
else if (MENU_NO == 2 || MENU_NO == 3 || MENU_NO == 5 || MENU_NO == 10)
{
    var is_cancel_sel = MENUCOORD[MENU_NO] == 3;
    var cx1 = 96;
    var cy1 = 148;
    var cx2 = 224;
    var cy2 = 188;
    var c_col = is_cancel_sel ? COL_PLUS : COL_A;
    draw_set_color(c_black);
    draw_set_alpha(0.5);
    draw_rectangle(cx1 + 2, cy1 + 2, cx2 + 2, cy2 + 2, false);
    draw_set_color(is_cancel_sel ? make_color_rgb(20, 6, 10) : make_color_rgb(10, 8, 16));
    draw_set_alpha(0.92);
    draw_rectangle(cx1, cy1, cx2, cy2, false);
    draw_set_color(c_col);
    draw_set_alpha(1);
    draw_rectangle(cx1, cy1, cx2, cy1 + 1, false);
    draw_rectangle(cx1, cy2 - 1, cx2, cy2, false);
    draw_rectangle(cx1, cy1, cx1 + 1, cy2, false);
    draw_rectangle(cx2 - 1, cy1, cx2, cy2, false);
    var cancel_str = (TYPE == 0) ? "IPTAL" : "Iptal";
    if (MENU_NO == 10)
    {
        cancel_str = "Veri Kullanmadan Basla";
    }
    draw_set_halign(fa_center);
    draw_set_color(c_col);
    draw_text_shadow(160, cy1 + 12, cancel_str);
    if (is_cancel_sel)
    {
        draw_sprite(spr_heartsmall, 0, cx1 + 10, cy1 + 16);
    }
}
draw_set_alpha(1);
if (MESSAGETIMER <= 0)
{
    if (TYPE == 1)
    {
        draw_set_color(COL_A);
        draw_set_halign(fa_left);
        draw_text_transformed(8, 224, "DELTARUNE " + version_text + " (C) Toby Fox 2018-2025", 0.5, 0.5, 0);
        draw_set_halign(fa_right);
        draw_text_transformed(312, 224, "SOL/SAG: KARTLARI KAYDIR", 0.5, 0.5, 0);
    }
}
else
{
    MESSAGETIMER -= (1 / global.fps_scale);
    draw_set_color(COL_PLUS);
    draw_set_halign(fa_center);
    draw_text_shadow(160, 214, TEMPCOMMENT);
}
draw_set_halign(fa_left);
draw_set_valign(fa_top);
draw_set_color(c_white);

