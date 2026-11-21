scr_84_set_draw_font("main");
var pulse = 0.5 + (0.5 * sin(current_time * 0.005));
if (MENU_NO >= 0)
{
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
    if (MENU_NO == 11)
    {
        PREV_MENU = 10;
    }
    var on_slots = true;
    if (MENU_NO == 0 && MENUCOORD[0] >= 3)
    {
        on_slots = false;
    }
    if ((MENU_NO == 2 || MENU_NO == 3 || MENU_NO == 5 || MENU_NO == 10) && MENUCOORD[MENU_NO] == 3)
    {
        on_slots = false;
    }
    var active_slot = _last_slot;
    if (MENUCOORD[PREV_MENU] >= 0 && MENUCOORD[PREV_MENU] <= 2)
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
        if (MENUCOORD[10] == i && MENU_NO == 11)
        {
            CONT_THIS = 11;
        }
        var is_sel_slot = on_slots && MENUCOORD[PREV_MENU] == i;
        var card_pop = is_sel_slot ? -3 : 0;
        BOX_X1 = round((63 + (i * slot_spacing)) - _slot_scroll);
        BOX_Y1 = 36 + card_pop;
        BOX_X2 = BOX_X1 + slot_w;
        BOX_Y2 = BOX_Y1 + slot_h;
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1, BOX_Y1, slot_w, slot_h, 0, make_color_rgb(4, 14, 20), is_sel_slot ? 0.88 : 0.65);
        var bcol = COL_A;
        if (is_sel_slot || CONT_THIS >= 1)
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
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1 + 2, BOX_Y1 + 2, slot_w - 4, 18, 0, bcol, is_sel_slot ? 0.24 : 0.1);
        var b_alf = is_sel_slot ? 1 : 0.55;
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1 - 2, BOX_Y1 - 2, slot_w + 4, 2, 0, bcol, b_alf);
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1 - 2, BOX_Y2, slot_w + 4, 2, 0, bcol, b_alf);
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1 - 2, BOX_Y1 - 2, 2, slot_h + 4, 0, bcol, b_alf);
        draw_sprite_ext(spr_pxwhite, 0, BOX_X2, BOX_Y1 - 2, 2, slot_h + 4, 0, bcol, b_alf);
        draw_sprite_ext(spr_pxwhite, 0, BOX_X1 + 6, BOX_Y1 + 21, slot_w - 12, 1, 0, bcol, b_alf * 0.65);
        draw_set_color(bcol);
        draw_set_alpha(1);
        draw_set_font(fnt_main);
        var slot_tag = (MENU_NO == 10 || MENU_NO == 11) ? ("BL" + string(global.chapter - 1) + " KAYIT 0" + string(i + 1)) : ("KAYIT 0" + string(i + 1));
        draw_text_transformed(BOX_X1 + 8, BOX_Y1 + 4, slot_tag, 0.75, 0.75, 0);
        if (CONT_THIS < 4 || CONT_THIS == 11)
        {
            var NOWNAME = (FILE[i] == 0) ? "[BOS]" : NAME[i];
            var NOWTIME = TIME_STRING[i];
            if (MENU_NO == 10 || MENU_NO == 11)
            {
                var filefound = 0;
                if (INCOMPLETE_LOAD == 0 && COMPLETEFILE_PREV[i] == 1)
                {
                    filefound = 1;
                }
                if (INCOMPLETE_LOAD == 1 && INCOMPLETEFILE_PREV[i] == 1)
                {
                    filefound = 1;
                }
                if (filefound)
                {
                    NOWNAME = COMPLETEFILE_PREV_NAME[i];
                    NOWTIME = COMPLETETIME_STRING[i];
                }
                else if (INCOMPLETE_LOAD)
                {
                    NOWNAME = "Dosya bulunamadi.";
                }
                else
                {
                    NOWNAME = "Tamamlama dosyasi yok.";
                    NOWTIME = " ";
                }
            }
            if (global.lang == "en")
            {
                if (scr_kana_check(NOWNAME))
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
                draw_set_font(fnt_ja_main);
            }
            draw_text_shadow(BOX_X1 + 22, BOX_Y1 + 25, NOWNAME);
            scr_84_set_draw_font("main");
            draw_set_halign(fa_right);
            draw_text_shadow(BOX_X2 - 8, BOX_Y1 + 3, NOWTIME);
            draw_set_halign(fa_left);
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
                if (MENU_NO == 11)
                {
                    SELTEXT_A = "Basla";
                    SELTEXT_B = "Geri";
                    SELTEXT_C = " ";
                }
            }
            draw_set_color(COL_B);
            if (MENU_NO == 7)
            {
                draw_set_color(c_red);
            }
            if (SELTEXT_C != " ")
            {
                draw_text_shadow(BOX_X1 + 10, BOX_Y1 + 25, SELTEXT_C);
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
            var NOWPLACE = PLACE[i];
            if (MENU_NO == 10 || MENU_NO == 11)
            {
                if (COMPLETEFILE_PREV[i] == 1)
                {
                    NOWPLACE = "Bolum " + string(global.chapter - 1) + " Tamamlandi";
                }
                else
                {
                    NOWPLACE = "[Bitis jeneriginde olusur.]";
                }
                if (INCOMPLETE_LOAD)
                {
                    NOWPLACE = COMPLETEFILE_PREV_PLACE[i];
                }
            }
            draw_text_shadow_width(BOX_X1 + 22, BOX_Y1 + 45, NOWPLACE, 164);
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
            if (MENU_NO == 10)
            {
                st_txt = "[ Z : ONCEKI KAYDI YUKLE ]";
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
}
if (MENU_NO >= 0)
{
    if (MENU_NO == 0 || MENU_NO == 2 || MENU_NO == 3 || MENU_NO == 5 || MENU_NO == 10)
    {
        if (MENUCOORD[MENU_NO] >= 0 && MENUCOORD[MENU_NO] <= 2)
        {
            var cur_slot_x = round((63 + (MENUCOORD[MENU_NO] * 208)) - _slot_scroll);
            HEARTX = cur_slot_x + 8;
            HEARTY = 62;
        }
    }
    if (MENU_NO == 2 || MENU_NO == 3 || MENU_NO == 5 || MENU_NO == 10 || MENU_NO == 11)
    {
        CANCELTEXT = (TYPE == 1) ? "Iptal" : "IPTAL";
        if (MENU_NO == 10 || MENU_NO == 11)
        {
            CANCELTEXT = "Bolum " + string(global.chapter - 1) + " Kaydini Kullanma";
        }
        var c_sel = MENU_NO < 20 && MENUCOORD[PREV_MENU] == 3;
        var cw = (MENU_NO == 10 || MENU_NO == 11) ? 216 : 132;
        var cx1 = 160 - (cw * 0.5);
        var cy1 = c_sel ? 148 : 151;
        var cx2 = 160 + (cw * 0.5);
        var cy2 = cy1 + 42;
        var ccol = c_sel ? COL_B : COL_A;
        draw_sprite_ext(spr_pxwhite, 0, cx1, cy1, cw, cy2 - cy1, 0, make_color_rgb(4, 14, 20), c_sel ? 0.88 : 0.65);
        draw_sprite_ext(spr_pxwhite, 0, cx1 - 2, cy1 - 2, cw + 4, 2, 0, ccol, c_sel ? 1 : 0.6);
        draw_sprite_ext(spr_pxwhite, 0, cx1 - 2, cy2, cw + 4, 2, 0, ccol, c_sel ? 1 : 0.6);
        draw_sprite_ext(spr_pxwhite, 0, cx1 - 2, cy1 - 2, 2, (cy2 - cy1) + 4, 0, ccol, c_sel ? 1 : 0.6);
        draw_sprite_ext(spr_pxwhite, 0, cx2, cy1 - 2, 2, (cy2 - cy1) + 4, 0, ccol, c_sel ? 1 : 0.6);
        draw_set_color(ccol);
        draw_set_halign(fa_center);
        draw_text_shadow(160, cy1 + 12, CANCELTEXT);
        draw_set_halign(fa_left);
        if (c_sel)
        {
            HEARTX = cx1 + 8;
            HEARTY = cy1 + 17;
        }
    }
    if (MENU_NO == 0 || MENU_NO == 1)
    {
        COPYTEXT = (TYPE == 1) ? "Kopyala" : "KOPYALA";
        ERASETEXT = (TYPE == 1) ? "Sil" : "SIL";
        CHFILETEXT = "Bl " + string(global.chapter - 1) + " Kayitlari";
        CHSELECTTEXT = "Bolum Secimi";
        QUITTEXT = "Oyundan Cik";
        LANGUAGETEXT = (global.lang == "en") ? stringset("æ—¥æœ¬èªž") : stringset("Turkce");
        var btn_labels = [COPYTEXT, ERASETEXT, CHFILETEXT, LANGUAGETEXT, CHSELECTTEXT, QUITTEXT];
        var btn_tags = ["01 / KOPYALA", "02 / SIL", "03 / ONCEKI BL", "04 / DIL", "05 / BOLUMLER", "06 / CIKIS"];
        var btn_count = (CANQUIT == 1) ? 6 : 5;
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
            draw_sprite_ext(spr_pxwhite, 0, bx1, by1, btn_w, btn_h, 0, make_color_rgb(4, 14, 20), b_sel ? 0.9 : 0.64);
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
        var on_btns = MENU_NO == 0 && MENUCOORD[0] >= 3;
        for (var b = 0; b < btn_count; b++)
        {
            var b_on = b == _last_btn;
            draw_set_color(b_on ? COL_B : COL_A);
            draw_set_alpha((on_btns ? 1 : 0.5) * (b_on ? 1 : 0.45));
            var dx = (160 - ((btn_count - 1) * 10 * 0.5)) + (b * 10);
            draw_rectangle(dx - (b_on ? 3 : 1), 203, dx + (b_on ? 3 : 1), 206, false);
        }
        if (on_btns)
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
        draw_text_transformed(195, 230, "DELTARUNE " + version_text + " (C) Toby Fox 2018-2026 ", 0.5, 0.5, 0);
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
                TEMPCOMMENT = stringsetloc(" ", "DEVICE_MENU_slash_Draw_0_gml_215_0");
            }
            if (MENU_NO == 2)
            {
                TEMPCOMMENT = "KOPYALANACAK OLANI SEC.";
            }
            if (MENU_NO == 3)
            {
                TEMPCOMMENT = "YANSIMA ICIN HEDEFI SEC.";
            }
            if (MENU_NO == 4)
            {
                TEMPCOMMENT = "BIRLESTIRILECEK.";
            }
            if (MENU_NO == 5 || MENU_NO == 6 || MENU_NO == 7)
            {
                TEMPCOMMENT = "SILINECEK OLANI SEC.";
            }
        }
        if (TYPE == 1)
        {
            if (MENU_NO == 0 || MENU_NO == 1)
            {
                TEMPCOMMENT = "Lutfen bir kayit dosyasi secin.";
            }
            if (MENU_NO == 2)
            {
                TEMPCOMMENT = "Kopyalanacak dosyayi secin.";
            }
            if (MENU_NO == 3)
            {
                TEMPCOMMENT = "Kopyalanacak hedefi secin.";
            }
            if (MENU_NO == 4)
            {
                TEMPCOMMENT = "Dosyanin uzerine yazilacak.";
            }
            if (MENU_NO == 5 || MENU_NO == 6 || MENU_NO == 7)
            {
                TEMPCOMMENT = "Silinecek dosyayi secin.";
            }
            if (MENU_NO == 10)
            {
                TEMPCOMMENT = "Bolum " + string(global.chapter) + "'e Bolum " + string(global.chapter - 1) + " kaydiyla basla.";
            }
            if (MENU_NO == 11)
            {
                TEMPCOMMENT = "Bolum " + string(global.chapter) + ", Kayit " + string(MENUCOORD[10] + 1) + " icinde baslayacak.";
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
if (MENU_NO >= 0)
{
    draw_sprite(spr_heartsmall, 0, HEARTXCUR, HEARTYCUR);
}
draw_set_font(fnt_main);
draw_set_color(COL_B);
draw_text_transformed(camerax() + 8, cameray() + 3, "BOLUM " + string(global.chapter), 0.75, 0.75, 0);
draw_set_color(c_white);
scr_84_set_draw_font("main");
